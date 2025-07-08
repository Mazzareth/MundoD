using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Mundo.Services;
using Mundo.Secrets;

namespace Mundo
{
    class Program
    {
        private DiscordSocketClient _client;
        private PingDbService _db;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var secrets = SecretsManager.Load();
            _db = new PingDbService(secrets.ConnectionString);
            _client = new DiscordSocketClient();
            _client.Log += msg => { Console.WriteLine(msg.ToString()); return Task.CompletedTask; };
            _client.MessageReceived += HandleMessageAsync;
            await _client.LoginAsync(TokenType.Bot, secrets.DiscordToken);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;
            if (message.Content.Equals("!ping", StringComparison.OrdinalIgnoreCase))
            {
                _db.LogPing(message.Author.Id);
                await message.Channel.SendMessageAsync("Pong!");
            }
        }
    }
}