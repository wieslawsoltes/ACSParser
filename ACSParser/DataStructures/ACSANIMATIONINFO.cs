namespace ACSParser.DataStructures;

public class ACSANIMATIONINFO
{
    public STRING AnimationName;
    public ACSLOCATOR AnimationLocator; // 8 bytes

    // TODO: Parse not tested.
    public static ACSANIMATIONINFO Parse(BinaryReader reader)
    {
        ACSANIMATIONINFO animation = new ACSANIMATIONINFO();

        animation.AnimationName = STRING.Parse(reader);
        animation.AnimationLocator = ACSLOCATOR.Parse(reader);

        return animation;
    }

    public void Write(BinaryWriter writer)
    {
        AnimationName.Write(writer);
        AnimationLocator.Write(writer);
    }
}
