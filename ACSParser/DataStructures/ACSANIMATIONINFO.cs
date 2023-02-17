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
}

public class ANIMATION
{
    public STRING AnimationName;
    public BYTE TransitionType; // 1 byte
    public STRING ReturnAnimation;
    public ACSFRAMEINFO[] AnimationFrames;

    // TODO: Parse not tested.
    public static ANIMATION Parse(BinaryReader reader)
    {
        ANIMATION animation = new ANIMATION();

        animation.AnimationName = STRING.Parse(reader);
        animation.TransitionType = reader.BYTE();
        animation.AnimationName = STRING.Parse(reader);

        USHORT animationFramesCount = reader.USHORT();
        animation.AnimationFrames = new ACSFRAMEINFO[animationFramesCount];
        for (var i = 0; i < animationFramesCount; i++)
        {
            animation.AnimationFrames[i] = ACSFRAMEINFO.Parse(reader);
        }

        return animation;
    }
}
