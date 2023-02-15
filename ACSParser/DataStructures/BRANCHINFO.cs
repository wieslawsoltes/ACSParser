namespace ACSParser;

public struct BRANCHINFO
{
    public USHORT FrameIndex;
    public USHORT Probability;

    public static BRANCHINFO Parse(BinaryReader reader)
    {
        BRANCHINFO branch = new BRANCHINFO();

        branch.FrameIndex = reader.USHORT();
        branch.Probability = reader.USHORT();

        return branch;
    }
}
