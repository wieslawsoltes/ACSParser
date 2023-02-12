using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential)]
public struct RGNDATAHEADER
{
    public uint dwSize;
    public uint iType;
    public uint nCount;
    public uint nRgnSize;
    public RECT rcBound;

    public static RGNDATAHEADER Parse(BinaryReader reader)
    {
        RGNDATAHEADER rgndataheader = new RGNDATAHEADER();
        rgndataheader.dwSize = reader.ReadUInt32();
        rgndataheader.iType = reader.ReadUInt32();
        rgndataheader.nCount = reader.ReadUInt32();
        rgndataheader.nRgnSize = reader.ReadUInt32();
        rgndataheader.rcBound = RECT.Parse(reader);
        return rgndataheader;
    }
}
