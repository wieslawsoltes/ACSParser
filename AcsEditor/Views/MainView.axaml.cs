using System;
using System.Linq;
using AcsEditor.Models;
using AcsEditor.Services;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace AcsEditor.Views;

public partial class MainView : UserControl
{
    private AcsFile? _currentFile;
    private readonly McpServerHost _mcpServerHost;

    public MainView()
    {
        InitializeComponent();
        _mcpServerHost = new McpServerHost();

        // Set default path for testing
        if (OperatingSystem.IsWindows())
        {
            PathTextBox.Text = @"c:\path\to\character.acs";
        }
        else if (OperatingSystem.IsMacOS())
        {
            PathTextBox.Text = "../clippitMS/CLIPPIT.ACS";
        }

        // Initialize MCP server
        _ = InitializeMcpServerAsync();
    }

    private async System.Threading.Tasks.Task InitializeMcpServerAsync()
    {
        try
        {
            await _mcpServerHost.StartAsync();
            ShowSuccess("MCP server started successfully. You can now use AI tools to interact with animations.");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to start MCP server: {ex.Message}");
        }
    }

    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open ACS File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("ACS Files")
                {
                    Patterns = new[] { "*.acs" }
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count > 0)
        {
            PathTextBox.Text = files[0].Path.LocalPath;
        }
    }

    private void LoadButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var path = PathTextBox.Text;
        if (string.IsNullOrWhiteSpace(path))
        {
            ShowError("Please select an ACS file first.");
            return;
        }

        try
        {
            _currentFile = AcsLoader.Load(path);
            if (_currentFile != null)
            {
                DataContext = _currentFile;
                ShowSuccess($"Loaded ACS file: {_currentFile.Name}");
                ShowSuccess($"Animations: {_currentFile.Animations.Count}, Images: {_currentFile.Images.Count}, Audio: {_currentFile.AudioFiles.Count}");
                
                // Update MCP service with the new file and selection callback
                _mcpServerHost.UpdateContext(_currentFile, OnAnimationSelectedFromMcp);
                ShowSuccess("MCP service updated with new file context. AI tools can now access animations.");
                
                // Auto-select the first animation for better user experience
                if (_currentFile.Animations.Count > 0)
                {
                    var firstAnimation = _currentFile.Animations[0];
                    NavigationTreeView.SelectedItem = firstAnimation;
                    ContentDisplayControl.Content = firstAnimation;
                    ShowSuccess($"Auto-selected first animation: {firstAnimation.Name}");
                }
            }
            else
            {
                ShowError("Failed to load ACS file. Please check the file format.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error loading file: {ex.Message}");
        }
    }

    /// <summary>
    /// Callback method for when an animation is selected via MCP tools
    /// </summary>
    private void OnAnimationSelectedFromMcp(AcsAnimation animation)
    {
        // Update the UI to reflect the MCP-selected animation
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            NavigationTreeView.SelectedItem = animation;
            ContentDisplayControl.Content = animation;
            ShowSuccess($"Animation selected via MCP: {animation.Name ?? "Unnamed Animation"}");
        });
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1)
        {
            var selectedItem = e.AddedItems[0];
            
            // Handle TreeViewItem selection (for folder nodes)
            if (selectedItem is TreeViewItem treeViewItem)
            {
                ContentDisplayControl.Content = treeViewItem.DataContext;
            }
            // Handle direct model object selection
            else if (selectedItem is AcsBase)
            {
                ContentDisplayControl.Content = selectedItem;
            }
        }
        else
        {
            ContentDisplayControl.Content = null;
        }
    }

    private void ShowError(string message)
    {
        // TODO: Implement proper error display (status bar, message box, etc.)
        Console.WriteLine($"Error: {message}");
    }

    private void ShowSuccess(string message)
    {
        // TODO: Implement proper success message display
        Console.WriteLine($"Success: {message}");
    }

    /// <summary>
    /// Cleanup method to properly dispose of the MCP server
    /// </summary>
    public async System.Threading.Tasks.Task DisposeAsync()
    {
        try
        {
            await _mcpServerHost.StopAsync();
            ShowSuccess("MCP server stopped successfully.");
        }
        catch (Exception ex)
        {
            ShowError($"Error stopping MCP server: {ex.Message}");
        }
    }
}