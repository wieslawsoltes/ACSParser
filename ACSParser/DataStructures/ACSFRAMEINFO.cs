namespace ACSParser.DataStructures;

public class ACSFRAMEINFO
{
    public ACSFRAMEIMAGE[] Images;
    public USHORT AudioIndex; // 2 bytes
    public USHORT Duration; // 2 bytes
    public SHORT ExitFrameIndex; // 2 bytes
    public BRANCHINFO[] Branches;
    public ACSOVERLAYINFO[] Overlays;

    // TODO: Parse not tested.
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

    public void Write(BinaryWriter writer)
    {
        for (var i = 0; i < Images.Length; i++)
        {
            Images[i].Write(writer);
        }

        writer.Write(AudioIndex);
        writer.Write(Duration);
        writer.Write(ExitFrameIndex);

        for (var i = 0; i < Branches.Length; i++)
        {
            Branches[i].Write(writer);
        }

        for (var i = 0; i < Overlays.Length; i++)
        {
            Overlays[i].Write(writer);
        }
    }
}
