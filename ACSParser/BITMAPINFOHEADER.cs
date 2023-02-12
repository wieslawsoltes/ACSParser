using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class BITMAPINFOHEADER
{
    public uint Size;
    public int Width;
    public int Height;
    public ushort Planes;
    public ushort BitCount;
    public uint Compression;
    public uint SizeImage;
    public int XPelsPerMeter;
    public int YPelsPerMeter;
    public uint ClrUsed;
    public uint ClrImportant;

    public static BITMAPINFOHEADER Parse(BinaryReader reader)
    {
        BITMAPINFOHEADER header = new BITMAPINFOHEADER();
        header.Size = reader.ReadUInt32();
        header.Width = reader.ReadInt32();
        header.Height = reader.ReadInt32();
        header.Planes = reader.ReadUInt16();
        header.BitCount = reader.ReadUInt16();
        header.Compression = reader.ReadUInt32();
        header.SizeImage = reader.ReadUInt32();
        header.XPelsPerMeter = reader.ReadInt32();
        header.YPelsPerMeter = reader.ReadInt32();
        header.ClrUsed = reader.ReadUInt32();
        header.ClrImportant = reader.ReadUInt32();
        return header;
    }
}
