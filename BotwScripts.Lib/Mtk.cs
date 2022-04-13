using Acheron.Web;
using System.Text;
using System.Text.Json;

namespace BotwScripts.Lib
{
    public class Mtk
    {
        // static
        public static string StaticPath { get; set; } = $"{GetEnv("localappdata")}\\mtk";
        public static string RemotePath { get; set; } = $"https://raw.githubusercontent.com/ArchLeaders/Modding-Toolkit/master";

        public static TValue? LoadJson<TValue>(string json) => JsonSerializer.Deserialize<TValue>(json);
        public static TValue? LoadJson<TValue>(byte[] json) => JsonSerializer.Deserialize<TValue>(json);

        public static string? GetEnv(string var) => Environment.GetEnvironmentVariable(var);
        public static string GetLocal(string config) => $"{StaticPath}\\{config}.json";
        public static string GetRemote(string path) => $"{RemotePath}/{path}";

        public static JsonElement? GetConfig(string config)
        {
            if (!File.Exists(GetLocal("config")))
                return null;

            var json = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText(GetLocal("config")));

            if (json == null)
                return null;

            return (JsonElement)json[config];
        }

        public static void UpdateScript(string module)
        {
            // Find local script
            string localScript = $"{StaticPath}\\Scripts\\{module}.py";
            string localVersions = GetLocal("versions");

            // Check local script
            if (!File.Exists(localScript))
            {
                Update();
                return;
            }

            // Check for local versions file
            if (!File.Exists(localVersions))
                GetRemote("BotwScripts.Lib/versions.json").DownloadFile(localVersions);

            // Get local version database
            Dictionary<string, string>? localJson = LoadJson<Dictionary<string, string>>(File.ReadAllText(localVersions));

            // Get remote version database
            Dictionary<string, string>? remoteJson = LoadJson<Dictionary<string, string>>(GetRemote("BotwScripts.Lib/versions.json").GetBytes());

            if (localJson == null || remoteJson == null)
                return;

            string[] localVersion = localJson[module].Split('.');
            string[] remoteVersion = remoteJson[module].Split('.');

            // Check major
            if (int.Parse(remoteVersion[0]) > int.Parse(localVersion[0]))
                Update();

            // Check mid
            else if (int.Parse(remoteVersion[1]) > int.Parse(localVersion[1]))
                Update();

            // Check minor
            else if (int.Parse(remoteVersion[2]) > int.Parse(localVersion[2]))
                Update();

            void Update()
            {
                GetRemote($"BotwScripts.Lib/Python/{module}.py").DownloadFile(localScript, true);
            }
        }

        // non-static

        public Mtk()
        {
            Config = LoadJson<Dictionary<string, JsonElement>>(GetLocal("config")) ?? new Dictionary<string, JsonElement>();

            // set configs
            Python = Config["python"].GetString() ?? "";
        }

        public Dictionary<string, JsonElement> Config { get; set; } = new();

        // loaded configs
        public string InstallPath { get; set; } = "";
        public string Python { get; set; } = "";
    }
}
