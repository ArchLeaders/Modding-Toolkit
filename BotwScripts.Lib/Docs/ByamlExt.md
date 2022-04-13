## ByamlExt.dll Reference

**Open BYML**
> BymlFileData byaml = ByamlFile.FastLoadN(stream)<br>
> BymlFileData byaml = ByamlFile.LoadN(string)

```cs
BymlFileData byaml = ByamlFile.LoadN("path\\to\\byml.mubin");
```

<br>

**Edit BYML**
> byaml.RootNode[string]

```cs
byaml.RootNode["Objs"][0]["UnitConfigName"] = "LinkTagAnd";
```

<br>

**Write BYML**
> ByamlFile.SaveN(BymlFileData)

```cs
File.WriteAllBytes("path\\to\\edited_byml.mubin", ByamlFile.SaveN(byaml));
```