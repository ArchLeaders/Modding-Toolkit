using ByamlExt.Byaml;
using SARCExt;
using BotwScripts.Lib.Formats;
using AampLibraryCSharp;
using BfresLibrary;

///
///  Read/write BYML and Yaz0
///

// Decompress with Yaz0 and open BYML
BymlFileData byaml = ByamlFile.FastLoadN(Yaz0.DecompressToStream("D:\\byaml.sbyml"));

// Print data
// Console.WriteLine(byaml.RootNode["Objs"][0]["UnitConfigName"]);

// Update data
byaml.RootNode["Objs"][0]["UnitConfigName"] = "LinkTagAnd";

// Write BYML and Yaz0 compress byte[]
File.WriteAllBytes("D:\\edit.sbyml", Yaz0.Compress(ByamlFile.SaveN(byaml), 7));

/// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

///
/// Read/write SARC
///

// open SARC file
SarcData sarc = SARC.UnpackRamN(File.ReadAllBytes("D:\\Bootup.pack"));

// Print SARC files
// foreach (var file in sarc.Files)
//    Console.WriteLine(file.Key);

// Add file
sarc.Files.Add("edit.sbyml", File.ReadAllBytes("D:\\edit.sbyml"));

// Write modified SARC file
File.WriteAllBytes("D:\\edit.pack", SARC.PackN(sarc).Item2);

/// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

///
/// Read/write AAMP
///

// Open aamp file
AampFile aamp = AampFile.LoadFile("D:\\system.bchmres");

// Modify nested value
aamp.RootNode.childParams[0].paramObjects[0].paramEntries[0].Value = new StringEntry("new_value");

// Write modified file
aamp.Save("D:\\newfile.bchmres");

/// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

///
/// Read/write BFRES
///

ResFile bfres = Bfres.LoadBfres("D:\\tests.sbfres");

bfres.Name = "test";
bfres.Save("D:\\test.bfres");