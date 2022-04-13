using Acheron.Web;
using System.Text.Json;

namespace BotwScripts.Lib
{
    public class Mtk
    {
        // static
        public static string StaticPath { get; set; } = $"{GetEnv("localappdata")}\\mtk";
        public static string RemotePath { get; set; } = $"https://raw.githubusercontent.com/ArchLeaders/Modding-Toolkit/master";
        
        public static dynamic? LoadStatic(string config) => JsonSerializer.Deserialize<dynamic>(File.ReadAllText(GetStatic(config)));
        public static async Task<dynamic>? LoadRemote(string path) => await JsonSerializer.Deserialize<dynamic>(BitConverter.ToString(await GetRemote(path).GetBytes()));

        public static string? GetEnv(string var) => Environment.GetEnvironmentVariable(var);
        public static string GetStatic(string config) => $"{StaticPath}\\{config}.json";
        public static string GetRemote(string path) => $"{RemotePath}/{path}";

        public static dynamic GetConfig(string config)
        {
            if (!File.Exists(GetStatic("config")))
                return "0:GetConfig:Error - Config could not be found";

            dynamic? json = LoadStatic("config");

            if (json == null)
                return "1:GetConfig:Error - Config returned null";

            return json[config];
        }

        public static void UpdateScript(string module)
        {
            // Find local script
            string localScript = $"{StaticPath}\\Scripts\\{module}.py";

            // Check local script
            if (!File.Exists(localScript))
            {
                Update();
                return;
            }

            // Get local version database
            dynamic? localJson = LoadStatic("versions");

            // Get remote version database
            dynamic? remoteJson = LoadRemote("BotwScripts.Lib/versions.json");

            if (localJson == null || remoteJson == null)
                return;

            string[] localVersion = ((string)localJson[module]).Split();
            string[] remoteVersion = ((string)remoteJson[module]).Split();

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
            Config = LoadStatic("config") ?? new Dictionary<string, dynamic>();

            // set configs
            Python = Config["python"];
        }

        public Dictionary<string, dynamic> Config { get; set; } = new();

        // loaded configs
        public string InstallPath { get; set; } = "";
        public string Python { get; set; } = "";
    }
}
