using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

namespace McCreate.App.Extensions;

public static class AnsiConsoleExtension
{
    public static SelectionPrompt<T> UseDefaultStyles<T>(this SelectionPrompt<T> obj, Func<T, string> converter) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(obj);

        obj
            .PageSize(8)
            .HighlightStyle(new Style().Foreground(Color.DodgerBlue2))
            .Converter = converter;
        
        
        return obj;
    }
}