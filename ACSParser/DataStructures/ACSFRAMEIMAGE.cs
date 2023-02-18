namespace ACSParser.DataStructures;

public class ACSFRAMEIMAGE // 8 bytes
{
    public ULONG ImageIndex; // 4 bytes
    public SHORT XOffset; // 2 bytes
    public SHORT YOffset; // 2 bytes

    // TODO: Parse not tested.
    public static ACSFRAMEIMAGE Parse(BinaryReader reader)
    {
        ACSFRAMEIMAGE frameImage = new ACSFRAMEIMAGE();

        frameImage.ImageIndex = reader.ULONG();
        frameImage.XOffset = reader.SHORT();
        frameImage.YOffset = reader.SHORT();

        return frameImage;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(ImageIndex);
        writer.Write(XOffset);
        writer.Write(YOffset);
    }
}
