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
        // Parse(@"/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT.ACS");
        Parse(@"c:\Users\Administrator\Documents\GitHub\Acdparser\clippitMS\CLIPPIT.ACS");
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

            // TODO:
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
    }
}
