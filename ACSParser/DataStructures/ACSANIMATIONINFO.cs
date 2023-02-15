namespace ACSParser;

public struct ACSANIMATIONINFO
{
    public STRING AnimationName;
    public ACSLOCATOR AnimationLocator;

    public static ACSANIMATIONINFO Parse(BinaryReader reader)
    {
        ACSANIMATIONINFO animation = new ACSANIMATIONINFO();

        animation.AnimationName = STRING.Parse(reader);
        animation.AnimationLocator = ACSLOCATOR.Parse(reader);

        return animation;
    }
}
