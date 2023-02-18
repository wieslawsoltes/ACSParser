namespace ACSParser.DataStructures;

public class IMAGE
{
    public BYTE Unknown; // 1 byte
    public USHORT Width; // 2 bytes
    public USHORT Height; // 2 bytes
    public BOOL IsImageDataCompressed; // 1 byte
    public DATABLOCK ImageData;
    public COMPRESSED RegionData;

    // TODO: Parse not tested.
    public static IMAGE Parse(BinaryReader reader)
    {
        IMAGE image = new IMAGE();

        image.Unknown = reader.BYTE();
        image.Width = reader.USHORT();
        image.Height = reader.USHORT();

        image.IsImageDataCompressed = reader.BOOL();
        image.ImageData = DATABLOCK.Parse(reader);

        if (image.IsImageDataCompressed != 0)
        {
            // TODO: ImageData is compressed.
            // var decompressedDataSize =  ((image.Width + 3) & 0xFC) * image.Height;
            // var decompressed = Decompressor.Decompress(image.ImageData.Data, decompressedDataSize);
        }

        image.RegionData = COMPRESSED.Parse(reader);

        return image;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(Unknown);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(IsImageDataCompressed);
        ImageData.Write(writer);
        RegionData.Write(writer);
    }

    public void SaveBitmap(Stream stream, PALETTECOLOR[] colorTable)
    {
        var decompressedDataSize = ((Width + 3) & 0xFC) * Height;
        var decompressed = Decompressor.Decompress(ImageData.Data, decompressedDataSize);

        using var writer = new BinaryWriter(stream);

        // OffBits = BITMAPFILEHEADER + BITMAPINFOHEADER + Color Table
        ULONG offBits = 14 + 40 + (ULONG)colorTable.Length;
        ULONG size = offBits + (ULONG)decompressed.Length;

        // BMP header

        var fileHeader = new BITMAPFILEHEADER
        {
            Type = 0x4D42,
            Size = size, // TODO:
            Reserved1 = 0,
            Reserved2 = 0,
            OffBits = offBits, // TODO:
        };
        fileHeader.Write(writer);

        // BMP info header

        var infoHeader = new BITMAPINFOHEADER
        {
            Size = 40,
            Width = Width,
            Height = Height,
            Planes = 1,
            BitCount = 8,
            Compression = (ULONG)Compression.BI_RGB,
            SizeImage = 0, // For BI_RGB it can be zero.
            XPelsPerMeter = 0, // TODO:
            YPelsPerMeter = 0, // TODO:
            ClrUsed = 0,
            ClrImportant = 0
        };
        infoHeader.Write(writer);

        // BMP color table

        foreach (var paletteColor in colorTable)
        {
            paletteColor.Color.Write(writer);
        }

        // BMP bytes

        writer.Write(decompressed);
    }
}
