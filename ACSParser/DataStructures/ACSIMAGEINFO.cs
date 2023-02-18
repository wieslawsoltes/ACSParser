namespace ACSParser.DataStructures;

public class ACSIMAGEINFO // 12 bytes
{
    public ACSLOCATOR InfoLocation; // 8 bytes
    public ULONG Checksum; // 4 bytes

    // TODO: Parse not tested.
    public static ACSIMAGEINFO Parse(BinaryReader reader)
    {
        ACSIMAGEINFO imageInfo = new ACSIMAGEINFO();

        imageInfo.InfoLocation = ACSLOCATOR.Parse(reader);
        imageInfo.Checksum = reader.ULONG();

        return imageInfo;
    }

    public void Write(BinaryWriter writer)
    {
        InfoLocation.Write(writer);
        writer.Write(Checksum);
    }
}

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
}
