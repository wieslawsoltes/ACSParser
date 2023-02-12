using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct COMPRESSED
{
    public uint CompressedSize;
    public uint UncompressedSize;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public byte[] Data;

    public static COMPRESSED Parse(BinaryReader reader)
    {
        var compressed = new COMPRESSED();
        compressed.CompressedSize = reader.ReadUInt32();
        compressed.UncompressedSize = reader.ReadUInt32();
        compressed.Data = reader.ReadBytes((int)compressed.CompressedSize);
        return compressed;
    }
}
