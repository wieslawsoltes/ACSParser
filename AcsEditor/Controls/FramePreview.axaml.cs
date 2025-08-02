using System;
using AcsEditor.Models;
using AcsEditor.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Skia;

namespace AcsEditor.Controls;

public class FramePreview : TemplatedControl
{
    public static readonly StyledProperty<AcsFrame?> FrameProperty = 
        AvaloniaProperty.Register<FramePreview, AcsFrame?>(nameof(Frame));

    public AcsFrame? Frame
    {
        get => GetValue(FrameProperty);
        set => SetValue(FrameProperty, value);
    }

    private SKBitmapControl? _bitmapControl;
    private TextBlock? _noFrameText;
    private AcsFile? _acsFile;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _bitmapControl = e.NameScope.Find<SKBitmapControl>("PART_BitmapControl");
        _noFrameText = e.NameScope.Find<TextBlock>("PART_NoFrameText");

        UpdateDisplay();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FrameProperty)
        {
            UpdateDisplay();
        }
        else if (change.Property == DataContextProperty)
        {
            // Try to get AcsFile from DataContext hierarchy
            var context = DataContext;
            while (context != null)
            {
                if (context is AcsFile acsFile)
                {
                    _acsFile = acsFile;
                    UpdateDisplay();
                    break;
                }
                // Try to get parent's DataContext
                if (context is Control control && control.Parent != null)
                {
                    context = control.Parent.DataContext;
                }
                else
                {
                    break;
                }
            }

            // Also check if we can get it from the parent chain
            if (_acsFile == null)
            {
                var parent = Parent;
                while (parent != null)
                {
                    if (parent.DataContext is AcsFile file)
                    {
                        _acsFile = file;
                        UpdateDisplay();
                        break;
                    }
                    parent = parent.Parent;
                }
            }
        }
    }

    private void UpdateDisplay()
    {
        if (_bitmapControl == null || _noFrameText == null) return;

        var frame = Frame;
        var hasFrame = frame != null && _acsFile != null;

        if (hasFrame)
        {
            try
            {
                var bitmap = ImageConverter.CompositeFrameImages(_acsFile!, frame!);
                _bitmapControl.Bitmap = bitmap;
                _noFrameText.IsVisible = bitmap == null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying frame: {ex.Message}");
                _bitmapControl.Bitmap = null;
                _noFrameText.IsVisible = true;
            }
        }
        else
        {
            _bitmapControl.Bitmap = null;
            _noFrameText.IsVisible = true;
        }
    }
}