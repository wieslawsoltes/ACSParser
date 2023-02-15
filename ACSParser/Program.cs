using System;
using System.IO;

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
            Console.WriteLine("  Signature: 0x{0:X8}", header.Signature);
            Console.WriteLine("  Character Info Location: {0} (size {1} bytes)", header.CharacterInfo.Offset, header.CharacterInfo.Size);
            Console.WriteLine("  Animation Info List Location: {0} (size {1} bytes)", header.AnimationInfoList.Offset, header.AnimationInfoList.Size);
            Console.WriteLine("  Image Info List Location: {0} (size {1} bytes)", header.ImageInfoList.Offset, header.ImageInfoList.Size);
            Console.WriteLine("  Audio Info List Location: {0} (size {1} bytes)", header.AudioInfoList.Offset, header.AudioInfoList.Size);


            input.Seek(header.CharacterInfo.Offset, SeekOrigin.Begin);
            var characterInfo = ACSCHARACTERINFO.Parse(reader);




        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
    }
}
