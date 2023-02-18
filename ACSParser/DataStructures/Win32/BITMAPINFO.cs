namespace ACSParser.DataStructures;

public class BITMAPINFO
{
    public BITMAPINFOHEADER Header;
    public RGBQUAD[] Colors;

    // TODO: Parse not tested.
    public static BITMAPINFO Parse(BinaryReader reader)
    {
        BITMAPINFO bitmapinfo = new BITMAPINFO();

        bitmapinfo.Header = BITMAPINFOHEADER.Parse(reader);

        // TODO:
        var numColors = bitmapinfo.Header.GetNumColors();
        bitmapinfo.Colors = new RGBQUAD[numColors];
        for (var i = 0; i < numColors; i++)
        {
            bitmapinfo.Colors[i] = RGBQUAD.Parse(reader);
        }

        return bitmapinfo;
    }

    public void Write(BinaryWriter writer)
    {
        Header.Write(writer);

        for (var i = 0; i < Colors.Length; i++)
        {
            Colors[i].Write(writer);
        }
    }
}
