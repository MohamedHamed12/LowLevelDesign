using IDM.Core.Interfaces;
using IDM.Infrastructure;
using IDM.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false);

builder.Services.AddDownloadManager(builder.Configuration);
builder.Logging.AddConsole();

var host = builder.Build();

var downloadManager = host.Services.GetRequiredService<IDownloadManager>();

AnsiConsole.Write(new FigletText("IDM").Centered().Color(Color.Blue));
AnsiConsole.MarkupLine("[bold blue]Internet Download Manager[/] - .NET 9\n");

while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]What would you like to do?[/]")
            .PageSize(10)
            .AddChoices(new[]
            {
                "Add Download",
                "View Downloads",
                "Pause Download",
                "Resume Download",
                "Cancel Download",
                "Exit"
            }));

    switch (choice)
    {
        case "Add Download":
            await AddDownloadAsync(downloadManager);
            break;
        case "View Downloads":
            await ViewDownloadsAsync(downloadManager);
            break;
        case "Pause Download":
            await PauseDownloadAsync(downloadManager);
            break;
        case "Resume Download":
            await ResumeDownloadAsync(downloadManager);
            break;
        case "Cancel Download":
            await CancelDownloadAsync(downloadManager);
            break;
        case "Exit":
            AnsiConsole.MarkupLine("[yellow]Goodbye![/]");
            return;
    }

    AnsiConsole.WriteLine();
}

static async Task AddDownloadAsync(IDownloadManager manager)
{
    var url = AnsiConsole.Ask<string>("[cyan]Enter download URL:[/]");
    var destination = AnsiConsole.Ask<string>("[cyan]Enter destination path:[/]");

    var task = await manager.AddDownloadAsync(url, destination);
    
    task.ProgressChanged += (_, e) =>
    {
        // Progress updates handled in View Downloads
    };

    task.StatusChanged += (_, e) =>
    {
        AnsiConsole.MarkupLine($"[yellow]Download {e.TaskId}: {e.NewStatus}[/]");
    };

    AnsiConsole.MarkupLine($"[green]Download added with ID: {task.Id}[/]");
}

static async Task ViewDownloadsAsync(IDownloadManager manager)
{
    var downloads = await manager.GetAllDownloadsAsync();
    
    if (!downloads.Any())
    {
        AnsiConsole.MarkupLine("[yellow]No downloads found.[/]");
        return;
    }

    var table = new Table();
    table.AddColumn("ID");
    table.AddColumn("Status");
    table.AddColumn("Progress");
    table.AddColumn("Speed");
    table.AddColumn("ETA");
    table.AddColumn("URL");

    foreach (var download in downloads)
    {
        var statusColor = download.Status switch
        {
            IDM.Core.Enums.DownloadStatus.Completed => "green",
            IDM.Core.Enums.DownloadStatus.Downloading => "blue",
            IDM.Core.Enums.DownloadStatus.Failed => "red",
            IDM.Core.Enums.DownloadStatus.Cancelled => "grey",
            _ => "yellow"
        };

        table.AddRow(
            download.Id.ToString()[..8],
            $"[{statusColor}]{download.Status}[/]",
            $"{download.Progress:F2}%",
            $"{FormatBytes(download.Speed)}/s",
            FormatTimeSpan(download.EstimatedTimeRemaining),
            download.Url.Length > 40 ? download.Url[..40] + "..." : download.Url
        );
    }

    AnsiConsole.Write(table);
}

static async Task PauseDownloadAsync(IDownloadManager manager)
{
    var downloads = await manager.GetActiveDownloadsAsync();
    if (!downloads.Any())
    {
        AnsiConsole.MarkupLine("[yellow]No active downloads to pause.[/]");
        return;
    }

    var choices = downloads.Select(d => $"{d.Id.ToString()[..8]} - {d.Url}").ToList();
    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[cyan]Select download to pause:[/]")
            .AddChoices(choices));

    var id = Guid.Parse(selected.Split(" - ")[0]);
    await manager.PauseDownloadAsync(id);
    AnsiConsole.MarkupLine("[green]Download paused.[/]");
}

static async Task ResumeDownloadAsync(IDownloadManager manager)
{
    var downloads = (await manager.GetAllDownloadsAsync())
        .Where(d => d.Status == IDM.Core.Enums.DownloadStatus.Paused);
    
    if (!downloads.Any())
    {
        AnsiConsole.MarkupLine("[yellow]No paused downloads to resume.[/]");
        return;
    }

    var choices = downloads.Select(d => $"{d.Id.ToString()[..8]} - {d.Url}").ToList();
    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[cyan]Select download to resume:[/]")
            .AddChoices(choices));

    var id = Guid.Parse(selected.Split(" - ")[0]);
    await manager.ResumeDownloadAsync(id);
    AnsiConsole.MarkupLine("[green]Download resumed.[/]");
}

static async Task CancelDownloadAsync(IDownloadManager manager)
{
    var downloads = await manager.GetAllDownloadsAsync();
    if (!downloads.Any())
    {
        AnsiConsole.MarkupLine("[yellow]No downloads to cancel.[/]");
        return;
    }

    var choices = downloads.Select(d => $"{d.Id.ToString()[..8]} - {d.Url}").ToList();
    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[cyan]Select download to cancel:[/]")
            .AddChoices(choices));

    var id = Guid.Parse(selected.Split(" - ")[0]);
    await manager.CancelDownloadAsync(id);
    AnsiConsole.MarkupLine("[green]Download cancelled.[/]");
}

static string FormatBytes(double bytes)
{
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    int order = 0;
    while (bytes >= 1024 && order < sizes.Length - 1)
    {
        order++;
        bytes /= 1024;
    }
    return $"{bytes:0.##} {sizes[order]}";
}

static string FormatTimeSpan(TimeSpan ts)
{
    if (ts == TimeSpan.MaxValue)
        return "--:--:--";
    
    return ts.TotalHours >= 1 
        ? $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}"
        : $"{ts.Minutes:D2}:{ts.Seconds:D2}";
}
