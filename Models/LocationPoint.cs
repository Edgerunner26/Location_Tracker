using SQLite;

namespace HeatMapApp.Models;

/// <summary>
/// Represents a single recorded GPS position stored in SQLite.
/// </summary>
public class LocationPoint
{
    /// <summary>Gets or sets the auto-incremented primary key.</summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>Gets or sets the latitude in decimal degrees.</summary>
    public double Latitude { get; set; }

    /// <summary>Gets or sets the longitude in decimal degrees.</summary>
    public double Longitude { get; set; }

    /// <summary>Gets or sets the UTC time the position was captured.</summary>
    public DateTime TimestampUtc { get; set; }
}
