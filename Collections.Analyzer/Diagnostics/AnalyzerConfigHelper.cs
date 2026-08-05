using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics;

internal static class AnalyzerConfigHelper
{
    private static readonly ConditionalWeakTable<SyntaxTree, object> ConfigCache = new();

    public static T GetConfig<T>(AnalyzerOptions options, SyntaxTree syntaxTree, System.Func<AnalyzerConfigOptions, T> factory)
    {
        if (ConfigCache.TryGetValue(syntaxTree, out var cached) && cached is T typedConfig)
        {
            return typedConfig;
        }

        var analyzerOptions = options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        var config = factory(analyzerOptions);
        
        ConfigCache.Remove(syntaxTree);
        ConfigCache.Add(syntaxTree, config!);
        
        return config;
    }
}
