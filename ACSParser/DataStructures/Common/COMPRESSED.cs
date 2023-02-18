namespace ACSParser.DataStructures;

public class COMPRESSED
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
            var compressedData = reader.ReadBytes((int)compressed.CompressedSize);

            // TODO: Decompress. Data size is compressed.UncompressedSize after decompression.
            compressed.Data = compressedData;
        }

        return compressed;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(CompressedSize);
        writer.Write(UncompressedSize);

        for (var i = 0; i < Data.Length; i++)
        {
            writer.Write(Data[i]);
        }
    }
}
