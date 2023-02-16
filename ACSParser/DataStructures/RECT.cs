namespace ACSParser.DataStructures;

public struct RECT
{
    public LONG Left;
    public LONG Top;
    public LONG Right;
    public LONG Bottom;

    public static RECT Parse(BinaryReader reader)
    {
        RECT rect = new RECT();

        rect.Left = reader.LONG();
        rect.Top = reader.LONG();
        rect.Right = reader.LONG();
        rect.Bottom = reader.LONG();

        return rect;
    }
}
