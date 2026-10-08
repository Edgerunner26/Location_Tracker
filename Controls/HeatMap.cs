using HeatMapApp.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Circle = Microsoft.Maui.Controls.Maps.Circle;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HeatMapApp.Controls;

/// <summary>
/// Extends the .NET MAUI <see cref="Map"/> control with heat-map rendering.
/// Each location is drawn as a translucent circle; overlapping circles
/// in frequently visited places produce "hotter" (denser) colors.
/// </summary>
public class HeatMap : Map
{
    private const double PointRadiusMeters = 40;

    /// <summary>Bindable list of points to render.</summary>
    public static readonly BindableProperty PointsProperty = BindableProperty.Create(
        nameof(Points),
        typeof(IList<LocationPoint>),
        typeof(HeatMap),
        propertyChanged: (bindable, _, _) => ((HeatMap)bindable).Redraw());

    public HeatMap()
    {
        IsShowingUser = true;
    }

    /// <summary>Gets or sets the points shown as heat.</summary>
    public IList<LocationPoint>? Points
    {
        get => (IList<LocationPoint>?)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    /// <summary>Clears and redraws all heat circles.</summary>
    public void Redraw()
    {
        MapElements.Clear();
        if (Points is null || Points.Count == 0)
        {
            return;
        }

        foreach (LocationPoint point in Points)
        {
            int neighbors = CountNeighbors(point);
            Color heat = GetHeatColor(neighbors);

            MapElements.Add(new Circle
            {
                Center = new Location(point.Latitude, point.Longitude),
                Radius = Distance.FromMeters(PointRadiusMeters),
                StrokeColor = heat.WithAlpha(0.6f),
                StrokeWidth = 2,
                FillColor = heat.WithAlpha(0.35f)
            });
        }

        // Center the map on the latest recorded position.
        LocationPoint last = Points[^1];
        MoveToRegion(MapSpan.FromCenterAndRadius(
            new Location(last.Latitude, last.Longitude),
            Distance.FromKilometers(2)));
    }

    /// <summary>Counts points within 100 m to measure density.</summary>
    private int CountNeighbors(LocationPoint target)
    {
        var origin = new Location(target.Latitude, target.Longitude);
        return Points!.Count(p =>
            Location.CalculateDistance(origin, new Location(p.Latitude, p.Longitude),
                DistanceUnits.Kilometers) < 0.1);
    }

    /// <summary>Maps density to a blue→green→yellow→red gradient.</summary>
    private static Color GetHeatColor(int density) => density switch
    {
        <= 2 => Colors.Blue,
        <= 5 => Colors.LimeGreen,
        <= 10 => Colors.Yellow,
        _ => Colors.Red
    };
}