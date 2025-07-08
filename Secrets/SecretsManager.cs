using System;
using System.IO;
using Newtonsoft.Json;

namespace Mundo.Secrets
{
    /// <summary>
    /// Handles loading of sensitive bot secrets such as Discord token and DB connection string.
    /// Reads from Secrets/secrets.json at runtime; guides setup if missing.
    /// </summary>
    public class SecretsManager
    {
        /// <summary>
        /// POCO representing application secrets.
        /// </summary>
        public class AppSecrets
        {
            /// <summary>
            /// Discord bot token for authentication.
            /// </summary>
            public string DiscordToken { get; set; }
            /// <summary>
            /// SQLite connection string.
            /// </summary>
            public string ConnectionString { get; set; }
        }

        /// <summary>
        /// Loads application secrets from Secrets/secrets.json.
        /// Throws a helpful error if the file is missing.
        /// </summary>
        public static AppSecrets Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Secrets", "secrets.json");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "Secrets/secrets.json not found! Please copy Secrets/secrets.json.example and fill in your values.");
            }
            var json = File.ReadAllText(path);
            var secrets = JsonConvert.DeserializeObject<AppSecrets>(json);
            if (string.IsNullOrWhiteSpace(secrets?.DiscordToken) || string.IsNullOrWhiteSpace(secrets?.ConnectionString))
                throw new Exception("Invalid secrets.json: Make sure DiscordToken and ConnectionString are set.");
            return secrets;
        }
    }
}