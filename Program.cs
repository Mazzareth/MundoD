using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Mundo.Secrets;
using Mundo.Data;

namespace Mundo
{
    class Program
    {
        private DiscordSocketClient _client;
        private DatabaseService _db;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            // Load secrets
            var secrets = Secrets.Secrets.Load();
            if (string.IsNullOrWhiteSpace(secrets.BotToken) || secrets.BotToken == "YOUR_DISCORD_BOT_TOKEN_HERE")
            {
                Console.WriteLine("Bot token missing in Secrets/secrets.json. Please set your Discord bot token.");
                return;
            }

            // Init DB
            _db = new DatabaseService();

            // Discord client
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged
            };
            _client = new DiscordSocketClient(config);

            _client.Log += LogAsync;
            _client.Ready += OnReadyAsync;
            _client.MessageReceived += HandleMessageReceivedAsync;

            await _client.LoginAsync(TokenType.Bot, secrets.BotToken);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task OnReadyAsync()
        {
            Console.WriteLine($"Bot is connected as {_client.CurrentUser}");
            return Task.CompletedTask;
        }

        private async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            // Ignore self, bots, and system messages
            if (!(message is SocketUserMessage userMsg)) return;
            if (userMsg.Author.IsBot) return;

            if (userMsg.Content.Trim().Equals("!ping", StringComparison.OrdinalIgnoreCase))
            {
                await message.Channel.SendMessageAsync("Pong!");
                await _db.LogPing(userMsg.Author.Id);
            }
        }
    }
}