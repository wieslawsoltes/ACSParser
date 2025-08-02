using SkiaSharp;
using ACSParser.DataStructures;

namespace AcsEditor.Models;

public class AcsImage : AcsBase
{
    public string? Name { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public bool IsCompressed { get; set; }
    public byte[]? ImageData { get; set; }
    public SKBitmap? Bitmap { get; set; }
    
    // Keep reference to original IMAGE object for proper conversion
    public IMAGE? OriginalImage { get; set; }
}