using System;
using AcsEditor.Models;
using AcsEditor.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Skia;

namespace AcsEditor.Controls;

public class ImagePreview : TemplatedControl
{
    public static readonly StyledProperty<AcsImage?> ImageProperty = 
        AvaloniaProperty.Register<ImagePreview, AcsImage?>(nameof(Image));

    public AcsImage? Image
    {
        get => GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    private SKBitmapControl? _bitmapControl;
    private TextBlock? _noImageText;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _bitmapControl = e.NameScope.Find<SKBitmapControl>("PART_BitmapControl");
        _noImageText = e.NameScope.Find<TextBlock>("PART_NoImageText");

        UpdateDisplay();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ImageProperty)
        {
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (_bitmapControl == null || _noImageText == null) return;

        var image = Image;
        var hasImage = image != null;

        if (hasImage)
        {
            try
            {
                // Try to get color palette from parent AcsFile
                var acsFile = GetAcsFileFromContext();
                var bitmap = image!.Bitmap ?? ImageConverter.ConvertToBitmap(image, acsFile?.ColorTable, acsFile?.TransparentColorIndex ?? 0);
                _bitmapControl.Bitmap = bitmap;
                _noImageText.IsVisible = bitmap == null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying image: {ex.Message}");
                _bitmapControl.Bitmap = null;
                _noImageText.IsVisible = true;
            }
        }
        else
        {
            _bitmapControl.Bitmap = null;
            _noImageText.IsVisible = true;
        }
    }

    private AcsFile? GetAcsFileFromContext()
    {
        // Try to get AcsFile from DataContext hierarchy
        var context = DataContext;
        while (context != null)
        {
            if (context is AcsFile acsFile)
                return acsFile;
            
            if (context is Control control && control.Parent != null)
                context = control.Parent.DataContext;
            else
                break;
        }

        // Also check parent chain
        var parent = Parent;
        while (parent != null)
        {
            if (parent.DataContext is AcsFile file)
                return file;
            parent = parent.Parent;
        }

        return null;
    }
}