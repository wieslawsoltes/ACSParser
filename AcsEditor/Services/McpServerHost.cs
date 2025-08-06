using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using AcsEditor.Models;

namespace AcsEditor.Services;

/// <summary>
/// Host for the MCP server that runs alongside the Avalonia application
/// </summary>
public class McpServerHost
{
    private IHost? _host;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _hostTask;

    /// <summary>
    /// Start the MCP server
    /// </summary>
    public async Task StartAsync()
    {
        if (_host != null)
        {
            return; // Already started
        }

        _cancellationTokenSource = new CancellationTokenSource();

        var builder = Host.CreateEmptyApplicationBuilder(settings: null);
        
        // Configure logging to stderr for MCP protocol compliance
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        _host = builder.Build();

        // Start the host in a background task
        _hostTask = _host.RunAsync(_cancellationTokenSource.Token);
        
        // Give the server a moment to initialize
        await Task.Delay(100);
    }

    /// <summary>
    /// Stop the MCP server
    /// </summary>
    public async Task StopAsync()
    {
        if (_host == null)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        
        if (_hostTask != null)
        {
            try
            {
                await _hostTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelling
            }
        }

        _host?.Dispose();
        _cancellationTokenSource?.Dispose();
        
        _host = null;
        _hostTask = null;
        _cancellationTokenSource = null;
    }

    /// <summary>
    /// Update the MCP service with the current file and selection callback
    /// </summary>
    public void UpdateContext(AcsFile? file, Action<AcsAnimation>? onAnimationSelected)
    {
        McpAnimationService.Initialize(file, onAnimationSelected);
    }

    /// <summary>
    /// Check if the MCP server is running
    /// </summary>
    public bool IsRunning => _host != null && _hostTask != null && !_hostTask.IsCompleted;
}