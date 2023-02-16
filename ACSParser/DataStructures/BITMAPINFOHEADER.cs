namespace ACSParser.DataStructures;

public struct BITMAPINFOHEADER
{
    public ULONG Size;
    public LONG Width;
    public LONG Height;
    public USHORT Planes;
    public USHORT BitCount;
    public ULONG Compression;
    public ULONG SizeImage;
    public LONG XPelsPerMeter;
    public LONG YPelsPerMeter;
    public ULONG ClrUsed;
    public ULONG ClrImportant;

    public static BITMAPINFOHEADER Parse(BinaryReader reader)
    {
        BITMAPINFOHEADER header = new BITMAPINFOHEADER();

        header.Size = reader.ULONG();
        header.Width = reader.LONG();
        header.Height = reader.LONG();
        header.Planes = reader.USHORT();
        header.BitCount = reader.USHORT();
        header.Compression = reader.ULONG();
        header.SizeImage = reader.ULONG();
        header.XPelsPerMeter = reader.LONG();
        header.YPelsPerMeter = reader.LONG();
        header.ClrUsed = reader.ULONG();
        header.ClrImportant = reader.ULONG();

        return header;
    }
}
