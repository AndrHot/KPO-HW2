using Microsoft.Extensions.DependencyInjection;
using Pos.Services;

namespace Pos;

public static class DependencyInjection
{
    public static IServiceCollection AddPos(this IServiceCollection services) => services
        .AddSingleton<IJournalRepository, MemoryRepository>()
        .AddSingleton<IResultsWriter, CsvWriter>()
        .AddSingleton<Journal>();
}
