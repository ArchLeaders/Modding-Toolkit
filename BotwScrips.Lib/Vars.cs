using System.Text.Json;

namespace BotwScrips.Lib
{
    public class Vars
    {
        // static

        public static string ConfigPath { get; set; } = $"{GetEnv("localappdata")}\\mtk\\config.json";

        public static string? GetEnv(string var) => Environment.GetEnvironmentVariable(var);

        public static dynamic GetConfig(string config)
        {
            if (!File.Exists(ConfigPath))
                return "0:GetConfig:Error - Settings could not be found";

            var json = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText(ConfigPath));

            if (json == null)
                return "1:GetConfig:Error - Settings returned null";

            return json[config];
        }

        // non-static

        public Vars()
        {
            Config = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText(ConfigPath)) ?? new();

            // set configs
            Python = Config["python"];
        }

        public Dictionary<string, dynamic> Config { get; set; } = new();

        // loaded configs
        public string Python { get; set; } = "";
    }
}
