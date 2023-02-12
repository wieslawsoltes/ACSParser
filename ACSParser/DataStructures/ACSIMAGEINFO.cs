using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSIMAGEINFO
{
    public ACSLOCATOR InfoLocation;
    public uint Checksum;
    public byte Unknown;
    public ushort Width;
    public ushort Height;
    public bool IsCompressed;
    public DATABLOCK ImageData;
    public COMPRESSED RegionData;

    public static ACSIMAGEINFO Parse(BinaryReader reader)
    {
        ACSIMAGEINFO imageInfo = new ACSIMAGEINFO
        {
            InfoLocation = ACSLOCATOR.Parse(reader),
            Checksum = reader.ReadUInt32(),
            Unknown = reader.ReadByte(),
            Width = reader.ReadUInt16(),
            Height = reader.ReadUInt16(),
            IsCompressed = reader.ReadBoolean()
        };

        int imageDataSize = imageInfo.Width * imageInfo.Height;
        if (imageInfo.IsCompressed)
        {
            imageDataSize = ((imageInfo.Width + 3) & 0xFC) * imageInfo.Height;
        }

        imageInfo.ImageData = DATABLOCK.Parse(reader, imageDataSize);
        imageInfo.RegionData = COMPRESSED.Parse(reader);

        return imageInfo;
    }
}
