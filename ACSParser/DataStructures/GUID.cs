using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GUID
{
    public uint Data1;
    public ushort Data2;
    public ushort Data3;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Data4;

    public static GUID Parse(BinaryReader reader)
    {
        GUID guid = new GUID();

        guid.Data1 = reader.ReadUInt32();
        guid.Data2 = reader.ReadUInt16();
        guid.Data3 = reader.ReadUInt16();
        guid.Data4 = reader.ReadBytes(8);

        return guid;
    }
}
