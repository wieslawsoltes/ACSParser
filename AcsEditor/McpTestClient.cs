using System;
using System.Threading.Tasks;
using AcsEditor.Models;
using AcsEditor.Services;

namespace AcsEditor;

/// <summary>
/// Simple test client to demonstrate MCP functionality
/// This can be used to test the MCP tools without needing an external MCP client
/// </summary>
public class McpTestClient
{
    public static async Task RunTestAsync()
    {
        Console.WriteLine("=== MCP Animation Service Test ===");
        
        // Simulate loading an ACS file
        var testFile = new AcsFile
        {
            Name = "Test Character",
            FilePath = "/test/path/character.acs",
            Animations = new()
            {
                new AcsAnimation 
                { 
                    Name = "Idle", 
                    Frames = new() { new AcsFrame { Duration = 1000 }, new AcsFrame { Duration = 500 } }
                },
                new AcsAnimation 
                { 
                    Name = "Wave", 
                    Frames = new() { new AcsFrame { Duration = 200 }, new AcsFrame { Duration = 300 }, new AcsFrame { Duration = 250 } }
                },
                new AcsAnimation 
                { 
                    Name = "Speak", 
                    Frames = new() { new AcsFrame { Duration = 150 } }
                }
            },
            Images = new() { new AcsImage { Name = "image1" }, new AcsImage { Name = "image2" } },
            AudioFiles = new() { new AcsAudio { Name = "sound1" } }
        };

        // Initialize the MCP service
        McpAnimationService.Initialize(testFile, OnAnimationSelected);
        
        Console.WriteLine("\n1. Testing ListAnimations:");
        Console.WriteLine(McpAnimationService.ListAnimations());
        
        Console.WriteLine("\n2. Testing GetFileInfo:");
        Console.WriteLine(McpAnimationService.GetFileInfo());
        
        Console.WriteLine("\n3. Testing SelectAnimationByIndex(1):");
        Console.WriteLine(McpAnimationService.SelectAnimationByIndex(1));
        
        Console.WriteLine("\n4. Testing SelectAnimationByName('Idle'):");
        Console.WriteLine(McpAnimationService.SelectAnimationByName("Idle"));
        
        Console.WriteLine("\n5. Testing GetAnimationDetails(0):");
        Console.WriteLine(McpAnimationService.GetAnimationDetails(0));
        
        Console.WriteLine("\n6. Testing SelectAnimationByName('wave') (case insensitive):");
        Console.WriteLine(McpAnimationService.SelectAnimationByName("wave"));
        
        Console.WriteLine("\n7. Testing invalid index:");
        Console.WriteLine(McpAnimationService.SelectAnimationByIndex(10));
        
        Console.WriteLine("\n8. Testing non-existent animation name:");
        Console.WriteLine(McpAnimationService.SelectAnimationByName("NonExistent"));
        
        Console.WriteLine("\n=== Test Complete ===");
    }
    
    private static void OnAnimationSelected(AcsAnimation animation)
    {
        Console.WriteLine($"*** CALLBACK: Animation '{animation.Name}' was selected! ***");
    }
}