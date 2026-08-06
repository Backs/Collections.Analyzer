using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics;

internal static class AnalyzerConfigHelper
{
    private static readonly ConditionalWeakTable<SyntaxTree, object> ConfigCache = new();
    private static readonly char[] Separators = { ',' };

    public static T GetConfig<T>(AnalyzerOptions options, SyntaxTree syntaxTree, Func<AnalyzerConfigOptions, T> factory)
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

    public static string[] GetList(AnalyzerConfigOptions options, string optionName, string[] defaultValues)
    {
        return options.TryGetValue(optionName, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.Split(Separators, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray()
            : defaultValues;
    }
}
