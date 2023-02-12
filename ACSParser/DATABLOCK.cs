using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DATABLOCK
{
    public byte[] Data;

    public static DATABLOCK Parse(BinaryReader reader)
    {
        DATABLOCK datablock = new DATABLOCK();

        int size = reader.ReadInt32();
        datablock.Data = reader.ReadBytes(size);

        return datablock;
    }

    public static DATABLOCK Parse(BinaryReader reader, int size)
    {
        DATABLOCK datablock = new DATABLOCK();

        datablock.Data = reader.ReadBytes(size);

        return datablock;
    }
}
