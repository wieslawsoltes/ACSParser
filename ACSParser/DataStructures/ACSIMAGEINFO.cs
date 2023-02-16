namespace ACSParser.DataStructures;

public struct ACSIMAGEINFO
{
    public ACSLOCATOR InfoLocation; // 8 bytes
    public ULONG Checksum; // 4 bytes

    // TODO: Parse not tested.
    public static ACSIMAGEINFO Parse(BinaryReader reader)
    {
        ACSIMAGEINFO imageInfo = new ACSIMAGEINFO();

        imageInfo.InfoLocation = ACSLOCATOR.Parse(reader);
        imageInfo.Checksum = reader.ULONG();

        // TODO: Read IMAGE from InfoLocation.
        
        return imageInfo;
    }
}

public struct IMAGE
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
        image.RegionData = COMPRESSED.Parse(reader);

        return image;
    }
}
