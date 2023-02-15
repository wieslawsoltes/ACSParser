namespace ACSParser;

public struct ACSIMAGEINFO
{
    public ACSLOCATOR InfoLocation;
    public ULONG Checksum;

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
    public BYTE Unknown;
    public USHORT Width;
    public USHORT Height;
    public BOOL IsImageDataCompressed;
    public DATABLOCK ImageData;
    public COMPRESSED RegionData;

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
