﻿using Discord;
using Discord.WebSocket;
using DTBot_Template.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DTBot_Template
{
    public class Discord : Generics.BaseBot
    {
        #region Fields

        private DiscordSocketClient _client;

        #endregion Fields

        #region Methods

        private async Task MessageReceived(SocketMessage args)
        {
            if (!args.Author.IsBot)
            {
                if (args.Content[0] == Command)
                {
                    Generics.Command command = new Generics.Command(args);
                    await CommandHandler(command, this, CacheHandler.FindCurrency(((SocketGuildChannel)command.discord_Source.Channel).Guild.Id.ToString(), command.Source));
                }
                else
                {
                    Generics.Message message = new Generics.Message(args);
                    await MessageHandler(message, this, CacheHandler.FindCurrency(((SocketGuildChannel)message.discord_Source.Channel).Guild.Id.ToString(), message.Source));
                }
            }
        }

        private async Task Start(string token)
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        #endregion Methods

        #region Constructors

        public Discord(string token, char Command = '!') : base(Command)
        {
            _client = new DiscordSocketClient();

            _client.MessageReceived += MessageReceived;

            new Thread(async () => await Start(token)).Start();
        }

        #endregion Constructors

        public override async Task SendDM(Generics.User user, string Message)
        {
            await user.SendDM(Message);
        }

        public async override Task SendMessage(Generics.Channel channel, string Message)
        {
            await channel.SendMessage(Message);
        }
    }
}