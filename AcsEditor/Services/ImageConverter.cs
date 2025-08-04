using System;
using System.IO;
using System.Linq;
using AcsEditor.Models;
using ACSParser;
using ACSParser.DataStructures;
using SkiaSharp;
using BYTE = System.Byte;

namespace AcsEditor.Services;

public class ImageConverter
{
    public static SKBitmap? ConvertToBitmap(AcsImage acsImage, PALETTECOLOR[]? colorTable = null, byte transparentColorIndex = 0)
    {
        // Only return null if we have neither ImageData nor OriginalImage
        if ((acsImage.ImageData == null || acsImage.ImageData.Length == 0) && acsImage.OriginalImage == null)
            return null;

        try
        {
            Console.WriteLine($"Converting image: {acsImage.Width}x{acsImage.Height}, DataSize: {acsImage.ImageData?.Length ?? 0}, Compressed: {acsImage.IsCompressed}");
            
            // Use the original IMAGE object if available (preferred approach)
            if (acsImage.OriginalImage != null)
            {
                Console.WriteLine("Using original IMAGE object with proven SaveBitmap method");
                
                // Create a memory stream and use IMAGE.SaveBitmap (the proven approach from ACSParserConsole)
                var memoryStream = new MemoryStream();
                
                try
                {
                    // Use the proven SaveBitmap method that works in ACSParserConsole
                    acsImage.OriginalImage.SaveBitmap(memoryStream, colorTable ?? new PALETTECOLOR[0]);
                    
                    // Don't reset position, create a new stream from the byte array
                    var bmpBytes = memoryStream.ToArray();
                    Console.WriteLine($"Generated BMP data: {bmpBytes.Length} bytes");
                    
                    using var bmpStream = new MemoryStream(bmpBytes);
                    var skBitmap = SKBitmap.Decode(bmpStream);
                    
                    if (skBitmap != null)
                    {
                        Console.WriteLine($"Successfully converted image to SKBitmap: {skBitmap.Width}x{skBitmap.Height}");
                        
                        // Apply transparency if we have a color table and transparent color index
                        if (colorTable != null && transparentColorIndex < colorTable.Length)
                        {
                            skBitmap = ApplyTransparency(skBitmap, colorTable, transparentColorIndex);
                        }
                        
                        acsImage.Bitmap = skBitmap;
                        return skBitmap;
                    }
                    else
                    {
                        Console.WriteLine("Failed to decode BMP to SKBitmap");
                        return null;
                    }
                }
                finally
                {
                    memoryStream.Dispose();
                }
            }
            else
            {
                Console.WriteLine("No original IMAGE object available, creating new one from AcsImage data");
                
                // Fallback: Create an IMAGE object from AcsImage data
                var memoryStream = new MemoryStream();
                
                try
                {
                    var image = new IMAGE
                    {
                        Width = (ushort)acsImage.Width,
                        Height = (ushort)acsImage.Height,
                        IsImageDataCompressed = acsImage.IsCompressed ? (byte)1 : (byte)0,
                        ImageData = new DATABLOCK { Data = acsImage.ImageData ?? new byte[0] },
                        RegionData = new COMPRESSED { Data = new byte[0], CompressedSize = 0, UncompressedSize = 0 }
                    };
                    
                    // Use the proven SaveBitmap method
                    image.SaveBitmap(memoryStream, colorTable ?? new PALETTECOLOR[0]);
                    
                    // Don't reset position, create a new stream from the byte array
                    var bmpBytes = memoryStream.ToArray();
                    Console.WriteLine($"Generated BMP data: {bmpBytes.Length} bytes");
                    
                    using var bmpStream = new MemoryStream(bmpBytes);
                    var skBitmap = SKBitmap.Decode(bmpStream);
                    
                    if (skBitmap != null)
                    {
                        Console.WriteLine($"Successfully converted image to SKBitmap: {skBitmap.Width}x{skBitmap.Height}");
                        
                        // Apply transparency if we have a color table and transparent color index
                        if (colorTable != null && transparentColorIndex < colorTable.Length)
                        {
                            skBitmap = ApplyTransparency(skBitmap, colorTable, transparentColorIndex);
                        }
                        
                        acsImage.Bitmap = skBitmap;
                        return skBitmap;
                    }
                    else
                    {
                        Console.WriteLine("Failed to decode BMP to SKBitmap");
                        return null;
                    }
                }
                finally
                {
                    memoryStream.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting image to bitmap: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public static SKBitmap? GetBitmapForFrame(AcsFile acsFile, AcsFrameImage frameImage)
    {
        Console.WriteLine($"GetBitmapForFrame: ImageIndex={frameImage.ImageIndex}, Total Images={acsFile.Images.Count}");
        if (frameImage.ImageIndex >= acsFile.Images.Count)
        {
            Console.WriteLine($"Image index {frameImage.ImageIndex} out of range (max: {acsFile.Images.Count - 1})");
            return null;
        }

        var image = acsFile.Images[(int)frameImage.ImageIndex];
        if (image.Bitmap != null)
        {
            Console.WriteLine($"Using cached bitmap for image {frameImage.ImageIndex}");
            return image.Bitmap;
        }

        Console.WriteLine($"Converting image {frameImage.ImageIndex} to bitmap");
        return ConvertToBitmap(image, acsFile.ColorTable, acsFile.TransparentColorIndex);
    }

    public static SKBitmap? CompositeFrameImages(AcsFile acsFile, AcsFrame frame)
    {
        if (frame.Images.Count == 0)
            return null;

        // For single image frames, return the image directly
        if (frame.Images.Count == 1)
        {
            return GetBitmapForFrame(acsFile, frame.Images[0]);
        }

        // For multiple images, composite them with proper offsets
        try
        {
            // Calculate the bounding box for all images
            var minX = frame.Images.Min(img => img.XOffset);
            var minY = frame.Images.Min(img => img.YOffset);
            var maxX = frame.Images.Max(img => 
            {
                var bitmap = GetBitmapForFrame(acsFile, img);
                return img.XOffset + (bitmap?.Width ?? 0);
            });
            var maxY = frame.Images.Max(img => 
            {
                var bitmap = GetBitmapForFrame(acsFile, img);
                return img.YOffset + (bitmap?.Height ?? 0);
            });

            var compositeWidth = maxX - minX;
            var compositeHeight = maxY - minY;

            if (compositeWidth <= 0 || compositeHeight <= 0)
                return GetBitmapForFrame(acsFile, frame.Images[0]);

            var compositeBitmap = new SKBitmap(compositeWidth, compositeHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
            
            using var canvas = new SKCanvas(compositeBitmap);
            canvas.Clear(SKColors.Transparent);

            // Draw each image at its offset position
            foreach (var frameImage in frame.Images)
            {
                var bitmap = GetBitmapForFrame(acsFile, frameImage);
                if (bitmap != null)
                {
                    var x = frameImage.XOffset - minX;
                    var y = frameImage.YOffset - minY;
                    canvas.DrawBitmap(bitmap, x, y);
                }
            }

            return compositeBitmap;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error compositing frame images: {ex.Message}");
            // Fallback to first image
            return GetBitmapForFrame(acsFile, frame.Images[0]);
        }
    }
    
    private static SKBitmap ApplyTransparency(SKBitmap originalBitmap, PALETTECOLOR[] colorTable, byte transparentColorIndex)
    {
        try
        {
            // Get the transparent color from the palette
            var transparentColor = colorTable[transparentColorIndex].Color;
            var transparentSKColor = new SKColor(transparentColor.Red, transparentColor.Green, transparentColor.Blue);
            
            Console.WriteLine($"Applying transparency for color index {transparentColorIndex}: RGB({transparentColor.Red}, {transparentColor.Green}, {transparentColor.Blue})");
            
            // Create a new bitmap with alpha channel
            var transparentBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            
            // Copy pixels and apply transparency
            for (int y = 0; y < originalBitmap.Height; y++)
            {
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    var pixel = originalBitmap.GetPixel(x, y);
                    
                    // Check if this pixel matches the transparent color (ignoring alpha)
                    if (pixel.Red == transparentSKColor.Red && 
                        pixel.Green == transparentSKColor.Green && 
                        pixel.Blue == transparentSKColor.Blue)
                    {
                        // Make this pixel transparent
                        transparentBitmap.SetPixel(x, y, SKColors.Transparent);
                    }
                    else
                    {
                        // Keep the original pixel with full opacity
                        transparentBitmap.SetPixel(x, y, new SKColor(pixel.Red, pixel.Green, pixel.Blue, 255));
                    }
                }
            }
            
            Console.WriteLine($"Applied transparency to {originalBitmap.Width}x{originalBitmap.Height} bitmap");
            return transparentBitmap;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying transparency: {ex.Message}");
            return originalBitmap; // Return original bitmap if transparency application fails
        }
    }
}