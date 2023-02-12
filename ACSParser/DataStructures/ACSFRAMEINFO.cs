using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSFRAMEINFO
{
    public ACSFRAMEIMAGE[] Images;
    public ushort AudioIndex;
    public ushort Duration;
    public short ExitFrameIndex;
    public BRANCHINFO[] Branches;
    public ACSOVERLAYINFO[] Overlays;

    public static ACSFRAMEINFO Parse(BinaryReader reader)
    {
        var frame = new ACSFRAMEINFO();

        frame.Images = reader.ReadListWithSize<ACSFRAMEIMAGE>().ToArray();
        frame.AudioIndex = reader.ReadUInt16();
        frame.Duration = reader.ReadUInt16();
        frame.ExitFrameIndex = reader.ReadInt16();
        frame.Branches = reader.ReadListWithSize<BRANCHINFO>().ToArray();
        frame.Overlays = reader.ReadListWithSize<ACSOVERLAYINFO>().ToArray();

        return frame;
    }
}
