namespace ACSParser.DataStructures;

public struct COMPRESSED
{
    public ULONG CompressedSize; // 4 bytes
    public ULONG UncompressedSize; // 4 bytes
    public BYTE[] Data; // Data.Length * 1 byte

    // TODO: Parse not tested.
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
