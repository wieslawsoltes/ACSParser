using ACSParser.DataStructures;

namespace ACSParser;

class Program
{
    static void Main(string[] args)
    {
        // if (args.Length != 1)
        // {
        //     Console.WriteLine("Usage: ACSParser.exe <ACS file>");
        //     return;
        // }
        // var acsFile = args[0];
        // Parse(acsFile);

        // Parse(@"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT_noballoon.acs");
        // Parse(@"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT_novoice.acs");
        // Parse(@"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT_voice.acs");
        Parse(@"/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT.ACS");
        //Parse(@"c:\Users\Administrator\Documents\GitHub\Acdparser\clippitMS\CLIPPIT.ACS");
    }

    private static void Parse(string acsFile)
    {
        try
        {
            if (!File.Exists(acsFile))
            {
                Console.WriteLine("File not found: {0}", acsFile);
                return;
            }

            using var stream = File.OpenRead(acsFile);

            Console.WriteLine($"ACS File: {acsFile}");

            var acs = ACS.Parse(stream);

            
            // DEBUG: Dump IMAGE decompressed data
            for (var i = 0; i < acs.Images.Length; i++)
            {
                var image = acs.Images[i];
                var decompressedDataSize = ((image.Width + 3) & 0xFC) * image.Height;
                var decompressed = Decompressor.Decompress(image.ImageData.Data, decompressedDataSize);
                var name = i.ToString().PadLeft(4, '0') + ".bmp";

                using var bitmapStream = File.Create(name);
                using var binaryWriter = new BinaryWriter(bitmapStream);
                
                //  The Image Data has 1 Plane and a Bit Count of 8, and does not use any pixel compression.

                // BITMAPFILEHEADER + BITMAPINFOHEADER
                ULONG offBits = 14 + 40 + (ULONG)acs.CharacterInfo.ColorTable.Length;
                ULONG size = offBits + (ULONG)decompressed.Length;

                // BMP header
                var fileHeader = new BITMAPFILEHEADER
                {
                    Type = 0x4D42,
                    Size = size, // TODO:
                    Reserved1 = 0,
                    Reserved2 = 0,
                    OffBits = offBits, // TODO:
                };
                fileHeader.Write(binaryWriter);

                // BMP info header
                var infoHeader = new BITMAPINFOHEADER
                {
                    Size = 40,
                    Width = image.Width,
                    Height = image.Height,
                    Planes = 1,
                    BitCount = 8,
                    Compression = (ULONG)Compression.BI_RGB,
                    SizeImage = 0, // For BI_RGB it can be zero.
                    XPelsPerMeter = 0, // TODO:
                    YPelsPerMeter = 0, // TODO:
                    ClrUsed = 0,
                    ClrImportant = 0
                };
                infoHeader.Write(binaryWriter);

                // BMP color table
                var colorTable = acs.CharacterInfo.ColorTable;
                foreach (var paletteColor in colorTable)
                {
                    paletteColor.Color.Write(binaryWriter);
                }

                // BMP bytes
                binaryWriter.Write(decompressed);
            }


            // TODO:
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
    }
}
