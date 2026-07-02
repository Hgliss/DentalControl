using System.IO;

namespace DentalControl.Data.Database;

public static class DatabaseProvider
{
    private const string DatabaseFolder = "Database";
    private const string DatabaseName = "dentalcontrol.db";

    public static string GetConnectionString()
    {
        var folder = Path.Combine(
            AppContext.BaseDirectory,
            DatabaseFolder);

        Directory.CreateDirectory(folder);

        var databasePath = Path.Combine(
            folder,
            DatabaseName);

        return $"Data Source={databasePath}";
    }
}