using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SAMPLE
{
    public uint Value1;
    public ushort Value2;
    public ushort Value3;
    public byte Value4;
    public byte Value5;
    public byte Value6;
    public byte Value7;

    public static SAMPLE Parse(BinaryReader reader)
    {
        var sample = new SAMPLE();

        sample.Value1 = reader.ReadUInt32();
        sample.Value2 = reader.ReadUInt16();
        sample.Value3 = reader.ReadUInt16();
        sample.Value4 = reader.ReadByte();
        sample.Value5 = reader.ReadByte();
        sample.Value6 = reader.ReadByte();
        sample.Value7 = reader.ReadByte();

        return sample;
    }
}
