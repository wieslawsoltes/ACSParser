using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AcsEditor.Models;
using AcsEditor.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Skia;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SkiaSharp;

namespace AcsEditor.Controls;

public class AnimationPlayer : TemplatedControl
{
    public static readonly StyledProperty<AcsAnimation?> AnimationProperty = 
        AvaloniaProperty.Register<AnimationPlayer, AcsAnimation?>(nameof(Animation));

    public AcsAnimation? Animation
    {
        get => GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    private class AnimationFrame
    {
        public AnimationFrame(SKBitmap? bitmap, TimeSpan duration, int frameIndex)
        {
            Bitmap = bitmap;
            Duration = duration;
            FrameIndex = frameIndex;
        }

        public SKBitmap? Bitmap { get; }
        public TimeSpan Duration { get; }
        public int FrameIndex { get; }
    }

    private SKBitmapControl? _bitmapControl;
    private Button? _playPauseButton;
    private Slider? _progressSlider;
    private ComboBox? _speedComboBox;
    private CheckBox? _loopCheckBox;
    private CheckBox? _audioCheckBox;
    private StackPanel? _noAnimationText;

    private List<AnimationFrame>? _frames;
    private int _currentFrameIndex = 0;
    private bool _isPlaying = false;
    private IDisposable? _animationTimer;
    private AcsFile? _acsFile;
    private readonly double[] _speedMultipliers = { 0.5, 0.75, 1.0, 1.5, 2.0 };
    
    // Animation frame timing
    private DateTime _frameStartTime;
    private TimeSpan _currentFrameDuration;
    
    // Audio support
    private bool _audioEnabled = true;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _bitmapControl = e.NameScope.Find<SKBitmapControl>("PART_BitmapControl");
        _playPauseButton = e.NameScope.Find<Button>("PART_PlayPauseButton");
        _progressSlider = e.NameScope.Find<Slider>("PART_ProgressSlider");
        _speedComboBox = e.NameScope.Find<ComboBox>("PART_SpeedComboBox");
        _loopCheckBox = e.NameScope.Find<CheckBox>("PART_LoopCheckBox");
        _audioCheckBox = e.NameScope.Find<CheckBox>("PART_AudioCheckBox");
        _noAnimationText = e.NameScope.Find<StackPanel>("PART_NoAnimationText");
        


        if (_playPauseButton != null)
        {
            _playPauseButton.Click += PlayPauseButton_Click;
        }

        if (_progressSlider != null)
        {
            _progressSlider.PropertyChanged += ProgressSlider_PropertyChanged;
        }

        if (_audioCheckBox != null)
        {
            _audioCheckBox.PropertyChanged += AudioCheckBox_PropertyChanged;
        }

        UpdateUI();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AnimationProperty)
        {
            var animation = change.GetNewValue<AcsAnimation?>();
            LoadAnimation(animation);
        }
        else if (change.Property == DataContextProperty)
        {
            if (DataContext is AcsFile acsFile)
            {
                _acsFile = acsFile;
                LoadAnimation(Animation);
            }
        }
    }

    private void LoadAnimation(AcsAnimation? animation)
    {
        Console.WriteLine($"LoadAnimation called - Animation: {animation?.Name ?? "None"}, AcsFile: {_acsFile?.Name ?? "None"}");
        StopAnimation();
        _frames = null;
        _currentFrameIndex = 0;

        if (animation == null || _acsFile == null)
        {
            if (animation == null && _acsFile != null)
            {
                Console.WriteLine("No animation selected - waiting for user to select an animation from the tree view");
            }
            else if (animation != null && _acsFile == null)
            {
                Console.WriteLine("Error: Animation selected but no ACS file loaded");
            }
            else
            {
                Console.WriteLine("No animation or ACS file available");
            }
            UpdateUI();
            return;
        }

        try
        {
            Console.WriteLine($"Loading animation '{animation.Name}' with {animation.Frames.Count} frames");
            _frames = new List<AnimationFrame>();

            for (int i = 0; i < animation.Frames.Count; i++)
            {
                var frame = animation.Frames[i];
                Console.WriteLine($"Processing frame {i}/{animation.Frames.Count} with {frame.Images.Count} images, duration: {frame.Duration}ms");
                
                if (frame.Images.Count == 0)
                {
                    Console.WriteLine($"Warning: Frame {i} has no images, skipping");
                    continue;
                }
                
                var bitmap = ImageConverter.CompositeFrameImages(_acsFile, frame);
                var duration = TimeSpan.FromMilliseconds(Math.Max(frame.Duration * 10, 50)); // Minimum 50ms per frame
                
                if (bitmap != null)
                {
                    Console.WriteLine($"Frame {i} bitmap: {bitmap.Width}x{bitmap.Height}");
                }
                else
                {
                    Console.WriteLine($"Warning: Frame {i} bitmap is null");
                }
                
                _frames.Add(new AnimationFrame(bitmap, duration, i));
            }

            if (_frames.Count > 0)
            {
                DisplayCurrentFrame();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading animation: {ex.Message}");
        }

        UpdateUI();
    }

    private void PlayPauseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_isPlaying)
        {
            StopAnimation();
        }
        else
        {
            StartAnimation();
        }
    }

    private void ProgressSlider_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Slider.ValueProperty && _frames != null && !_isPlaying)
        {
            var progress = _progressSlider?.Value ?? 0;
            _currentFrameIndex = (int)Math.Round((progress / 100.0) * (_frames.Count - 1));
            DisplayCurrentFrame();
        }
    }

    private void AudioCheckBox_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == CheckBox.IsCheckedProperty && _audioCheckBox != null)
        {
            _audioEnabled = _audioCheckBox.IsChecked ?? false;
            if (!_audioEnabled)
            {
                StopCurrentAudio();
            }
        }
    }

    private void StartAnimation()
    {
        if (_frames == null || _frames.Count == 0) return;

        _isPlaying = true;
        
        // Initialize frame timing
        var currentFrame = _frames[_currentFrameIndex];
        var speedMultiplier = GetCurrentSpeedMultiplier();
        _currentFrameDuration = TimeSpan.FromMilliseconds(currentFrame.Duration.TotalMilliseconds / speedMultiplier);
        _frameStartTime = DateTime.Now;
        
        // Start the animation loop using RequestAnimationFrame
        RequestAnimationFrame();
        UpdateUI();
    }

    private void StopAnimation()
    {
        _isPlaying = false;
        _animationTimer?.Dispose();
        _animationTimer = null;
        StopCurrentAudio();
        UpdateUI();
    }

    private void RequestAnimationFrame()
    {
        if (!_isPlaying || _frames == null || _frames.Count == 0) return;

        // Get the top level control to request animation frame
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        topLevel.RequestAnimationFrame(OnAnimationFrame);
    }

    private void OnAnimationFrame(TimeSpan timeStamp)
    {
        if (!_isPlaying || _frames == null || _frames.Count == 0) return;

        // Check if it's time to advance to the next frame
        var elapsed = DateTime.Now - _frameStartTime;
        if (elapsed >= _currentFrameDuration)
        {
            // Advance to next frame
            _currentFrameIndex++;
            
            // Check for loop
            if (_currentFrameIndex >= _frames.Count)
            {
                if (_loopCheckBox?.IsChecked == true)
                {
                    _currentFrameIndex = 0;
                }
                else
                {
                    StopAnimation();
                    return;
                }
            }

            // Update frame timing for the new frame
            var currentFrame = _frames[_currentFrameIndex];
            var speedMultiplier = GetCurrentSpeedMultiplier();
            _currentFrameDuration = TimeSpan.FromMilliseconds(currentFrame.Duration.TotalMilliseconds / speedMultiplier);
            _frameStartTime = DateTime.Now;

            // Display the current frame and update progress
            DisplayCurrentFrame();
            UpdateProgress();
            
            // Play audio for this frame if available
            PlayFrameAudio();
        }

        // Schedule the next animation frame
        RequestAnimationFrame();
    }

    private void DisplayCurrentFrame()
    {
        if (_frames == null || _currentFrameIndex >= _frames.Count || _bitmapControl == null) 
        {
            Console.WriteLine("DisplayCurrentFrame: Invalid state or null controls");
            return;
        }

        var frame = _frames[_currentFrameIndex];
        Console.WriteLine($"DisplayCurrentFrame: Frame {_currentFrameIndex}, Bitmap: {frame.Bitmap?.Width ?? 0}x{frame.Bitmap?.Height ?? 0}");
        
        // Ensure we're on the UI thread
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                // Set the bitmap
                _bitmapControl.Bitmap = frame.Bitmap;
                
                // Force invalidation to ensure the frame is properly rendered
                _bitmapControl.InvalidateVisual();
                _bitmapControl.InvalidateMeasure();
                _bitmapControl.InvalidateArrange();
                
                Console.WriteLine($"Frame {_currentFrameIndex} bitmap set and invalidated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting bitmap: {ex.Message}");
            }
        }, DispatcherPriority.Render);
    }

    private void UpdateProgress()
    {
        if (_progressSlider == null || _frames == null || _frames.Count == 0) return;

        var progress = (double)_currentFrameIndex / (_frames.Count - 1) * 100;
        _progressSlider.Value = progress;
    }

    private double GetCurrentSpeedMultiplier()
    {
        var selectedIndex = _speedComboBox?.SelectedIndex ?? 2;
        if (selectedIndex >= 0 && selectedIndex < _speedMultipliers.Length)
        {
            return _speedMultipliers[selectedIndex];
        }
        return 1.0;
    }

    private void UpdateUI()
    {
        var hasAnimation = _frames != null && _frames.Count > 0;
        
        if (_playPauseButton != null)
        {
            _playPauseButton.Content = _isPlaying ? "Pause" : "Play";
            _playPauseButton.IsEnabled = hasAnimation;
        }

        if (_progressSlider != null)
        {
            _progressSlider.IsEnabled = hasAnimation;
            if (hasAnimation)
            {
                _progressSlider.Maximum = 100;
                UpdateProgress();
            }
        }

        if (_speedComboBox != null)
        {
            _speedComboBox.IsEnabled = hasAnimation;
        }

        if (_loopCheckBox != null)
        {
            _loopCheckBox.IsEnabled = hasAnimation;
        }

        if (_audioCheckBox != null)
        {
            _audioCheckBox.IsEnabled = hasAnimation;
        }

        if (_noAnimationText != null)
        {
            _noAnimationText.IsVisible = !hasAnimation;
        }

        if (_bitmapControl != null)
        {
            if (hasAnimation)
            {
                DisplayCurrentFrame();
            }
            else
            {
                _bitmapControl.Bitmap = null;
            }
        }
    }
    
    private void PlayFrameAudio()
    {
        if (!_audioEnabled || _frames == null || _currentFrameIndex >= _frames.Count || _acsFile == null)
            return;
            
        try
        {
            // Get the current animation frame from the original model
            var animation = Animation;
            if (animation == null || _currentFrameIndex >= animation.Frames.Count)
                return;
                
            var currentAcsFrame = animation.Frames[_currentFrameIndex];
            if (currentAcsFrame.AudioIndex == 0 || currentAcsFrame.AudioIndex > _acsFile.AudioFiles.Count)
                return;
                
            // Stop any currently playing audio
            StopCurrentAudio();
            
            // Get the audio data for this frame
            var audioIndex = currentAcsFrame.AudioIndex - 1; // AudioIndex is 1-based
            if (audioIndex < _acsFile.AudioFiles.Count)
            {
                var audioFile = _acsFile.AudioFiles[audioIndex];
                if (audioFile.AudioData != null && audioFile.AudioData.Length > 0)
                {
                    Console.WriteLine($"Playing audio for frame {_currentFrameIndex}, audio index {audioIndex}");
                    PlayAudioData(audioFile.AudioData);
                }
                else
                {
                    Console.WriteLine($"No audio data available for frame {_currentFrameIndex}, audio index {audioIndex}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing frame audio: {ex.Message}");
        }
    }
    
    private void PlayAudioData(byte[] audioData)
    {
        try
        {
            // For now, we'll log audio playback instead of actually playing it
            // This can be extended with cross-platform audio libraries later
            Console.WriteLine($"Audio playback requested: {audioData.Length} bytes");
            
            // TODO: Implement cross-platform audio playback
            // Options:
            // 1. NAudio for Windows
            // 2. AVFoundation for macOS  
            // 3. ALSA/PulseAudio for Linux
            // 4. OpenAL cross-platform
            // 5. Use Process.Start to call system audio player
            
            // Save audio file for external playback (optional)
            if (audioData.Length > 0)
            {
                var tempFile = Path.Combine(Path.GetTempPath(), $"acs_audio_{DateTime.Now.Ticks}.wav");
                File.WriteAllBytes(tempFile, audioData);
                Console.WriteLine($"Audio saved to: {tempFile}");
                
                // Try to play using system default player (cross-platform)
                TryPlayWithSystemPlayer(tempFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing audio data: {ex.Message}");
        }
    }
    
    private void TryPlayWithSystemPlayer(string audioFile)
    {
        try
        {
            // Use system default audio player
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = audioFile,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            
            System.Diagnostics.Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not play audio with system player: {ex.Message}");
        }
    }
    
    private void StopCurrentAudio()
    {
        try
        {
            Console.WriteLine("Audio stopped");
            // TODO: Implement audio stopping for cross-platform solution
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping audio: {ex.Message}");
        }
    }
    
    public void SetAudioEnabled(bool enabled)
    {
        _audioEnabled = enabled;
        if (!enabled)
        {
            StopCurrentAudio();
        }
    }
}