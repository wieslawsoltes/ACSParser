namespace ACSParser.DataStructures;

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

    public void Write(BinaryWriter writer)
    {
        AnimationName.Write(writer);
        writer.Write(TransitionType);
        ReturnAnimation.Write(writer);

        writer.Write((USHORT)AnimationFrames.Length);
        for (var i = 0; i < AnimationFrames.Length; i++)
        {
            AnimationFrames[i].Write(writer);
        }
    }
}
