using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSLOCATOR
{
    public uint Offset;
    public uint Size;

    public static ACSLOCATOR Parse(BinaryReader reader)
    {
        ACSLOCATOR locator = new ACSLOCATOR();

        locator.Offset = reader.ReadUInt32();
        locator.Size = reader.ReadUInt32();

        return locator;
    }
}
