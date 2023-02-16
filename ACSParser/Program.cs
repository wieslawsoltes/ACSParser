using ACSParser.DataStructures;

namespace ACSParser;

class Program
{
    static void Main(string[] args)
    {
        //if (args.Length != 1)
        //{
        //    Console.WriteLine("Usage: ACSParser.exe <ACS file>");
        //    return;
        //}

        //var acsFile = args[0];
        //Parse(acsFile);

        //Parse(@"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT_noballoon.acs");
        //Parse(@"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT_novoice.acs");
        //Parse(@"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT_voice.acs");
        Parse(@"/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT.ACS");
        //Parse(@"c:\Users\Administrator\Documents\GitHub\Acdparser\clippitMS\CLIPPIT.ACS");
    }

    private static ACS? Parse(string acsFile)
    {
        if (!File.Exists(acsFile))
        {
            Console.WriteLine("File not found: {0}", acsFile);
            return null;
        }

        Console.WriteLine($"ACS File: {acsFile}");

        ACS acs = new ACS();

        try
        {
            using var input = File.OpenRead(acsFile);
            using var reader = new BinaryReader(input);

            //
            // ACSHEADER
            //
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            var header = ACSHEADER.Parse(reader);
            acs.Header = header;
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine("ACS File Header:");
            Console.WriteLine("Signature: 0x{0:X8}", header.Signature);

            //
            // Debug: ACSCHARACTERINFO
            //
            //Console.WriteLine($"Position: {reader.BaseStream.Position}");
            //Console.WriteLine($"Character Info");
            //var characterInfoStyles = CharacterInfoStyles();
            //Util.PrintMemory(input, reader, header.CharacterInfo.Offset, 41, characterInfoStyles);
            //Console.WriteLine("");
            //Console.WriteLine($"Position: {reader.BaseStream.Position}");
            
            //
            // Debug: ACSCHARACTERINFO
            //
            //Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine($"Character Info Location: {header.CharacterInfo.Offset:X8} (size {header.CharacterInfo.Size} bytes)");
            //Util.PrintMemory(input, reader, header.CharacterInfo.Offset, 160, characterInfoStyles);
            //Util.PrintMemory(input, reader, header.CharacterInfo, characterInfoStyles);
            //Console.WriteLine("");
 
            //
            // Read: ACSCHARACTERINFO
            //
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            input.Seek(header.CharacterInfo.Offset, SeekOrigin.Begin);
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            var characterInfo = ACSCHARACTERINFO.Parse(reader);
            acs.CharacterInfo = characterInfo;
            Console.WriteLine($"Position: {reader.BaseStream.Position}");

            //
            // Debug: ACSANIMATIONINFO[]
            //
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine($"Animation Info List Location: {header.AnimationInfoList.Offset:X8} (size {header.AnimationInfoList.Size} bytes)");
            Util.PrintMemory(input, reader, header.AnimationInfoList);
            Console.WriteLine("");
            Console.WriteLine($"Position: {reader.BaseStream.Position}");

            //
            // Read: ACSANIMATIONINFO[]
            //
            input.Seek(header.AnimationInfoList.Offset, SeekOrigin.Begin);
            ULONG animationInfoCount = reader.ULONG();
            acs.AnimationInfo = new ACSANIMATIONINFO[animationInfoCount];
            for (var i = 0; i < animationInfoCount; i++)
            {
                acs.AnimationInfo[i] = ACSANIMATIONINFO.Parse(reader);
            }

            acs.Animations = new ANIMATION[animationInfoCount];
            for (var i = 0; i < acs.AnimationInfo.Length; i++)
            {
                input.Seek(acs.AnimationInfo[i].AnimationLocator.Offset, SeekOrigin.Begin);
                acs.Animations[i] = ANIMATION.Parse(reader);
            }

            //
            // Debug: ACSIMAGEINFO[]
            //
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine($"Image Info List Location: {header.ImageInfoList.Offset:X8} (size {header.ImageInfoList.Size} bytes)");
            Util.PrintMemory(input, reader, header.ImageInfoList);
            Console.WriteLine("");
            Console.WriteLine($"Position: {reader.BaseStream.Position}");

            //
            // Read: ACSIMAGEINFO[]
            //
            input.Seek(header.ImageInfoList.Offset, SeekOrigin.Begin);
            ULONG imageInfoCount = reader.ULONG();
            acs.ImageInfo = new ACSIMAGEINFO[imageInfoCount];
            for (var i = 0; i < imageInfoCount; i++)
            {
                acs.ImageInfo[i] = ACSIMAGEINFO.Parse(reader);
            }

            acs.Images = new IMAGE[imageInfoCount];
            for (var i = 0; i < acs.ImageInfo.Length; i++)
            {
                input.Seek(acs.ImageInfo[i].InfoLocation.Offset, SeekOrigin.Begin);
                acs.Images[i] = IMAGE.Parse(reader);
            }

            //
            // Debug: ACSAUDIOINFO[]
            //
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine($"Audio Info List Location: {header.AudioInfoList.Offset:X8} (size {header.AudioInfoList.Size} bytes)");
            Util.PrintMemory(input, reader, header.AudioInfoList);
            Console.WriteLine("");
            Console.WriteLine($"Position: {reader.BaseStream.Position}");

            //
            // Read: ACSAUDIOINFO[]
            //
            input.Seek(header.AudioInfoList.Offset, SeekOrigin.Begin);
            ULONG audioInfoCount = reader.ULONG();
            acs.AudioInfo = new ACSAUDIOINFO[audioInfoCount];
            for (var i = 0; i < audioInfoCount; i++)
            {
                acs.AudioInfo[i] = ACSAUDIOINFO.Parse(reader);
            }

            acs.Audios = new AUDIO[audioInfoCount];
            for (var i = 0; i < acs.AudioInfo.Length; i++)
            {
                input.Seek(acs.AudioInfo[i].AudioData.Offset, SeekOrigin.Begin);
                acs.Audios[i] = AUDIO.Parse(reader);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
            return null;
        }

        return acs;
    }

    private static List<Style> CharacterInfoStyles()
    {
        var characterInfoStyles = new List<Style>();
        // MinorVersion
        ULONG start = 0;
        ULONG end = 2;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Blue));
        // MajorVersion
        start = end;
        end += 2;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Green));
        // LocalizedInfo
        start = end;
        end += 8;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Cyan));
        // UniqueId
        start = end;
        end += 16;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Magenta));
        // CharacterWidth
        start = end;
        end += 2;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Blue));
        // CharacterHeight
        start = end;
        end += 2;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Green));
        // TransparentColorIndex
        start = end;
        end += 1;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Yellow));
        // Flags
        start = end;
        end += 4;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Red));
        // AnimationSetMajorVersion
        start = end;
        end += 2;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Blue));
        // AnimationSetMinorVersion
        start = end;
        end += 2;
        characterInfoStyles.Add(new(start, end, ConsoleColor.Green));
        return characterInfoStyles;
    }

}
