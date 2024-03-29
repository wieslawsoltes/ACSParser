namespace ACSParser.DataStructures;

public class TRAYICON
{
    public ULONG MonochromeBitmapSize; // 4 bytes
    public ICONIMAGE MonochromeBitmap;
    public ULONG ColorBitmapSize; // 4 bytes
    public ICONIMAGE ColorBitmap;

    // TODO: Parse not tested.
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

    public void Write(BinaryWriter writer)
    {
        writer.Write(MonochromeBitmapSize);
        MonochromeBitmap.Write(writer);
        writer.Write(ColorBitmapSize);
        ColorBitmap.Write(writer);
    }
}
