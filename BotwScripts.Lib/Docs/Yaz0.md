## Yaz0.cs Reference

**Compress**
> Yaz0.Compress(byte[])<br>
> Yaz0.Compress(string)

```cs
byte[] bytes = Yaz0.Compress("path\\to\\non-yaz0.byml")
await File.WriteAllBytesAsync("path\\to\\yaz0.sbyml", bytes)
```

<br>

**Decompress**
> Yaz0.Decompress(byte[])<br>
> Yaz0.Decompress(string)

```cs
byte[] bytes = Yaz0.Compress("path\\to\\yaz0.sbyml")
await File.WriteAllBytesAsync("path\\to\\non-yaz0.byml", bytes)
```