namespace ACSParser;

public struct RGNDATAHEADER
{
    public ULONG Size;
    public ULONG Type;
    public ULONG Count;
    public ULONG RgnSize;
    public RECT Bound;

    public static RGNDATAHEADER Parse(BinaryReader reader)
    {
        RGNDATAHEADER rgndataheader = new RGNDATAHEADER();

        rgndataheader.Size = reader.ULONG();
        rgndataheader.Type = reader.ULONG();
        rgndataheader.Count = reader.ULONG();
        rgndataheader.RgnSize = reader.ULONG();
        rgndataheader.Bound = RECT.Parse(reader);

        return rgndataheader;
    }
}
