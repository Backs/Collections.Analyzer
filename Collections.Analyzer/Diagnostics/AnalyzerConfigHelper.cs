using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics;

internal static class AnalyzerConfigHelper
{
    private static readonly ConditionalWeakTable<SyntaxTree, ConcurrentDictionary<Type, object>> ConfigCache = new();
    private static readonly char[] Separators = { ',' };

    public static T GetConfig<T>(AnalyzerOptions options, SyntaxTree syntaxTree, Func<AnalyzerConfigOptions, T> factory)
    {
        var typeConfigs = ConfigCache.GetValue(syntaxTree, _ => new ConcurrentDictionary<Type, object>());
        return (T)typeConfigs.GetOrAdd(typeof(T), _ => factory(options.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree))!);
    }

    public static string[] GetList(AnalyzerConfigOptions options, string optionName, string[] defaultValues)
    {
        return options.TryGetValue(optionName, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.Split(Separators, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray()
            : defaultValues;
    }
}
