namespace ACSParser.DataStructures;

public class BITMAPFILEHEADER
{
    public USHORT Type;
    public ULONG Size;
    public USHORT Reserved1;
    public USHORT Reserved2;
    public ULONG OffBits;

    // TODO: Parse not tested.
    public static BITMAPFILEHEADER Parse(BinaryReader reader)
    {
        BITMAPFILEHEADER header = new BITMAPFILEHEADER();

        header.Type = reader.USHORT();
        header.Size = reader.ULONG();
        header.Reserved1 = reader.USHORT();
        header.Reserved2 = reader.USHORT();
        header.OffBits = reader.ULONG();

        return header;
    }
}
