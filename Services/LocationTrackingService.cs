using HeatMapApp.Models;

namespace HeatMapApp.Services;

/// <summary>
/// Periodically reads the device position and persists it to SQLite.
/// </summary>
public class LocationTrackingService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);

    private readonly LocationDatabase _database;
    private CancellationTokenSource? _cts;

    /// <summary>Raised whenever a new point has been saved.</summary>
    public event EventHandler<LocationPoint>? LocationSaved;

    public LocationTrackingService(LocationDatabase database)
    {
        _database = database;
    }

    /// <summary>Gets a value indicating whether tracking is running.</summary>
    public bool IsTracking => _cts is not null;

    /// <summary>Requests permission and starts the tracking loop.</summary>
    public async Task StartAsync()
    {
        if (IsTracking)
        {
            return;
        }

        PermissionStatus status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            throw new PermissionException("Location permission was denied.");
        }

        _cts = new CancellationTokenSource();
        _ = TrackLoopAsync(_cts.Token);
    }

    /// <summary>Stops the tracking loop.</summary>
    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;
    }

    private async Task TrackLoopAsync(CancellationToken token)
    {
        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

        while (!token.IsCancellationRequested)
        {
            try
            {
                Location? location = await Geolocation.Default.GetLocationAsync(request, token);
                if (location is not null)
                {
                    var point = new LocationPoint
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        TimestampUtc = DateTime.UtcNow
                    };

                    await _database.SaveAsync(point);
                    LocationSaved?.Invoke(this, point);
                }

                await Task.Delay(PollInterval, token);
            }
            catch (OperationCanceledException)
            {
                // Expected when Stop() is called.
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Location error: {ex.Message}");
            }
        }
    }
}