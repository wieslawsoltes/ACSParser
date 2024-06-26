using System;
using ACDParser.Model;
using ACDParser.Services;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ACDParser.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        if (OperatingSystem.IsWindows())
        {
            PathTextBox.Text = @"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT ACS Decompiled\CLIPPIT.acd";
        }

        if (OperatingSystem.IsMacOS())
        {
            PathTextBox.Text = "/Users/wieslawsoltes/GitHub/ACSParser/clippitMS/CLIPPIT ACS Decompiled/CLIPPIT.acd";
        }
    }

    private void ParseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var path = PathTextBox.Text;
        if (path is { })
        {
            var acd = AcdLoader.Load(path);
            if (acd is { })
            {
                DataContext = acd;
            }
        }
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1)
        {
            var item = e.AddedItems[0];
            if (item is TreeViewItem treeViewItem)
            {
                DefineContentControl.Content = treeViewItem.DataContext;
            }
            else if (item is AcdBase acdBase)
            {
                DefineContentControl.Content = acdBase;
            }
        }
        else
        {
            DefineContentControl.Content = null;
        }
    }
}

