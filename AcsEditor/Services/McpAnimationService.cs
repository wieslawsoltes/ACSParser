using System;
using System.ComponentModel;
using System.Linq;
using AcsEditor.Models;
using ModelContextProtocol.Server;

namespace AcsEditor.Services;

/// <summary>
/// MCP service that provides tools for animation selection and management
/// </summary>
[McpServerToolType]
public class McpAnimationService
{
    private static AcsFile? _currentFile;
    private static Action<AcsAnimation>? _onAnimationSelected;

    /// <summary>
    /// Initialize the MCP service with the current ACS file and selection callback
    /// </summary>
    public static void Initialize(AcsFile? file, Action<AcsAnimation>? onAnimationSelected)
    {
        _currentFile = file;
        _onAnimationSelected = onAnimationSelected;
    }

    [McpServerTool, Description("List all available animations in the current ACS file")]
    public static string ListAnimations()
    {
        if (_currentFile?.Animations == null || !_currentFile.Animations.Any())
        {
            return "No animations available. Please load an ACS file first.";
        }

        var animationList = _currentFile.Animations
            .Select((anim, index) => $"{index}: {anim.Name ?? "Unnamed Animation"} ({anim.Frames.Count} frames)")
            .ToList();

        return $"Available animations ({_currentFile.Animations.Count} total):\n" + 
               string.Join("\n", animationList);
    }

    [McpServerTool, Description("Select an animation by its index (0-based)")]
    public static string SelectAnimationByIndex(int index)
    {
        if (_currentFile?.Animations == null || !_currentFile.Animations.Any())
        {
            return "No animations available. Please load an ACS file first.";
        }

        if (index < 0 || index >= _currentFile.Animations.Count)
        {
            return $"Invalid animation index {index}. Available range: 0-{_currentFile.Animations.Count - 1}";
        }

        var selectedAnimation = _currentFile.Animations[index];
        _onAnimationSelected?.Invoke(selectedAnimation);
        
        return $"Selected animation {index}: {selectedAnimation.Name ?? "Unnamed Animation"} with {selectedAnimation.Frames.Count} frames";
    }

    [McpServerTool, Description("Select an animation by its name (case-insensitive partial match)")]
    public static string SelectAnimationByName(string name)
    {
        if (_currentFile?.Animations == null || !_currentFile.Animations.Any())
        {
            return "No animations available. Please load an ACS file first.";
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return "Animation name cannot be empty.";
        }

        var matchingAnimations = _currentFile.Animations
            .Where(anim => anim.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        if (!matchingAnimations.Any())
        {
            return $"No animations found matching '{name}'. Use ListAnimations to see available options.";
        }

        if (matchingAnimations.Count > 1)
        {
            var matches = matchingAnimations
                .Select((anim, index) => $"{_currentFile.Animations.IndexOf(anim)}: {anim.Name}")
                .ToList();
            return $"Multiple animations match '{name}':\n" + string.Join("\n", matches) + 
                   "\nPlease use SelectAnimationByIndex with a specific index.";
        }

        var selectedAnimation = matchingAnimations.First();
        var animationIndex = _currentFile.Animations.IndexOf(selectedAnimation);
        _onAnimationSelected?.Invoke(selectedAnimation);
        
        return $"Selected animation '{selectedAnimation.Name}' (index {animationIndex}) with {selectedAnimation.Frames.Count} frames";
    }

    [McpServerTool, Description("Get detailed information about a specific animation by index")]
    public static string GetAnimationDetails(int index)
    {
        if (_currentFile?.Animations == null || !_currentFile.Animations.Any())
        {
            return "No animations available. Please load an ACS file first.";
        }

        if (index < 0 || index >= _currentFile.Animations.Count)
        {
            return $"Invalid animation index {index}. Available range: 0-{_currentFile.Animations.Count - 1}";
        }

        var animation = _currentFile.Animations[index];
        var details = $"Animation Details:\n" +
                     $"Index: {index}\n" +
                     $"Name: {animation.Name ?? "Unnamed Animation"}\n" +
                     $"Return Animation: {animation.ReturnAnimation ?? "None"}\n" +
                     $"Transition Type: {animation.TransitionType}\n" +
                     $"Frame Count: {animation.Frames.Count}\n";

        if (animation.Frames.Any())
        {
            details += "Frames:\n";
            for (int i = 0; i < animation.Frames.Count; i++)
            {
                var frame = animation.Frames[i];
                details += $"  Frame {i}: Duration {frame.Duration}ms, Images: {frame.Images.Count}\n";
            }
        }

        return details;
    }

    [McpServerTool, Description("Get the currently loaded ACS file information")]
    public static string GetFileInfo()
    {
        if (_currentFile == null)
        {
            return "No ACS file is currently loaded.";
        }

        return $"Current ACS File:\n" +
               $"Name: {_currentFile.Name ?? "Unknown"}\n" +
               $"File Path: {_currentFile.FilePath ?? "Unknown"}\n" +
               $"Animations: {_currentFile.Animations.Count}\n" +
               $"Images: {_currentFile.Images.Count}\n" +
               $"Audio Files: {_currentFile.AudioFiles.Count}\n" +
               $"Character Info: {(_currentFile.CharacterInfo != null ? "Available" : "None")}";
    }
}