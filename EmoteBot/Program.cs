using System;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;

namespace EmoteBot
{
    class Program
    {
        private static string nickname;
        private static string userName;
        private static readonly Random Random = new Random();
        private static WeatherCommand Weather;

        private static readonly string[] Responses = new[]
            {
                "/em dances happy dance because someone mentioned it.",
                "(awthanks)",
                "(shamrock)",
                "(pirate)",
                "(tea)",
                "(thumbsup)",
                "I love (15b)!",
                "(allthethings)"
            };
        static private XmppClientConnection client;
        private static Jid room;
        static private Regex hasMention;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            var cfg = new Configuration();
            nickname = cfg.Nickname;
            userName = cfg.UserName;
            room = new Jid(cfg.RoomJid);
            hasMention = new Regex(string.Format(".*@{0}.*", nickname));
            Weather = new WeatherCommand(cfg);
            client = new XmppClientConnection(cfg.Server);
            client.Open(cfg.UserJid, cfg.Password);
            client.OnLogin += OnLogin;

            AsyncMain().Wait();
            Console.WriteLine("Shutting down");
            Console.ReadLine();
        }

        private static async Task AsyncMain()
        {
            var messages = Observable.FromEventPattern<MessageHandler, Message>(h => client.OnMessage += h,
                                                                                h => client.OnMessage -= h)
                                     .SkipUntil(DateTimeOffset.Now.AddSeconds(4))
                                     .Where(e => e.EventArgs.Type == MessageType.groupchat)
                                     .Where(e => HasMention(e.EventArgs))
                                     .Throttle(TimeSpan.FromSeconds(5));

            while (true)
            {
                var message = await messages.Take(1);
                Console.WriteLine(message.EventArgs.Body);
                client.Send(new Message(room, MessageType.groupchat, Responses[Random.Next(Responses.Length)]));
                if (Weather.IsWeatherCommand(message.EventArgs.Body))
                {
                    var weatherReport = await Weather.GetWeather(message.EventArgs.Body);
                    client.Send(new Message(room, MessageType.groupchat, weatherReport));
                }
                Console.WriteLine();
            }
        }

        static bool HasMention(Message msg)
        {
            return !string.IsNullOrEmpty(msg.Body) && hasMention.IsMatch(msg.Body);
        }

        static private void OnLogin(object e)
        {
            client.SendMyPresence();
            var mucManager = new MucManager(client);
            mucManager.AcceptDefaultConfiguration(room);
            mucManager.JoinRoom(room, userName);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Logged in.");
            client.Send(new Message(room, MessageType.groupchat, "I'm a more sensible emote bot. I should only respond once every 5 seconds, and I should ignore the Room history."));
            client.Send(new Message(room, MessageType.groupchat, "Please talk to me. I would like to be your friend."));
        }
    }
}
