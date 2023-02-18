namespace ACSParser.DataStructures;

public class BRANCHINFO
{
    public USHORT FrameIndex; // 2 bytes
    public USHORT Probability; // 2 bytes

    // TODO: Parse not tested.
    public static BRANCHINFO Parse(BinaryReader reader)
    {
        BRANCHINFO branch = new BRANCHINFO();

        branch.FrameIndex = reader.USHORT();
        branch.Probability = reader.USHORT();

        return branch;
    }
}
