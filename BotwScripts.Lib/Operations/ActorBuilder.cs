﻿using AampLibraryCSharp;
using AampLibraryCSharp.IO;
using Acheron.Web;
using BfresLibrary;
using BotwScripts.Lib.Formats;
using SARCExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Operations;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotwScripts.Lib.Operations
{
    public class ActorBuilder
    {
        public string Name { get; private set; }
        public string Root { get; set; }
        public string ModFolder { get; set; }
        public string HKRBPath { get; set; }
        public string Func { get; set; }

        public string FullName { get; set; } = "FldObj_ActorBuilder_A_01";
        public string Prefix { get; set; } = "FldObj";
        public string LetterId { get; set; } = "A";
        public string NumaricId { get; set; } = "01";

        public long HKRBSize { get; private set; }
        public string ActorPack { get; private set; }
        public string CafeResource { get; private set; }
        public int LifeCondition { get; private set; } = 500;
        public string? BaseActor { get; private set; } = null;

        public ActorBuilder(string modRoot, string hkrbFile, bool format)
        {
            if (!File.Exists(hkrbFile) || !hkrbFile.EndsWith(".hkrb"))
            {
                Console.WriteLine($"Error initializing ActorBuilder - Havok Rigid Body '{hkrbFile}' could not be found.");
                return;
            }

            // Get/Set filenames/dirs
            Root = modRoot;
            ModFolder = $"{Root}\\Build";
            HKRBPath = hkrbFile;
            HKRBSize = new FileInfo(HKRBPath).Length;
            FullName = new FileInfo(hkrbFile).Name.Replace(".hkrb", "");
            Func = "[ACTORBUILDER]";

            // Format name with Prefix_Name_LID_NID
            if (format)
            {
                // Load headers
                Mtk.UpdateExternal("Headers.json", $"{Mtk.GetConfig("dynamic")}\\Data", "BotwScripts.Lib/Data");
                List<string> headers = Mtk.LoadJson<List<string>>(File.ReadAllText(Mtk.GetDynamic("Headers.json"))) ?? new();

                string[] indx = FullName.Split('_');

                int dummy = 0;
                int len = indx.Length;

                for (int i = 0; i < len; i++)
                {
                    if (indx[i].Length == 1 && !int.TryParse(indx[i], out dummy)) LetterId = indx[i];

                    else if (indx[i].Length == 2 && int.TryParse(indx[i], out dummy)) NumaricId = indx[i];

                    else if (headers.Contains(indx[i])) Prefix = indx[i];

                    else Name += indx[i] + '_';
                }

                // Remove trailing underscores
                while (Name[Name.Length - 1] == '_')
                    Name = Name.Remove(Name.Length - 1);

                FullName = $"{Prefix}_{Name}_{LetterId}_{NumaricId}";
            }

            else Name = FullName;

            CafeResource = $"{ModFolder}\\content\\Model\\{Prefix}_{Name}_{LetterId}";
            ActorPack = $"{ModFolder}\\content\\Actor\\Pack\\{FullName}.sbactorpack";
        }

        public async Task<Reporter> Contruct(Notify print, Option option, Input input)
        {
            // Create data dirs
            Directory.CreateDirectory($"%temp%\\{FullName}".ParsePathVars());
            Directory.CreateDirectory($"{Root}\\Assets\\bin\\");
            Directory.CreateDirectory($"{Root}\\Build\\content\\");

            // REFERENCE
            print($"{Func}");

            // Handle existing files/folders
            if (File.Exists(ActorPack) || Directory.Exists(ActorPack))
            {
                if (option($"The actorpack '{FullName}' already exists. Overwrite it? "))
                {
                    if (File.Exists(ActorPack)) File.Move(ActorPack, $"{Root}\\Assets\\bin\\{FullName}.bkp", true);

                    else Directory.Move(ActorPack, $"{Root}\\Assets\\bin\\{FullName}.bkp");
                }
            }

            // Find nearest HKRB match
            print($"{Func} Searching HKRB file dictionary . . .");

            // Get HKRB Dictionary
            Mtk.UpdateExternal("HKRBCache.sjson", $"{Mtk.GetConfig("dynamic")}\\Data", "BotwScripts.Lib/Data");
            byte[] hkrbCacheBytes = Yaz0.Decompress(File.ReadAllBytes(Mtk.GetDynamic("HKRBCache.sjson")));

            Dictionary<long, string> hkrbDictionary = JsonSerializer.Deserialize<Dictionary<long, string>>(hkrbCacheBytes) ?? new();

            // Get closest match
            long nearest = hkrbDictionary.Keys.OrderBy(x => Math.Abs(x - HKRBSize)).First();
            print($"{Func} Found {hkrbDictionary[nearest]} at {nearest}");

            // Prep mod folder
            BaseActor = hkrbDictionary[nearest];
            string? update = Mtk.GetConfig("update_dir").ToString();
            string? game = Mtk.GetConfig("game_dir").ToString();

            if (update == null)
                return new("The update path returned null.\nThis could mean that BCML is not installed or setup correctly.", "ActorBuilder Exception");

            Dictionary<string, string> prep = new()
            {
                { "!dir__a", $"{ModFolder}\\content\\Actor\\Pack" },
                { "!dir__b", $"{ModFolder}\\content\\Model" },
                { $"{update}\\Actor\\ActorInfo.product.sbyml", $"{ModFolder}\\content\\Actor\\ActorInfo.product.sbyml" },
                { $"{update}\\Actor\\Pack\\{BaseActor}.sbactorpack", ActorPack },
                { $"{update}\\Model\\FldObj_HyruleFountain_A.sbfres", $"{CafeResource}.sbfres" },
                { $"{game}\\Model\\FldObj_HyruleFountain_A.Tex1.sbfres", $"{CafeResource}.Tex1.sbfres" },
            };

            foreach (var dataSet in prep)
            {
                if (dataSet.Key.StartsWith("!dir"))
                {
                    Directory.CreateDirectory(dataSet.Value);
                    continue;
                }

                else if (File.Exists(dataSet.Value))
                    File.Move(dataSet.Value, $"{Root}\\Assets\\bin\\{new FileInfo(dataSet.Value).Name}.bkp", true);
                
                File.Copy(dataSet.Key, dataSet.Value, true);
            }

            // Open actorpack
            SarcData actorpack = SARC.UnpackRamN(Yaz0.Decompress(File.ReadAllBytes(ActorPack)));

            // Stage actorpack edits
            List<Task> edit = new();

            // Set actorpack data vars
            bool modifiedBLifeCondition = false;
            Dictionary<string, byte[]> files = new();

            foreach (var file in actorpack.Files)
                files.Add(file.Key, file.Value);

            // Get LifeCondition
            int lfConditionInt = 0;
            string lfConditionStr = input($"{Func} Enter the LifeCondition distance in meters: ");

            while (!int.TryParse(lfConditionStr, out lfConditionInt))
            {
                print($"{Func}[EXC] | '{lfConditionStr}' is invalid.");
                lfConditionStr = input($"{Func} Enter the LifeCondition distance in meters: ");
            }

            string lfcUnit = int.Parse(lfConditionStr) < 1000 ?
                $"{lfConditionStr}m" : int.Parse(lfConditionStr) / 1000 > 9 ?
                $"{int.Parse(lfConditionStr) / 1000}km" : $"0{int.Parse(lfConditionStr) / 1000}km";

            // Iterate SARC files
            foreach (var file in files)
            {
                // Edit BLifeCondition
                if (file.Key.EndsWith(".blifecondition"))
                    edit.Add(BLifeCondition(file));

                // Edit BMmodelList
                if (file.Key.EndsWith(".bmodellist"))
                    edit.Add(BModelList(file));

                // Edit BPhysics
                if (file.Key.EndsWith(".bphysics"))
                    edit.Add(BPhysics(file));

                // Edit BXML
                if (file.Key.EndsWith(".bxml"))
                    edit.Add(BXML(file));
            }

            // Await common edits
            await Task.WhenAll(edit);

            // Create Life Condition
            if (!modifiedBLifeCondition)
                await CreateBLifeCondition();

            // Add HKRB file
            print($"{Func} Adding Havok RigidBodies . . .");
            actorpack.Files.Add($"Physics/RigidBody/{Prefix}_{Name}_{LetterId}/{FullName}.hkrb", File.ReadAllBytes(HKRBPath));

            // Write SARC file
            print($"{Func} Writing actorpack . . .");
            File.WriteAllBytes(ActorPack, Yaz0.Compress(SARC.PackN(actorpack).Item2, 9));

            // Edit SBFRES files
            // !! Needs to handle existing files !!
            print($"{Func} Parsing Cafe Resources . . .");
            ResFile res = Bfres.LoadBfres($"{CafeResource}.sbfres");
            res.Models.RemoveAt(1);
            res.Models[0].Name = FullName;
            res.Name = $"{Prefix}_{Name}_{LetterId}";

            res.Save($"{CafeResource}.sbfres");

            res = Bfres.LoadBfres($"{CafeResource}.Tex1.sbfres");
            res.Name = $"{Prefix}_{Name}_{LetterId}.Tex1";
            res.Save($"{CafeResource}.Tex1.sbfres");

            // Return result
            Directory.Delete($"%temp%\\{FullName}".ParsePathVars(), true);
            return new("success");

            Task BModelList(KeyValuePair<string, byte[]> file)
            {
                // Notify interface
                print($"{Func} Modify binary model list (AAMP)");

                // Parse bmodellist
                AampFile bmodellist = AampFile.LoadFile(new MemoryStream(file.Value));

                // Set model list data
                bmodellist.RootNode.childParams[0].childParams[0].childParams[0].paramObjects[0].paramEntries[0].Value = new StringEntry(FullName);
                bmodellist.RootNode.childParams[0].childParams[0].paramObjects[0].paramEntries[0].Value = new StringEntry($"{Prefix}_{Name}_{LetterId}");
                bmodellist.Save($"%temp%\\{FullName}\\bmodellist.temp.io".ParsePathVars());

                // Add model list file
                actorpack.Files.Remove(file.Key);
                actorpack.Files.Add($"Actor/ModelList/{FullName}.bmodellist", File.ReadAllBytes($"%temp%\\{FullName}\\bmodellist.temp.io".ParsePathVars()));

                // Return completed
                return Task.CompletedTask;
            }

            Task BLifeCondition(KeyValuePair<string, byte[]> file)
            {
                // Notify interface
                print($"{Func} Modify binary life condition (AAMP)");

                // Update creation status
                modifiedBLifeCondition = true;

                // Parse blifecondition
                AampFile blfc = AampFile.LoadFile(new MemoryStream(file.Value));

                // Set life condition data
                blfc.RootNode.paramObjects[0].paramEntries[0].Value = (float)lfConditionInt;
                blfc.Save($"%temp%\\{FullName}\\blifecondition.temp.io".ParsePathVars());

                // Add life condition file
                actorpack.Files.Remove(file.Key);
                actorpack.Files.Add($"Actor/LifeCondition/Landmark{lfcUnit}.blifecondition", File.ReadAllBytes($"%temp%\\{FullName}\\blifecondition.temp.io".ParsePathVars()));

                // Return completed
                return Task.CompletedTask;
            }

            Task CreateBLifeCondition()
            {
                // Notify interface
                print($"{Func} Create binary life condition (AAMP)");

                // Check default.blifecondition
                Mtk.UpdateExternal("Default.blifecondition", Mtk.GetConfig("dynamic") != null ? $"{Mtk.GetConfig("dynamic")}\\Data" : $"{Mtk.StaticPath}\\Data", "BotwScripts.Lib/Data");

                // Parse default.blifecondition
                AampFile blfc = AampFile.LoadFile(Yaz0.DecompressToStream(Mtk.GetDynamic("Default.sblifecondition")));

                // Set life condition data
                blfc.RootNode.paramObjects[0].paramEntries[0].Value = (float)lfConditionInt;
                blfc.Save($"%temp%\\{FullName}\\def__blifecondition.temp.io".ParsePathVars());

                // Add life condition file
                actorpack.Files.Add($"Actor/LifeCondition/Landmark{lfcUnit}.blifecondition", File.ReadAllBytes($"%temp%\\{FullName}\\def__blifecondition.temp.io".ParsePathVars()));

                // Return completed
                return Task.CompletedTask;
            }

            Task BPhysics(KeyValuePair<string, byte[]> file)
            {
                // Notify interface
                print($"{Func} Modify binary physics (AAMP)");

                // Check HKX2.sbphysics
                Mtk.UpdateExternal("HKX2.sbphysics", Mtk.GetConfig("dynamic") != null ? $"{Mtk.GetConfig("dynamic")}\\Data" : $"{Mtk.StaticPath}\\Data", "BotwScripts.Lib/Data");

                // Parse bphysics
                AampFile bphysics = AampFile.LoadFile(Yaz0.DecompressToStream(Mtk.GetDynamic("HKX2.sbphysics")));

                // Update physics file
                bphysics.RootNode.childParams[0].childParams[0].childParams[0].paramObjects[0].paramEntries[3].Value = new StringEntry(
                    $"{Prefix}_{Name}_{LetterId}/{FullName}.hkrb");
                bphysics.Save($"%temp%\\{FullName}\\bphysics.temp.io".ParsePathVars());

                // Add physics file
                actorpack.Files.Remove(file.Key);
                actorpack.Files.Add($"Actor/Physics/{FullName}.bphysics", File.ReadAllBytes($"%temp%\\{FullName}\\bphysics.temp.io".ParsePathVars()));

                // Return completed
                return Task.CompletedTask;
            }

            Task BXML(KeyValuePair<string, byte[]> file)
            {
                // Notify interface
                print($"{Func} Modify binary actor links (AAMP)");

                // Parse bxml
                AampFile bxml = AampFile.LoadFile(new MemoryStream(file.Value));

                // Update actor link data
                ParamEntry[] links = bxml.RootNode.paramObjects[0].paramEntries;

                foreach (ParamEntry link in links)
                {
                    switch (link.HashString)
                    {
                        case "ModelUser":
                            link.Value = new StringEntry(FullName);
                            break;
                        case "PhysicsUser":
                            link.Value = new StringEntry(FullName);
                            break;
                        case "LifeConditionUser":
                            link.Value = new StringEntry($"Landmark{lfcUnit}");
                            break;
                        case "ProfileUser":
                            link.Value = new StringEntry($"MapDynamicActive");
                            break;
                    }
                }

                bxml.Save($"%temp%\\{FullName}\\bxml.temp.io".ParsePathVars());

                // Add actor link file
                actorpack.Files.Remove(file.Key);
                actorpack.Files.Add($"Actor/ActorLink/{FullName}.bxml", File.ReadAllBytes($"%temp%\\{FullName}\\bxml.temp.io".ParsePathVars()));

                // Return completed
                return Task.CompletedTask;
            }
        }
    }
}
