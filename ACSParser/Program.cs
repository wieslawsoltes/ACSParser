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
        var acsFile = @"c:\Users\Administrator\Documents\GitHub\Acdparser\clippitMS\CLIPPIT.ACS"; 
        //var acsFile = @"/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT.ACS";

        if (!File.Exists(acsFile))
        {
            Console.WriteLine("File not found: {0}", acsFile);
            return;
        }

        try
        {
            using var input = File.OpenRead(acsFile);
            using var reader = new BinaryReader(input);
            
            var header = ACSHEADER.Parse(reader);
            Console.WriteLine("ACS File Header:");
            Console.WriteLine("Signature: 0x{0:X8}", header.Signature);

            Console.WriteLine($"Character Info Location: {header.CharacterInfo.Offset:X8} (size {header.CharacterInfo.Size} bytes)");
            PrintMemory(input, reader, header.CharacterInfo);
            Console.WriteLine("");
  
            Console.WriteLine($"Animation Info List Location: {header.AnimationInfoList.Offset:X8} (size {header.AnimationInfoList.Size} bytes)");
            PrintMemory(input, reader, header.AnimationInfoList);
            Console.WriteLine("");

            Console.WriteLine($"Image Info List Location: {header.ImageInfoList.Offset:X8} (size {header.ImageInfoList.Size} bytes)");
            PrintMemory(input, reader, header.ImageInfoList);
            Console.WriteLine("");

            Console.WriteLine($"Audio Info List Location: {header.AudioInfoList.Offset:X8} (size {header.AudioInfoList.Size} bytes)");
            PrintMemory(input, reader, header.AudioInfoList);
            Console.WriteLine("");

            var characterInfo = ACSCHARACTERINFO.Parse(reader);

            // TODO:
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
    }

    private static void PrintMemory(FileStream input, BinaryReader reader, ACSLOCATOR locator)
    {            
        var offset = locator.Offset;
        var size = locator.Size;

        var currentOffset = input.Position;
        input.Seek(offset, SeekOrigin.Begin);
        var byteArray = reader.ReadBytes((int)size);

        var addressOffset = offset % 16; // Offset into 16-byte boundary
        var alignedOffset = offset - addressOffset; // Starting offset aligned to 16-byte boundary

        for (int i = 0; i < byteArray.Length; i += 16)
        {
            Console.Write($"{(i + alignedOffset):X8}: ");
            for (int j = i; j < i + 16; j++)
            {
                if (j < addressOffset || j >= addressOffset + byteArray.Length)
                {
                    if ((j + 1 - alignedOffset) % 8 == 0)
                    {
                        Console.Write("   | ");
                    }
                    else
                    {
                        Console.Write("   "); 
                    }
                }
                else
                {
                    Console.Write($"{byteArray[j - addressOffset]:X2} ");
                    if ((j + 1 - alignedOffset) % 8 == 0)
                    {
                        Console.Write("| ");
                    }
                }
            }
            Console.Write(" ");
            for (int j = i; j < i + 16; j++)
            {
                if (j < byteArray.Length + addressOffset && j >= addressOffset)
                {
                    char c = (char)byteArray[j - addressOffset];
                    Console.Write(Char.IsControl(c) ? ' ' : c);
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
