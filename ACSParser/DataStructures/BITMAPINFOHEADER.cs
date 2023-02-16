namespace ACSParser.DataStructures;

public struct BITMAPINFOHEADER
{
    public ULONG Size; // 4 bytes
    public LONG Width; // 4 bytes
    public LONG Height; // 4 bytes
    public USHORT Planes; // 2 bytes
    public USHORT BitCount; // 2 bytes
    public ULONG Compression; // 4 bytes
    public ULONG SizeImage; // 4 bytes
    public LONG XPelsPerMeter; // 4 bytes
    public LONG YPelsPerMeter; // 4 bytes
    public ULONG ClrUsed; // 4 bytes
    public ULONG ClrImportant; // 4 bytes

    // TODO: Parse not tested.
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
