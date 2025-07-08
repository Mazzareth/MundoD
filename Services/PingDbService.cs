using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Mundo.Services
{
    /// <summary>
    /// Service for recording 'ping' command usage in a lightweight local SQLite database.
    /// Ensures table existence and supports logging each ping with user and time.
    /// </summary>
    public class PingDbService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructs the PingDbService and ensures the PingLog table exists.
        /// </summary>
        /// <param name="connectionString">SQLite connection string for the database file.</param>
        public PingDbService(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTable();
        }

        /// <summary>
        /// Ensures the PingLog table exists, creating it if missing.
        /// </summary>
        private void EnsureTable()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS PingLog (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId TEXT NOT NULL,
                        Timestamp TEXT NOT NULL
                    );";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Logs a ping command by storing the user ID and UTC timestamp in the database.
        /// </summary>
        /// <param name="userId">Discord user ID who issued !ping.</param>
        public void LogPing(ulong userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText =
                    "INSERT INTO PingLog (UserId, Timestamp) VALUES (@uid, @ts);";
                cmd.Parameters.AddWithValue("@uid", userId.ToString());
                cmd.Parameters.AddWithValue("@ts", DateTime.UtcNow.ToString("o"));
                cmd.ExecuteNonQuery();
            }
        }
    }
}