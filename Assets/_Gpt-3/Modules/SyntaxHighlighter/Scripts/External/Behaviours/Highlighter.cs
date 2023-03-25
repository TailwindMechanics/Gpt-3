using System.Text.RegularExpressions;

using Modules.SyntaxHighlighter.Internal.DataObjects;


namespace Modules.SyntaxHighlighter.External.Behaviours
{
    public static class Highlighter
    {
        public static string Highlight(string code, HighlightSettingsVo settings)
        {
            if (code.Contains("csharp"))
            {
                return Csharp(code, settings);
            }

            return code.Contains("json") ? JsonHighlight(code, settings) : code;
        }

        static string Csharp(string code, HighlightSettingsVo settings)
        {
            const string keywordsPattern = @"\b(?:abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|var|virtual|void|volatile|while)\b";
            const string commentsPattern = @"(\/\/[^\n]*|\/\*[\s\S]*?\*\/)";
            const string stringsPattern = @"([""'])(?:\\\1|.)*?\1";
            const string propertyNamesPattern = @"\b(?<=\.)([A-Za-z_][A-Za-z0-9_]*)(?=\s*=)";
            const string numbersPattern = @"\b-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?[fFdD]?[uUlL]?[mM]?\b";
            const string booleansOrNullPattern = @"\b(?:true|false|null)\b";

            code = code.Replace("csharp", "").Trim();
            code = $"<color={settings.BaseColor}>{code}</color>";
            code = Regex.Replace(code, keywordsPattern, m => $"<color={settings.KeywordColor}>{m.Value}</color>");
            code = Regex.Replace(code, commentsPattern, m => $"<color={settings.CommentColor}>{m.Value}</color>");
            code = Regex.Replace(code, stringsPattern, m => $"<color={settings.StringColor}>{m.Value}</color>");
            code = Regex.Replace(code, propertyNamesPattern, m => $"<color={settings.PropertyNameColor}>{m.Value}</color>");
            code = Regex.Replace(code, numbersPattern, m => $"<color={settings.NumberColor}>{m.Value}</color>");
            code = Regex.Replace(code, booleansOrNullPattern, m => $"<color={settings.BooleanOrNullColor}>{m.Value}</color>");

            return code;
        }

        static string JsonHighlight(string code, HighlightSettingsVo settings)
        {
            const string propertyNamePattern = @"(?<=[""])(.*?)(?=[""]\s*:)";
            const string stringPattern = @"([""'])(?:\\\1|.)*?\1";
            const string numberPattern = @"(?<=\s)-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?";
            const string booleanOrNullPattern = @"\b(?:true|false|null)\b";

            code = code.Replace("json", "").Trim();
            code = $"<color={settings.BaseColor}>{code}</color>";
            code = Regex.Replace(code, propertyNamePattern, m => $"<color={settings.PropertyNameColor}>{m.Value}</color>");
            code = Regex.Replace(code, stringPattern, m => $"<color={settings.StringColor}>{m.Value}</color>");
            code = Regex.Replace(code, numberPattern, m => $"<color={settings.NumberColor}>{m.Value}</color>");
            code = Regex.Replace(code, booleanOrNullPattern, m => $"<color={settings.BooleanOrNullColor}>{m.Value}</color>");

            return code;
        }
    }
}