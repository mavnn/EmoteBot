using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CGurus.Weather.WundergroundAPI;

namespace EmoteBot
{
    public class WeatherCommand
    {
        private Configuration configuration;
        private readonly WApi wApi;
        private readonly Regex extractor;

        public WeatherCommand(Configuration cfg)
        {
            configuration = cfg;
            wApi = new WApi(cfg.WeatherApiKey);
            extractor = new Regex("@" + cfg.Nickname + @" weather in (?<state>.{2}), (?<city>.*)$");
        }

        private Tuple<string, string> ExtractStateAndCity(string body)
        {
            var match = extractor.Match(body);
            return Tuple.Create(match.Groups["state"].Value, match.Groups["city"].Value);
        }

        public bool IsWeatherCommand(string body)
        {
            return extractor.IsMatch(body);
        }

        public Task<string> GetWeather(string body)
        {
            var stateAndCity = ExtractStateAndCity(body);
            Console.WriteLine("Trying to get weather for {0}, {1} because of message: {2}", stateAndCity.Item1, stateAndCity.Item2, body);
            return Task.Factory.StartNew(() => wApi.GetForecastUS(stateAndCity.Item1, stateAndCity.Item2).Forecast.Txt_Forecast.ForecastDay[0].FctText_Metric);
        }
    }
}
