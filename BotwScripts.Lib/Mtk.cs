using Acheron.Web;
using System.Operations;
using System.Text;
using System.Text.Json;

namespace BotwScripts.Lib
{
    public static class Mtk
    {
        // static
        public static string StaticPath { get; set; } = $"{GetEnv("localappdata")}\\mtk";
        public static string RemotePath { get; set; } = $"https://raw.githubusercontent.com/ArchLeaders/Modding-Toolkit/master";

        public static TValue? LoadJson<TValue>(string json) => JsonSerializer.Deserialize<TValue>(json);
        public static TValue? LoadJson<TValue>(byte[] json) => JsonSerializer.Deserialize<TValue>(json);

        public static string? GetEnv(string var) => Environment.GetEnvironmentVariable(var);
        public static string GetLocal(string config) => $"{StaticPath}\\{config}.json";
        public static string GetDynamic(string config, string folder = "Data") => $"{GetConfig("dynamic")}\\{folder}\\{config}";
        public static string GetRemote(string path) => $"{RemotePath}/{path}";

        public static JsonElement? GetConfig(string config)
        {
            if (!File.Exists(GetLocal("config")))
                return null;

            var json = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText(GetLocal("config")));

            if (json == null)
                return null;

            return (JsonElement)json[config.ToLower()];
        }

        public static void UpdateExternal(string module, string localPath = "%localappdata%\\mtk\\Scripts", string remotePath = "BotwScripts.Lib/Python")
        {
            // Reformat input paths
            localPath = localPath.ParsePathVars();

            // Find local path
            localPath = $"{localPath}\\{module}";

            // Check local script
            if (!File.Exists(localPath))
            {
                Update();
                return;
            }

            // Check the version
            if (CheckUpdate(module))
                Update();

            void Update() => GetRemote($"{remotePath}/{module}").DownloadFile(localPath, true);
        }

        public static bool CheckUpdate(string key, int versionSections = 3)
        {
            string versions = GetLocal("versions");

            // Check for local versions file
            if (!File.Exists(versions))
                GetRemote("BotwScripts.Lib/versions.json").DownloadFile(versions);

            // Get local version database
            Dictionary<string, string>? localJson = LoadJson<Dictionary<string, string>>(File.ReadAllText(versions));

            // Get remote version database
            Dictionary<string, string>? remoteJson = LoadJson<Dictionary<string, string>>(GetRemote("BotwScripts.Lib/versions.json").GetBytes());

            if (localJson == null || remoteJson == null)
                return false;

            if (!localJson.ContainsKey(key))
                GetRemote("BotwScripts.Lib/versions.json").DownloadFile(versions);

            if (!localJson.ContainsKey(key))
                return false;

            bool CheckIndex(int indx) => int.Parse(remoteJson[key].Split('.')[indx]) > int.Parse(localJson[key].Split('.')[indx]);

            for (int i = 0; i < versionSections; i++)
                if (CheckIndex(i)) return true;

            return false;
        }

        public static string GetRoot(this string path)
        {
            string[] valid = new string[]
            {
                "content",
                "aoc",
                "logs",
                "patches"
            };

            foreach (var folder in valid)
            {
                DirectoryInfo dir = new(path);

                // path..\
                if (dir.Name == folder) return path.EditPath();

                // path
                else if (Directory.Exists($"{path}\\{folder}")) return path;

                // path
                else if (Directory.Exists($"{path}\\Build\\{folder}")) return path;

                // path../
                else if (Directory.Exists($"{path.SubFolder()}\\{folder}")) return path;
            }

            return "N/A";
        }
    }
}
