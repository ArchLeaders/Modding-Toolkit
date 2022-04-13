## AampLibraryCSharp.dll Reference

**Open AAMP**
> AampFile aamp = AampFile.LoadFile(string);

```cs
AampFile aamp = AampFile.LoadFile("path\\to\\aamp.bchmres");
```

<br>

**Edit AAMP**
> AampFile.RootNode.query[int]

```yml
!io
version: 0
type: xml
param_root: !list # RootNode
  objects: {}     
  lists:          # RootNode.childParams
    world: !list    # [0]
      objects:      # paramObjects
        '0': !obj   # [0]
                    # paramEntries -->
          3208210256: !str64 game_test # [0]
  [...]
```

```cs
// set Value as a new Aamp.StringValue
aamp.RootNode.childParams[0].paramObjects[0].paramEntries[0].Value = new StringEntry("new_value");
```

<br>

**Write AAMP**
> AampFile.Save(string);

```cs
aamp.Save("path\\to\\edited_aamp.bchmres")
```