using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public static RECT Parse(BinaryReader reader)
    {
        RECT rect = new RECT();

        rect.Left = reader.ReadInt32();
        rect.Top = reader.ReadInt32();
        rect.Right = reader.ReadInt32();
        rect.Bottom = reader.ReadInt32();

        return rect;
    }
}
