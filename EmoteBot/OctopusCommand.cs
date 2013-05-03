using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EmoteBot
{
    public class OctopusCommand
    {
        private Configuration configuration;
        private readonly Regex extractor;

        public OctopusCommand(Configuration cfg)
        {
            configuration = cfg;
            extractor = new Regex("@" + cfg.Nickname + @" deploy (?<project>.*)$");
        }

        public bool IsOctopusCommand(string body)
        {
            return extractor.IsMatch(body);
        }

        public async Task<string> Deploy(string body)
        {
            await Task.Delay(1500); 
            return await Task.Run(() => { return string.Format("You tried to deploy project {0}", extractor.Match(body).Groups["project"].Value); });
        }
    }
}
