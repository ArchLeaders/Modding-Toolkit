## SARCExt.dll Reference

**Open SARC**
> SarcData sarc = SARC.UnpackRamN(byte[]);

```cs
SarcData sarc = SARC.UnpackRamN(File.ReadAllBytes("path\\to\\sarc.pack"));
```

<br>

**Add File**
> sarc.Files.Add(string, byte[])

```cs
// adds an empty file named filename.txt
// to the root of the open SARC file
sarc.Files.Add("filename.txt", new byte[0]);
```

<br>

**Remove File(s)**
> sarc.Files.Remove(string)

```cs
// Removes a file named filename.txt
// from the root of the SARC file
sarc.Files.Remove("filename.txt")
```

<br>

**Write SarcData**
> Tuple<int, byte[]> SARC.PackN(SarcData)

```cs
File.WriteAllBytes("path\\to\\edited_sarc.pack", SARC.PackN(sarc).Item2);
```