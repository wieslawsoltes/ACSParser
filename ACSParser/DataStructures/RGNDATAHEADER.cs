namespace ACSParser.DataStructures;

public class RGNDATAHEADER
{
    public ULONG Size; // 4 bytes
    public ULONG Type; // 4 bytes
    public ULONG Count; // 4 bytes
    public ULONG RgnSize; // 4 bytes
    public RECT Bound; // 16 bytes

    // TODO: Parse not tested.
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

    public void Write(BinaryWriter writer)
    {
        writer.Write(Size);
        writer.Write(Type);
        writer.Write(Count);
        writer.Write(RgnSize);
        Bound.Write(writer);
    }
}
