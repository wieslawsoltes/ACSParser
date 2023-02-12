using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ICONIMAGE
{
    public BITMAPINFOHEADER Header;
    public byte[] BitmapData;

    public static ICONIMAGE Parse(BinaryReader reader, uint size)
    {
        ICONIMAGE iconImage = new ICONIMAGE();
        iconImage.Header = BITMAPINFOHEADER.Parse(reader);
        iconImage.BitmapData = reader.ReadBytes((int)(size - (uint)Marshal.SizeOf(iconImage.Header)));
        return iconImage;
    }
}
