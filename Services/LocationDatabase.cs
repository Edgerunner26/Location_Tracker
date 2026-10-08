using HeatMapApp.Models;
using SQLite;

namespace HeatMapApp.Services;

/// <summary>
/// Provides asynchronous access to the SQLite database that stores locations.
/// </summary>
public class LocationDatabase
{
    private const string DatabaseFileName = "locations.db3";

    private SQLiteAsyncConnection? _connection;

    /// <summary>
    /// Lazily opens the connection and creates the table on first use.
    /// </summary>
    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
        {
            return _connection;
        }

        string path = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
        _connection = new SQLiteAsyncConnection(
            path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _connection.CreateTableAsync<LocationPoint>();
        return _connection;
    }

    /// <summary>Saves a new location point.</summary>
    public async Task<int> SaveAsync(LocationPoint point)
    {
        SQLiteAsyncConnection db = await GetConnectionAsync();
        return await db.InsertAsync(point);
    }

    /// <summary>Returns every stored location ordered by time.</summary>
    public async Task<List<LocationPoint>> GetAllAsync()
    {
        SQLiteAsyncConnection db = await GetConnectionAsync();
        return await db.Table<LocationPoint>()
                       .OrderBy(p => p.TimestampUtc)
                       .ToListAsync();
    }
}
