namespace ACSParser.DataStructures;

public class RECT
{
    public LONG Left; // 4 bytes
    public LONG Top; // 4 bytes
    public LONG Right; // 4 bytes
    public LONG Bottom; // 4 bytes

    // TODO: Parse not tested.
    public static RECT Parse(BinaryReader reader)
    {
        RECT rect = new RECT();

        rect.Left = reader.LONG();
        rect.Top = reader.LONG();
        rect.Right = reader.LONG();
        rect.Bottom = reader.LONG();

        return rect;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(Left);
        writer.Write(Top);
        writer.Write(Right);
        writer.Write(Bottom);
    }
}
