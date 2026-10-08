using System.ComponentModel;
using System.Runtime.CompilerServices;
using HeatMapApp.Models;
using HeatMapApp.Services;

namespace HeatMapApp.ViewModels;

/// <summary>
/// View model for MainView; loads stored points and listens for new ones.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private readonly LocationDatabase _database;
    private readonly LocationTrackingService _tracker;
    private IList<LocationPoint> _points = new List<LocationPoint>();

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel(LocationDatabase database, LocationTrackingService tracker)
    {
        _database = database;
        _tracker = tracker;
        _tracker.LocationSaved += OnLocationSaved;
    }

    /// <summary>Gets the points bound to the heat map.</summary>
    public IList<LocationPoint> Points
    {
        get => _points;
        private set
        {
            _points = value;
            OnPropertyChanged();
        }
    }

    /// <summary>Loads history and starts tracking.</summary>
    public async Task InitializeAsync()
    {
        Points = await _database.GetAllAsync();
        await _tracker.StartAsync();
    }

    private void OnLocationSaved(object? sender, LocationPoint point)
    {
        // Assign a new list so the bindable property fires a redraw.
        MainThread.BeginInvokeOnMainThread(() =>
            Points = new List<LocationPoint>(Points) { point });
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}