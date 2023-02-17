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

                // TODO: BMP header
                var fileHeader = new BITMAPFILEHEADER
                {
                    Type = 0x424D,
                    Size = 0,
                    Reserved1 = 0,
                    Reserved2 = 0,
                    OffBits = 0,
                };
                binaryWriter.Write(fileHeader.Type);
                binaryWriter.Write(fileHeader.Size);
                binaryWriter.Write(fileHeader.Reserved1);
                binaryWriter.Write(fileHeader.Reserved2);
                binaryWriter.Write(fileHeader.OffBits);

                // TODO: BMP info header
                var infoHeader = new BITMAPINFOHEADER
                {
                    Size = 0,
                    Width = 0,
                    Height = 0,
                    Planes = 0,
                    BitCount = 0,
                    Compression = 0,
                    SizeImage = 0,
                    XPelsPerMeter = 0,
                    YPelsPerMeter = 0,
                    ClrUsed = 0,
                    ClrImportant = 0
                };
                binaryWriter.Write(infoHeader.Size);
                binaryWriter.Write(infoHeader.Width);
                binaryWriter.Write(infoHeader.Height);
                binaryWriter.Write(infoHeader.Planes);
                binaryWriter.Write(infoHeader.BitCount);
                binaryWriter.Write(infoHeader.Compression);
                binaryWriter.Write(infoHeader.SizeImage);
                binaryWriter.Write(infoHeader.XPelsPerMeter);
                binaryWriter.Write(infoHeader.YPelsPerMeter);
                binaryWriter.Write(infoHeader.ClrUsed);
                binaryWriter.Write(infoHeader.ClrImportant);

                var colorTable = acs.CharacterInfo.ColorTable;
                foreach (var paletteColor in colorTable)
                {
                    binaryWriter.Write(paletteColor.Color.Red);
                    binaryWriter.Write(paletteColor.Color.Green);
                    binaryWriter.Write(paletteColor.Color.Blue);
                    binaryWriter.Write(paletteColor.Color.Reserved);
                }

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
