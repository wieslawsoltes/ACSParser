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

            // Decompress the data using the Decompressor
            try
            {
                compressed.Data = Decompressor.Decompress(compressedData, (int)compressed.UncompressedSize);
                // Mark as no longer compressed by setting compressed size to 0
                compressed.CompressedSize = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to decompress COMPRESSED data: {ex.Message}");
                // Keep original compressed data as fallback
                compressed.Data = compressedData;
            }
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
