using ACSParser.DataStructures;

namespace ACSParser;

public class ACS
{
    public ACSHEADER Header; // 36 bytes
    public ACSCHARACTERINFO CharacterInfo;
    public ACSANIMATIONINFO[] AnimationInfo; // ANIMATION
    public ACSIMAGEINFO[] ImageInfo; // IMAGE
    public ACSAUDIOINFO[] AudioInfo; // AUDIO
    public ANIMATION[] Animations;
    public IMAGE[] Images;
    public AUDIO[] Audios;

    public static ACS Parse(Stream stream)
    {
        ACS acs = new ACS();

        using var reader = new BinaryReader(stream);

        //
        // ACSHEADER
        //
        // Console.WriteLine($"Position: {stream.Position}");
        var header = ACSHEADER.Parse(reader);
        acs.Header = header;
        // Console.WriteLine($"Position: {stream.Position}");
        // Console.WriteLine("ACS File Header:");
        // Console.WriteLine("Signature: 0x{0:X8}", header.Signature);

        //
        // Debug: ACSCHARACTERINFO
        //
        // Console.WriteLine($"Position: {stream.Position}");
        // Console.WriteLine($"Character Info");
        // var characterInfoStyles =Util. CharacterInfoStyles();
        // Util.PrintMemory(stream, reader, header.CharacterInfo.Offset, 41, characterInfoStyles);
        // Console.WriteLine("");
        // Console.WriteLine($"Position: {stream.Position}");
        
        //
        // Debug: ACSCHARACTERINFO
        //
        // Console.WriteLine($"Position: {stream.Position}");
        // Console.WriteLine($"Character Info Location: {header.CharacterInfo.Offset:X8} (size {header.CharacterInfo.Size} bytes)");
        // Util.PrintMemory(stream, reader, header.CharacterInfo.Offset, 160, characterInfoStyles);
        // Util.PrintMemory(stream, reader, header.CharacterInfo, characterInfoStyles);
        // Console.WriteLine("");

        //
        // Read: ACSCHARACTERINFO
        //
        // Console.WriteLine($"Position: {stream.Position}");
        stream.Seek(header.CharacterInfo.Offset, SeekOrigin.Begin);
        // Console.WriteLine($"Position: {stream.Position}");
        var characterInfo = ACSCHARACTERINFO.Parse(reader);
        acs.CharacterInfo = characterInfo;
        // Console.WriteLine($"Position: {stream.Position}");

        //
        // Debug: ACSANIMATIONINFO[]
        //
        // Console.WriteLine($"Position: {stream.Position}");
        // Console.WriteLine($"Animation Info List Location: {header.AnimationInfoList.Offset:X8} (size {header.AnimationInfoList.Size} bytes)");
        //Util.PrintMemory(stream, reader, header.AnimationInfoList);
        // Console.WriteLine("");
        // Console.WriteLine($"Position: {stream.Position}");

        //
        // Read: ACSANIMATIONINFO[]
        //
        stream.Seek(header.AnimationInfoList.Offset, SeekOrigin.Begin);
        ULONG animationInfoCount = reader.ULONG();
        acs.AnimationInfo = new ACSANIMATIONINFO[animationInfoCount];
        for (var i = 0; i < animationInfoCount; i++)
        {
            acs.AnimationInfo[i] = ACSANIMATIONINFO.Parse(reader);
        }

        acs.Animations = new ANIMATION[animationInfoCount];
        for (var i = 0; i < acs.AnimationInfo.Length; i++)
        {
            stream.Seek(acs.AnimationInfo[i].AnimationLocator.Offset, SeekOrigin.Begin);
            acs.Animations[i] = ANIMATION.Parse(reader);
        }

        //
        // Debug: ACSIMAGEINFO[]
        //
        // Console.WriteLine($"Position: {stream.Position}");
        // Console.WriteLine($"Image Info List Location: {header.ImageInfoList.Offset:X8} (size {header.ImageInfoList.Size} bytes)");
        // Util.PrintMemory(stream, reader, header.ImageInfoList);
        // Console.WriteLine("");
        // Console.WriteLine($"Position: {stream.Position}");

        //
        // Read: ACSIMAGEINFO[]
        //
        stream.Seek(header.ImageInfoList.Offset, SeekOrigin.Begin);
        ULONG imageInfoCount = reader.ULONG();
        acs.ImageInfo = new ACSIMAGEINFO[imageInfoCount];
        for (var i = 0; i < imageInfoCount; i++)
        {
            acs.ImageInfo[i] = ACSIMAGEINFO.Parse(reader);
        }

        acs.Images = new IMAGE[imageInfoCount];
        for (var i = 0; i < acs.ImageInfo.Length; i++)
        {
            stream.Seek(acs.ImageInfo[i].InfoLocation.Offset, SeekOrigin.Begin);
            acs.Images[i] = IMAGE.Parse(reader);
        }

        //
        // Debug: ACSAUDIOINFO[]
        //
        // Console.WriteLine($"Position: {stream.Position}");
        // Console.WriteLine($"Audio Info List Location: {header.AudioInfoList.Offset:X8} (size {header.AudioInfoList.Size} bytes)");
        // Util.PrintMemory(stream, reader, header.AudioInfoList);
        // Console.WriteLine("");
        // Console.WriteLine($"Position: {stream.Position}");

        //
        // Read: ACSAUDIOINFO[]
        //
        stream.Seek(header.AudioInfoList.Offset, SeekOrigin.Begin);
        ULONG audioInfoCount = reader.ULONG();
        acs.AudioInfo = new ACSAUDIOINFO[audioInfoCount];
        for (var i = 0; i < audioInfoCount; i++)
        {
            acs.AudioInfo[i] = ACSAUDIOINFO.Parse(reader);
        }

        acs.Audios = new AUDIO[audioInfoCount];
        for (var i = 0; i < acs.AudioInfo.Length; i++)
        {
            stream.Seek(acs.AudioInfo[i].AudioData.Offset, SeekOrigin.Begin);
            acs.Audios[i] = AUDIO.Parse(reader);
        }

        return acs;
    }

    public void Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream);

        Header.Write(writer);

        // TODO;
    }
}
