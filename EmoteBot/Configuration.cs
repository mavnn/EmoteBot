using System;
using System.IO;
using System.Linq;

namespace EmoteBot
{
    public class Configuration
    {
        public string Nickname;
        public string Server;
        public string UserJid;
        public string Password;
        public string RoomJid;
        public string UserName;
        public string WeatherApiKey;

        public Configuration()
        {
            var dir = new DirectoryInfo(".");
            try
            {
                GetConfiguration(dir);

            }
            catch (Exception e)
            {
                throw new Exception("I couldn't find any configuration!", e);
            }
        }

        private void GetConfiguration(DirectoryInfo dir)
        {
            var cfg = dir.GetFiles("emotebot.cfg");
            if (cfg.Length > 0)
            {
                using (var file = File.OpenText(cfg.First().FullName))
                {
                    file.ReadLine();
                    Nickname = file.ReadLine();
                    Server = file.ReadLine();
                    UserJid = file.ReadLine();
                    Password = file.ReadLine();
                    RoomJid = file.ReadLine();
                    UserName = file.ReadLine();
                    WeatherApiKey = file.ReadLine();
                }
            }
            else
            {
                GetConfiguration(dir.Parent);
            }
        }
    }
}
