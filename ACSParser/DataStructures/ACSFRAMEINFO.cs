namespace ACSParser;

public struct ACSFRAMEINFO
{
    public ACSFRAMEIMAGE[] Images;
    public USHORT AudioIndex;
    public USHORT Duration;
    public SHORT ExitFrameIndex;
    public BRANCHINFO[] Branches;
    public ACSOVERLAYINFO[] Overlays;

    public static ACSFRAMEINFO Parse(BinaryReader reader)
    {
        var frame = new ACSFRAMEINFO();

        USHORT frameImageCount = reader.USHORT();
        frame.Images = new ACSFRAMEIMAGE[frameImageCount];
        for (var i = 0; i < frameImageCount; i++)
        {
            frame.Images[i] = ACSFRAMEIMAGE.Parse(reader);
        }   

        frame.AudioIndex = reader.USHORT();
        frame.Duration = reader.USHORT();
        frame.ExitFrameIndex = reader.SHORT();

        BYTE branchInfoCount = reader.BYTE();
        frame.Branches = new BRANCHINFO[branchInfoCount];
        for (var i = 0; i < branchInfoCount; i++)
        {
            frame.Branches[i] = BRANCHINFO.Parse(reader);
        }   

        BYTE overlayInfoCount = reader.BYTE();
        frame.Overlays = new ACSOVERLAYINFO[overlayInfoCount];
        for (var i = 0; i < overlayInfoCount; i++)
        {
            frame.Overlays[i] = ACSOVERLAYINFO.Parse(reader);
        }      

        return frame;
    }
}
