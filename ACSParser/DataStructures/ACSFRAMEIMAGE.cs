using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSFRAMEIMAGE
{
    public uint ImageIndex;
    public short XOffset;
    public short YOffset;

    public static ACSFRAMEIMAGE Parse(BinaryReader reader)
    {
        return new ACSFRAMEIMAGE
        {
            ImageIndex = reader.ReadUInt32(),
            XOffset = reader.ReadInt16(),
            YOffset = reader.ReadInt16()
        };
    }
}
