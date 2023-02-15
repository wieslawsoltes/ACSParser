namespace ACSParser;

public struct COMPRESSED
{
    public ULONG CompressedSize;
    public ULONG UncompressedSize;
    public BYTE[] Data;

    public static COMPRESSED Parse(BinaryReader reader)
    {
        var compressed = new COMPRESSED();

        compressed.CompressedSize = reader.ULONG();
        compressed.UncompressedSize = reader.ULONG();

        if (compressed.CompressedSize == 0)
        {
            compressed.Data = reader.ReadBytes((int)compressed.UncompressedSize);
        }
        else
        {
            // TODO: Decompress. Data size is compressed.UncompressedSize after decompression.
            compressed.Data = reader.ReadBytes((int)compressed.CompressedSize);
        }

        return compressed;
    }
}
