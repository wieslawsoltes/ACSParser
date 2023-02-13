using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential)]
public struct RGNDATA
{
    public RGNDATAHEADER Header;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public RECT[] Rectangles;

    public static RGNDATA Parse(BinaryReader reader)
    {
        RGNDATA data = new RGNDATA();

        data.Header = reader.ReadStruct<RGNDATAHEADER>();
        data.Rectangles = reader.ReadStructs<RECT>((int)data.Header.nCount);

        return data;
    }
}
