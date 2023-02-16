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
        //Parse(@"/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT.ACS");
        Parse(@"c:\Users\Administrator\Documents\GitHub\Acdparser\clippitMS\CLIPPIT.ACS");
    }

    private static void Parse(string acsFile)
    {
        if (!File.Exists(acsFile))
        {
            Console.WriteLine("File not found: {0}", acsFile);
            return;
        }

        Console.WriteLine($"ACS File: {acsFile}");

        try
        {
            using var input = File.OpenRead(acsFile);
            using var reader = new BinaryReader(input);
            
            
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            var header = ACSHEADER.Parse(reader);
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine("ACS File Header:");
            Console.WriteLine("Signature: 0x{0:X8}", header.Signature);

            
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine($"Character Info");
            var characterInfoStyles = CharacterInfoStyles();
            PrintMemory(input, reader, header.CharacterInfo.Offset, 41, characterInfoStyles);
            Console.WriteLine("");
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            
            
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            Console.WriteLine($"Character Info Location: {header.CharacterInfo.Offset:X8} (size {header.CharacterInfo.Size} bytes)");
            //PrintMemory(input, reader, header.CharacterInfo, characterInfoStyles);
            PrintMemory(input, reader, header.CharacterInfo.Offset, 160, characterInfoStyles);
            Console.WriteLine("");
  
            //Console.WriteLine($"Position: {reader.BaseStream.Position}");
            //Console.WriteLine($"Animation Info List Location: {header.AnimationInfoList.Offset:X8} (size {header.AnimationInfoList.Size} bytes)");
            //PrintMemory(input, reader, header.AnimationInfoList);
            //Console.WriteLine("");
            //Console.WriteLine($"Position: {reader.BaseStream.Position}");

            //Console.WriteLine($"Position: {reader.BaseStream.Position}");
            //Console.WriteLine($"Image Info List Location: {header.ImageInfoList.Offset:X8} (size {header.ImageInfoList.Size} bytes)");
            //PrintMemory(input, reader, header.ImageInfoList);
            //Console.WriteLine("");
            //Console.WriteLine($"Position: {reader.BaseStream.Position}");

            //Console.WriteLine($"Position: {reader.BaseStream.Position}");
            //Console.WriteLine($"Audio Info List Location: {header.AudioInfoList.Offset:X8} (size {header.AudioInfoList.Size} bytes)");
            //PrintMemory(input, reader, header.AudioInfoList);
            //Console.WriteLine("");
            //Console.WriteLine($"Position: {reader.BaseStream.Position}");

            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            input.Seek(header.CharacterInfo.Offset, SeekOrigin.Begin);
            Console.WriteLine($"Position: {reader.BaseStream.Position}");
            var characterInfo = ACSCHARACTERINFO.Parse(reader);
            Console.WriteLine($"Position: {reader.BaseStream.Position}");

            // TODO:
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
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

    private class Style
    {
        public readonly ULONG Start;
        public readonly ULONG End;
        public readonly ConsoleColor Color;

        public Style(uint start, uint end, ConsoleColor color)
        {
            Start = start;
            End = end;
            Color = color;
        }

        public bool Match(ULONG offset) => offset >= Start && offset < End;
    }

    private static void PrintMemory(FileStream input, BinaryReader reader, ACSLOCATOR locator, List<Style>? styles = null)
    {
        PrintMemory(input, reader, locator.Offset, locator.Size, styles);
    }

    private static void PrintMemory(FileStream input, BinaryReader reader, ULONG offset, ULONG size, List<Style>? styles = null)
    {
        var currentOffset = input.Position;
        input.Seek(offset, SeekOrigin.Begin);
        var byteArray = reader.ReadBytes((int)size);

        var addressOffset = offset % 16; // Offset into 16-byte boundary
        var alignedOffset = offset - addressOffset; // Starting offset aligned to 16-byte boundary

        var currentColor = Console.ForegroundColor;
        
        for (var i = 0; i < byteArray.Length; i += 16)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{(i + alignedOffset):X8}: ");
            Console.ForegroundColor = currentColor;

            for (var j = i; j < i + 16; j++)
            {
                if (j < addressOffset || j >= addressOffset + byteArray.Length)
                {
                    if ((j + 1 - alignedOffset) % 8 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("   | ");
                        Console.ForegroundColor = currentColor;
                    }
                    else
                    {
                        Console.Write("   "); 
                    }
                }
                else
                {
                    var byteOffset = j - addressOffset;
                    var style = styles?.FirstOrDefault(x => x.Match((ULONG)byteOffset));
                    Console.ForegroundColor = style?.Color ?? ConsoleColor.Black;
                    Console.Write($"{byteArray[byteOffset]:X2} ");
                    Console.ForegroundColor = currentColor;

                    if ((j + 1 - alignedOffset) % 8 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("| ");
                        Console.ForegroundColor = currentColor;
                    }
                }
            }

            Console.Write(" ");

            for (int j = i; j < i + 16; j++)
            {
                if (j < byteArray.Length + addressOffset && j >= addressOffset)
                {
                    var byteOffset = j - addressOffset;
                    char c = (char)byteArray[byteOffset];

                    if (Char.IsControl(c))
                    {
                        Console.Write(' ');
                    }
                    else
                    {
                        var style = styles?.FirstOrDefault(x => x.Match((ULONG)byteOffset));
                        Console.ForegroundColor = style?.Color ?? ConsoleColor.DarkGray;
                        Console.Write(c); 
                        Console.ForegroundColor = currentColor;
                    }
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }

        input.Seek(currentOffset, SeekOrigin.Begin);
    }
}
