namespace ACSParser.DataStructures;

public struct TRAYICON
{
    public ULONG MonochromeBitmapSize;
    public ICONIMAGE MonochromeBitmap;
    public ULONG ColorBitmapSize;
    public ICONIMAGE ColorBitmap;

    public static TRAYICON Parse(BinaryReader reader)
    {
        TRAYICON trayIcon = new TRAYICON();

        trayIcon.MonochromeBitmapSize = reader.ULONG();
        trayIcon.MonochromeBitmap = ICONIMAGE.Parse(reader);
        // TODO: reader.ReadBytes((int)trayIcon.MonochromeBitmapSize);

        trayIcon.ColorBitmapSize = reader.ULONG();
        trayIcon.ColorBitmap = ICONIMAGE.Parse(reader);
        // TODO: reader.ReadBytes((int)trayIcon.ColorBitmapSize);

        return trayIcon;
    }
}
