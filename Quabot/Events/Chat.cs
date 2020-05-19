﻿using DTBot_Template.Data;
using DTBot_Template.Generics;
using System.Threading.Tasks;

namespace DTBot_Template.Events
{
    public static class ChatEvents
    {
        #region Methods

        public static async Task HandleCommand(Command command, BaseBot Bot, CurrencyConfig currency)
        {
            _userInfo[] tBanks = CacheHandler.FindUsers(command.mentions, currency);
            _userInfo bank = CacheHandler.FindUser(command.sender, currency);

            switch (command.commandStr)
            {
                case "echo":
                    await Bot.SendMessage(command.channel, command.commandArgString,command.Source,currency);
                    break;

                case "echodm":
                    await Bot.SendDM(command.sender, command.commandArgString,command.Source,currency);
                    break;

                //case "WTF":
                //    //Streamlabs.CreateDonation("Jamm", 1, botConfig.Streamlabs);
                //    await Bot.SendMessage(command.channel, Streamlabs.GetDonations(botConfig.Streamlabs).ToString());
                //    break;

                case "bal":

                    if (command.mentions.Length == 0) await Bot.SendMessage(command, "{User} You Have {Value} {Currency}", currency, bank.balance, currency.name);
                    else await Bot.SendMessage(command, "{User} {User0} Has {Value} {Currency}", currency, tBanks[0].balance, currency.name);
                    break;

                case "pay":
                    if (command.mentions.Length > 0 && command.values.Length > 0)
                    {
                        if (bank.balance >= command.values[0])
                        {
                            bank.balance -= command.values[0];
                            tBanks[0].balance += command.values[0];
                            bank.Update();
                            tBanks[0].Update();
                            await Bot.SendMessage(command, "{User} Paid {Value0} {Currency} To {User0}", currency, CurrencyName: currency.name);
                        }
                        else await Bot.SendMessage(command, "{User} You Only Have {Value} {Currency}", currency, bank.balance, CurrencyName: currency.name);
                    }
                    else await Bot.SendMessage(command, "{User} You Fucked Up {NWord}", currency);
                    break;

                default:
                    if (currency.SimpleResponses.ContainsKey(command.commandStr)) await Bot.SendMessage(command, currency.SimpleResponses[command.commandStr], currency);
                    break;
            }
        }

        public static async Task HandleMessage(Message message, BaseBot Bot, CurrencyConfig currency)
        {
            _userInfo bank = CacheHandler.FindUser(message.sender, currency);
            Rewards.MessageRewardUser(bank);

            string[] iams = { "i am", "i'm", "im" };
            int index;

            string msg = message.body.ToLower();
            foreach (string iam in iams)
            {
                if (msg.Contains(iam))
                {
                    index = msg.IndexOf(iam) + iam.Length;
                    string newMsg = msg.Substring(index, msg.Length - index).Trim();

                    await Bot.SendMessage(message, $"Hi {newMsg}, Im Dad", currency);
                    break;
                }
            }
        }

        #endregion Methods
    }
}