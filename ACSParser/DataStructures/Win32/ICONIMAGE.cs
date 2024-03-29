namespace ACSParser.DataStructures;

public class ICONIMAGE
{
    public BITMAPINFOHEADER IconHeader;
    public RGBQUAD[] ColorTable;
    public BYTE[] XORMaskBits;
    public BYTE[] ANDMaskBits;

    // TODO: Parse not tested.
    public static ICONIMAGE Parse(BinaryReader reader)
    {
        ICONIMAGE iconImage = new ICONIMAGE();

        iconImage.IconHeader = BITMAPINFOHEADER.Parse(reader);

        // TODO:
        var numColors = iconImage.IconHeader.GetNumColors();
        iconImage.ColorTable = new RGBQUAD[numColors];
        for (var i = 0; i < numColors; i++)
        {
            iconImage.ColorTable[i] = RGBQUAD.Parse(reader);
        }

        // TODO:
        var xorMaskSize = GetXORMaskSize(ref iconImage.IconHeader);
        iconImage.XORMaskBits = reader.ReadBytes(xorMaskSize);

        // TODO:
        var andMaskSize = GetANDMaskSize(ref iconImage.IconHeader);
        iconImage.ANDMaskBits = reader.ReadBytes(andMaskSize);

        return iconImage;
    }

    private static int GetXORMaskSize(ref BITMAPINFOHEADER iconHeader)
    {
        int xorMaskSize;

        if (iconHeader.BitCount > 8)
        {
            xorMaskSize = iconHeader.Width * iconHeader.Height * 4;
        }
        else
        {
            xorMaskSize = iconHeader.Width * iconHeader.Height * iconHeader.BitCount / 8;
        }

        return xorMaskSize;
    }

    private static int GetANDMaskSize(ref BITMAPINFOHEADER iconHeader)
    {
        var andMaskSize = iconHeader.Width * iconHeader.Height / 8;
        andMaskSize = (andMaskSize + 3) & ~3;
        return andMaskSize;
    }

    public void Write(BinaryWriter writer)
    {
        IconHeader.Write(writer);

        for (var i = 0; i < ColorTable.Length; i++)
        {
            ColorTable[i].Write(writer);
        }

        for (var i = 0; i < XORMaskBits.Length; i++)
        {
            writer.Write(XORMaskBits[i]);
        }

        for (var i = 0; i < ANDMaskBits.Length; i++)
        {
            writer.Write(ANDMaskBits[i]);
        }
    }
}
