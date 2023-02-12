using System;
using System.IO;

namespace ACSParser;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: ACSParser.exe <ACS file>");
            return;
        }

        var acsFile = args[0];

        if (!File.Exists(acsFile))
        {
            Console.WriteLine("File not found: {0}", acsFile);
            return;
        }

        try
        {
            using (var reader = new BinaryReader(File.OpenRead(acsFile)))
            {
                var header = ACSHEADER.Parse(reader);

                Console.WriteLine("ACS File Header:");
                Console.WriteLine("  Signature: 0x{0:X8}", header.Signature);
                Console.WriteLine("  Character Info Location: {0} (size {1} bytes)", header.CharacterInfo.Offset, header.CharacterInfo.Size);
                Console.WriteLine("  Animation Info List Location: {0} (size {1} bytes)", header.AnimationInfoList.Offset, header.AnimationInfoList.Size);
                Console.WriteLine("  Image Info List Location: {0} (size {1} bytes)", header.ImageInfoList.Offset, header.ImageInfoList.Size);
                Console.WriteLine("  Audio Info List Location: {0} (size {1} bytes)", header.AudioInfoList.Offset, header.AudioInfoList.Size);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
    }
}