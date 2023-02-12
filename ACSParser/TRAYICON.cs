using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class TRAYICON
{
    public uint MonochromeBitmapSize;
    public ICONIMAGE MonochromeBitmap;
    public uint ColorBitmapSize;
    public ICONIMAGE ColorBitmap;

    public static TRAYICON Parse(BinaryReader reader)
    {
        TRAYICON trayIcon = new TRAYICON();
        trayIcon.MonochromeBitmapSize = reader.ReadUInt32();
        trayIcon.MonochromeBitmap = ICONIMAGE.Parse(reader, trayIcon.MonochromeBitmapSize);
        trayIcon.ColorBitmapSize = reader.ReadUInt32();
        trayIcon.ColorBitmap = ICONIMAGE.Parse(reader, trayIcon.ColorBitmapSize);
        return trayIcon;
    }
}
