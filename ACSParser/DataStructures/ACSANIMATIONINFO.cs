using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSANIMATIONINFO
{
    public string AnimationName;
    public ACSLOCATOR AnimationLocator;

    public static ACSANIMATIONINFO Parse(BinaryReader reader)
    {
        ACSANIMATIONINFO animation = new ACSANIMATIONINFO();
        animation.AnimationName = reader.ReadStringNullTerminated();
        animation.AnimationLocator = ACSLOCATOR.Parse(reader);
        return animation;
    }
}
