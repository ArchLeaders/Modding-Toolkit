## BfresLibrary.dll Reference

**Open BFRES**
> ResFile bfres = Bfres.LoadBfres(string);

```cs
// Yazo compressed BFRES files will be
// automatically decompressed with LoadBfres()
ResFile bfres = Bfres.LoadBfres("path\\to\\caferes.bfres");
```

<br>

**Rename BFRES**
> ResFile.Name = string

```cs
bfres.Name = "edited_caferes";
```

<br>

**Write BFRES**
> ResFile.Save(string)

```cs
bfres.Save("path\\to\\edited_caferes.bfres");
```