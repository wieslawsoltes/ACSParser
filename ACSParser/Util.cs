using ACSParser.DataStructures;

namespace ACSParser;
    
internal class Style
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

internal static class Util
{
    public static bool IsBitSet(ULONG number, int bitPosition)
    {
        ULONG mask = (ULONG)1 << bitPosition;
        return (number & mask) != 0;
    }

    public static void PrintMemory(Stream input, BinaryReader reader, ULONG offset, ULONG size, List<Style>? styles = null)
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

    public static void PrintMemory(Stream input, BinaryReader reader, ACSLOCATOR locator, List<Style>? styles = null)
    {
        PrintMemory(input, reader, locator.Offset, locator.Size, styles);
    }

    public static List<Style> CharacterInfoStyles()
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
