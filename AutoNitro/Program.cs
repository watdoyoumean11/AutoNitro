using System;
using System.IO;
using Newtonsoft.Json;
using Discord;
using Discord.Gateway;
using System.Threading;

namespace AutoNitro
{
    class Program
    {
        public static DiscordSocketClient Client { get; private set; }

        static void Main(string[] args)
        {
            Console.Title = "AutoNitro - By iLinked";

            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));

            while (string.IsNullOrEmpty(config.Token))
            {
                Console.Write("Your Discord token: ");
                config.Token = Console.ReadLine();
            }

            File.WriteAllText("Config.json", JsonConvert.SerializeObject(config));

            Client = new DiscordSocketClient();
            Client.OnLoggedIn += Client_OnLoggedIn;
            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.Login(config.Token);

            Thread.Sleep(-1);
        }

        private static void Client_OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
        {
            int i = args.Message.Content.IndexOf("discord.gift/");

            if (i != -1)
            {
                string gift = args.Message.Content.Substring(i + 13);

                if (gift.Length == 16)
                {
                    try
                    {
                        Client.RedeemNitroGift(gift, args.Message.ChannelId);
                    }
                    catch (DiscordHttpException ex)
                    {
                        switch (ex.Code)
                        {
                            case DiscordError.NitroGiftRedeemed:
                                Console.WriteLine("[ERROR] Nitro gift already redeemed: " + gift);
                                break;
                            case DiscordError.UnknownGiftCode:
                                Console.WriteLine("[ERROR] Invalid nitro gift: " + gift);
                                break;
                            default:
                                Console.WriteLine($"[ERROR] Unknown error: {ex.Code} | {ex.ErrorMessage}");
                                break;
                        }
                    }
                }
            }
        }

        private static void Client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            Console.Title = $"AutoNitro - By iLinked | Account: {args.User}";

            Console.WriteLine($"[SUCCESS] Logged into {args.User}!");
        }
    }
}
