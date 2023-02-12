using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BRANCHINFO
{
    public ushort FrameIndex;
    public ushort Probability;

    public static BRANCHINFO Parse(BinaryReader reader)
    {
        BRANCHINFO branch = new BRANCHINFO();

        branch.FrameIndex = reader.ReadUInt16();
        branch.Probability = reader.ReadUInt16();

        return branch;
    }
}
