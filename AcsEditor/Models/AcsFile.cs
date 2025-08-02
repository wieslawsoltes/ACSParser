using System.Collections.Generic;
using ACSParser.DataStructures;

namespace AcsEditor.Models;

public class AcsFile : AcsBase
{
    public string? Name { get; set; }
    public string? FilePath { get; set; }
    public List<AcsAnimation> Animations { get; set; } = new();
    public List<AcsImage> Images { get; set; } = new();
    public List<AcsAudio> AudioFiles { get; set; } = new();
    public AcsCharacterInfo? CharacterInfo { get; set; }
    public PALETTECOLOR[]? ColorTable { get; set; }
    public byte TransparentColorIndex { get; set; }
}