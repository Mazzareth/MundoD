using System;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Dapper;

namespace Mundo.Data
{
    public class DatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "Data");
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);
            _dbPath = Path.Combine(dataDir, "mundo.sqlite");
            EnsureDatabase().Wait();
        }

        private async Task EnsureDatabase()
        {
            bool newDb = !File.Exists(_dbPath);
            using (var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                await connection.OpenAsync();
                if (newDb)
                {
                    // Will be created automatically by SQLiteConnection
                }
                await connection.ExecuteAsync(
                    @"CREATE TABLE IF NOT EXISTS PingLogs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId TEXT NOT NULL,
                        Timestamp INTEGER NOT NULL
                    );");
            }
        }

        public async Task LogPing(ulong userId)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                await connection.OpenAsync();
                var sql = "INSERT INTO PingLogs (UserId, Timestamp) VALUES (@UserId, @Timestamp);";
                await connection.ExecuteAsync(sql, new
                {
                    UserId = userId.ToString(),
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });
            }
        }
    }
}