using System;
using System.IO;
using Newtonsoft.Json;

namespace Mundo.Secrets
{
    public class SecretsModel
    {
        public string BotToken { get; set; }
    }

    public static class Secrets
    {
        public static SecretsModel Load()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string secretsPath = Path.Combine(baseDir, "Secrets", "secrets.json");
            if (!File.Exists(secretsPath))
                throw new FileNotFoundException($"Secrets file not found: {secretsPath}");
            string json = File.ReadAllText(secretsPath);
            return JsonConvert.DeserializeObject<SecretsModel>(json);
        }
    }
}