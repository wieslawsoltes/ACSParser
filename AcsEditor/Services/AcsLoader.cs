using System;
using System.IO;
using System.Linq;
using AcsEditor.Models;
using ACSParser;
using ACSParser.DataStructures;

namespace AcsEditor.Services;

public class AcsLoader
{
    public static string BasePath = "";

    public static AcsFile? Load(string path)
    {
        try
        {
            using var stream = File.OpenRead(path);
            Console.WriteLine($"Loading ACS file: {path}");
            var acsData = ACS.Parse(stream);
            Console.WriteLine($"Parsed ACS: {acsData.AnimationInfo.Length} animations, {acsData.ImageInfo.Length} images");
            
            var basePath = Path.GetDirectoryName(path);
            if (basePath is not null)
            {
                BasePath = basePath;
                Console.WriteLine($"Set BasePath to: {BasePath}");
            }

            var result = ConvertToViewModel(acsData, path);
            Console.WriteLine($"Converted to ViewModel: {result.Animations.Count} animations, {result.Images.Count} images");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading ACS file: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    private static AcsFile ConvertToViewModel(ACS acsData, string filePath)
    {
        Console.WriteLine($"ACS TransparentColorIndex from file: {acsData.CharacterInfo.TransparentColorIndex}");
        if (acsData.CharacterInfo.ColorTable != null && acsData.CharacterInfo.TransparentColorIndex < acsData.CharacterInfo.ColorTable.Length)
        {
            var transparentColor = acsData.CharacterInfo.ColorTable[acsData.CharacterInfo.TransparentColorIndex].Color;
            Console.WriteLine($"Transparent color from palette: RGB({transparentColor.Red}, {transparentColor.Green}, {transparentColor.Blue})");
        }
        
        var acsFile = new AcsFile
        {
            Name = Path.GetFileNameWithoutExtension(filePath),
            FilePath = filePath,
            CharacterInfo = new AcsCharacterInfo
            {
                Name = "Character" // TODO: Extract actual character name if available
            },
            ColorTable = acsData.CharacterInfo.ColorTable,
            TransparentColorIndex = acsData.CharacterInfo.TransparentColorIndex
        };

        // Convert animations
        for (int i = 0; i < acsData.AnimationInfo.Length; i++)
        {
            var animInfo = acsData.AnimationInfo[i];
            var animData = acsData.Animations[i];
            
            var animation = new AcsAnimation
            {
                Name = animInfo.AnimationName.AsString() ?? $"Animation {i}",
                TransitionType = animData.TransitionType,
                ReturnAnimation = animData.ReturnAnimation?.AsString()
            };

            // Convert frames
            for (int frameIndex = 0; frameIndex < animData.AnimationFrames.Length; frameIndex++)
            {
                var frameData = animData.AnimationFrames[frameIndex];
                
                var frame = new AcsFrame
                {
                    AudioIndex = frameData.AudioIndex,
                    Duration = frameData.Duration,
                    ExitFrameIndex = frameData.ExitFrameIndex
                };

                // Convert frame images
                foreach (var frameImage in frameData.Images)
                {
                    frame.Images.Add(new AcsFrameImage
                    {
                        ImageIndex = frameImage.ImageIndex,
                        XOffset = frameImage.XOffset,
                        YOffset = frameImage.YOffset
                    });
                }

                // Convert branches
                foreach (var branch in frameData.Branches)
                {
                    frame.Branches.Add(new AcsBranch
                    {
                        Probability = branch.Probability,
                        Animation = $"Frame {branch.FrameIndex}" // BRANCHINFO points to frame index, not animation name
                    });
                }

                // Convert overlays
                foreach (var overlay in frameData.Overlays)
                {
                    frame.Overlays.Add(new AcsOverlay
                    {
                        ImageIndex = overlay.ImageIndex,
                        XOffset = overlay.XOffset,
                        YOffset = overlay.YOffset
                    });
                }

                animation.Frames.Add(frame);
            }

            acsFile.Animations.Add(animation);
        }

        // Convert images
        for (int i = 0; i < acsData.ImageInfo.Length; i++)
        {
            var imageInfo = acsData.ImageInfo[i];
            var imageData = acsData.Images[i];
            
            var image = new AcsImage
            {
                Name = $"Image {i}",
                Width = imageData.Width,
                Height = imageData.Height,
                IsCompressed = imageData.IsImageDataCompressed != 0,
                ImageData = imageData.ImageData.Data,
                OriginalImage = imageData  // Store reference to original IMAGE object
            };

            acsFile.Images.Add(image);
        }

        // Convert audio - note: AUDIO structure is not fully implemented yet
        for (int i = 0; i < acsData.AudioInfo.Length; i++)
        {
            var audioInfo = acsData.AudioInfo[i];
            // var audioData = acsData.Audios[i]; // AUDIO is not implemented yet
            
            var audio = new AcsAudio
            {
                Name = $"Audio {i}",
                AudioData = null // TODO: Implement when AUDIO structure is complete
            };

            acsFile.AudioFiles.Add(audio);
        }

        return acsFile;
    }
}