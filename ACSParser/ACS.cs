using ACSParser.DataStructures;

namespace ACSParser;

public struct ACS
{
    public ACSHEADER Header; // 36 bytes
    public ACSCHARACTERINFO CharacterInfo;
    public ACSANIMATIONINFO[] AnimationInfo; // ANIMATION
    public ACSIMAGEINFO[] ImageInfo; // IMAGE
    public ACSAUDIOINFO[] AudioInfo; // AUDIO
    public ANIMATION[] Animations;
    public IMAGE[] Images;
    public AUDIO[] Audios;
}
