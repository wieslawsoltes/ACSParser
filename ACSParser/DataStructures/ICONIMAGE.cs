using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ICONIMAGE
{
    public BITMAPINFOHEADER IconHeader;
    public RGBQUAD[] ColorTable;
    public byte[] XORMaskBits;
    public byte[] ANDMaskBits;

    public static ICONIMAGE Parse(BinaryReader reader)
    {
        ICONIMAGE iconImage = new ICONIMAGE();
        iconImage.IconHeader = BITMAPINFOHEADER.Parse(reader);

        // Read the color table
        int colorTableSize = 0;
        if (iconImage.IconHeader.BitCount <= 8)
        {
            colorTableSize = (1 << iconImage.IconHeader.BitCount) * 4;
            iconImage.ColorTable = new RGBQUAD[1 << iconImage.IconHeader.BitCount];
            for (int i = 0; i < (1 << iconImage.IconHeader.BitCount); i++)
            {
                iconImage.ColorTable[i] = RGBQUAD.Parse(reader);
            }
        }

        // Read the XOR mask bits
        int xorMaskSize = (iconImage.IconHeader.Width * iconImage.IconHeader.Height *
            iconImage.IconHeader.BitCount + 7) / 8;
        iconImage.XORMaskBits = reader.ReadBytes(xorMaskSize);

        // Read the AND mask bits
        int andMaskSize = (iconImage.IconHeader.Width * iconImage.IconHeader.Height + 7) / 8;
        iconImage.ANDMaskBits = reader.ReadBytes(andMaskSize);

        return iconImage;
    }
    
     public static ICONIMAGE Parse(BinaryReader reader, uint size)
    {
        ICONIMAGE iconImage = new ICONIMAGE();

        // Read the Icon Header
        iconImage.IconHeader = BITMAPINFOHEADER.Parse(reader);

        // Read the Color Table
        int numColors = GetNumColors(iconImage.IconHeader);
        iconImage.ColorTable = new RGBQUAD[numColors];
        for (int i = 0; i < numColors; i++)
        {
            iconImage.ColorTable[i] = RGBQUAD.Parse(reader);
        }

        // Read the XOR Mask Bits
        int xorMaskSize = GetXORMaskSize(iconImage.IconHeader, numColors);
        iconImage.XORMaskBits = reader.ReadBytes(xorMaskSize);

        // Read the AND Mask Bits
        int andMaskSize = GetANDMaskSize(iconImage.IconHeader, numColors);
        iconImage.ANDMaskBits = reader.ReadBytes(andMaskSize);

        return iconImage;
    }

    private static int GetNumColors(BITMAPINFOHEADER iconHeader)
    {
        int numColors = 0;

        if (iconHeader.BitCount <= 8)
        {
            numColors = (1 << iconHeader.BitCount);
        }

        return numColors;
    }

    private static int GetXORMaskSize(BITMAPINFOHEADER iconHeader, int numColors)
    {
        int xorMaskSize = 0;

        if (iconHeader.BitCount > 8)
        {
            // Each pixel is 32 bits (ARGB)
            xorMaskSize = iconHeader.Width * iconHeader.Height * 4;
        }
        else
        {
            // Each pixel is indexed to the color table
            xorMaskSize = iconHeader.Width * iconHeader.Height * iconHeader.BitCount / 8;
        }

        return xorMaskSize;
    }

    private static int GetANDMaskSize(BITMAPINFOHEADER iconHeader, int numColors)
    {
        // Each bit in the AND mask represents a pixel in the XOR mask
        // 8 pixels fit in a byte
        int andMaskSize = (iconHeader.Width * iconHeader.Height) / 8;

        // Make sure the AND mask is DWORD aligned
        andMaskSize = (andMaskSize + 3) & ~3;

        return andMaskSize;
    }
}
