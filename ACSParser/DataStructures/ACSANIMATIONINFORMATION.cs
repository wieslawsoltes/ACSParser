using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSANIMATIONINFORMATION
{
    public string AnimationName;
    public byte TransitionType;
    public string ReturnAnimation;
    public List<ACSFRAMEINFO> AnimationFrames;

    public static ACSANIMATIONINFORMATION Parse(BinaryReader reader)
    {
        ACSANIMATIONINFORMATION animationInfo = new ACSANIMATIONINFORMATION();
        animationInfo.AnimationName = reader.ReadStringNullTerminated();
        animationInfo.TransitionType = reader.ReadByte();
        animationInfo.ReturnAnimation = reader.ReadStringNullTerminated();
        animationInfo.AnimationFrames = reader.ReadListWithSize<ACSFRAMEINFO>();
        return animationInfo;
    }
}
