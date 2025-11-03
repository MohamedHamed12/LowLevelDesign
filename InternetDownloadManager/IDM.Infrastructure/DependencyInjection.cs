using IDM.Core.Interfaces;
using IDM.Infrastructure.Configuration;
using IDM.Infrastructure.Services;
using IDM.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IDM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDownloadManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DownloadConfiguration>(configuration.GetSection(DownloadConfiguration.SectionName));

        services.AddHttpClient();

        services.AddSingleton<IDownloadQueue, DownloadQueue>();
        services.AddSingleton<IStorageManager, StorageManager>();
        services.AddSingleton<IProgressTracker, ProgressTracker>();
        services.AddSingleton<ISegmentDownloader, SegmentDownloader>();
        services.AddSingleton<IDownloadEngine, DownloadEngine>();
        services.AddSingleton<IDownloadManager>(sp =>
        {
            var config = configuration.GetSection(DownloadConfiguration.SectionName).Get<DownloadConfiguration>();
            return new DownloadManager(
                sp.GetRequiredService<IDownloadQueue>(),
                sp.GetRequiredService<IDownloadEngine>(),
                sp.GetRequiredService<IStorageManager>(),
                sp.GetRequiredService<ILogger<DownloadManager>>(),
                config?.MaxConcurrentDownloads ?? 3);
        });

        return services;
    }
}
