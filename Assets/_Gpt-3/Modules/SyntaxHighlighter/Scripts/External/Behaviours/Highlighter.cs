using System.Text.RegularExpressions;

using Modules.SyntaxHighlighter.Internal.DataObjects;


namespace Modules.SyntaxHighlighter.External.Behaviours
{
    public static class Highlighter
    {
        public static readonly string[] Languages = {"json", "csharp", "python", "javascript", "java", "glsl", "bash"};
        const string NoMatchPattern = @"(?:(?!x)x)";


        public static string Highlight(string code, HighlightSettingsVo settings)
        {
            foreach (var lang in Languages)
            {
                if (code.Contains(lang))
                {
                    switch (lang)
                    {
                        case "csharp":
                            return Csharp(code, settings);
                        case "json":
                            return JsonHighlight(code, settings);
                        case "python":
                            return Python(code, settings);
                        case "javascript":
                            return JavaScript(code, settings);
                        case "java":
                            return Java(code, settings);
                        case "glsl":
                            return GLSL(code, settings);
                        case "bash":
                            return Bash(code, settings);
                    }
                }
            }

            return code;
        }

        static string Csharp(string code, HighlightSettingsVo settings)
        {
            const string keywords = @"\b(?:abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|var|virtual|void|volatile|while)\b";
            const string comments = @"(\/\/[^\n]*|\/\*[\s\S]*?\*\/)";
            const string strings = @"([""'])(?:\\\1|.)*?\1";
            const string properties = @"\b(?<=\.)([A-Za-z_][A-Za-z0-9_]*)(?=\s*=)";
            const string numbers = @"\b-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?[fFdD]?[uUlL]?[mM]?\b";
            const string booleansOrNull = @"\b(?:true|false|null)\b";

            return AssignCodeColours("csharp", code, keywords, comments, strings, properties, numbers, booleansOrNull, settings);
        }

        static string JsonHighlight(string code, HighlightSettingsVo settings)
        {
            const string properties = @"(?<=[""])(.*?)(?=[""]\s*:)";
            const string strings = @"([""'])(?:\\\1|.)*?\1";
            const string numbers = @"(?<=\s)-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?";
            const string booleansOrNull = @"\b(?:true|false|null)\b";

            return AssignCodeColours("json", code, NoMatchPattern, NoMatchPattern, strings, properties, numbers, booleansOrNull, settings);
        }

        static string JavaScript(string code, HighlightSettingsVo settings)
        {
            const string keywords = @"\b(?:abstract|arguments|await|boolean|break|byte|case|catch|char|class|const|continue|debugger|default|delete|do|double|else|enum|eval|export|extends|false|final|finally|float|for|function|goto|if|implements|import|in|instanceof|int|interface|let|long|native|new|null|package|private|protected|public|return|short|static|super|switch|synchronized|this|throw|throws|transient|true|try|typeof|var|void|volatile|while|with|yield)\b";
            const string comments = @"(\/\/[^\n]*|\/\*[\s\S]*?\*\/)";
            const string strings = @"([""'])(?:\\\1|.)*?\1";
            const string numbers = @"\b-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?[bBfFdDlL]?[mM]?\b";
            const string booleansOrNull = @"\b(?:true|false|null)\b";

            return AssignCodeColours("javascript", code, keywords, comments, strings, NoMatchPattern, numbers, booleansOrNull, settings);
        }

        static string Java(string code, HighlightSettingsVo settings)
        {
            const string keywords = @"\b(?:abstract|assert|boolean|break|byte|case|catch|char|class|const|continue|default|do|double|else|enum|extends|final|finally|float|for|goto|if|implements|import|instanceof|int|interface|long|native|new|null|package|private|protected|public|return|short|static|strictfp|super|switch|synchronized|this|throw|throws|transient|try|void|volatile|while)\b";
            const string comments = @"(\/\/[^\n]*|\/\*[\s\S]*?\*\/)";
            const string strings = @"([""'])(?:\\\1|.)*?\1";
            const string numbers = @"\b-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?[fFdD]?[lL]?[mM]?\b";
            const string booleansOrNull = @"\b(?:true|false|null)\b";

            return AssignCodeColours("java", code, keywords, comments, strings, NoMatchPattern, numbers, booleansOrNull, settings);
        }

        static string Python(string code, HighlightSettingsVo settings)
        {
            const string keywords = @"\b(?:and|as|assert|break|class|continue|def|del|elif|else|except|finally|for|from|global|if|import|in|is|lambda|nonlocal|not|or|pass|raise|return|try|while|with|yield)\b";
            const string comments = @"(?<!<color=)#.*";
            const string strings = @"([""'])(?:\\\1|.)*?\1";
            const string numbers = @"\b-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?[jJ]?\b";
            const string booleansOrNull = @"\b(?:True|False|None)\b";

            return AssignCodeColours("python", code, keywords, comments, strings, NoMatchPattern, numbers, booleansOrNull, settings);
        }

        static string GLSL(string code, HighlightSettingsVo settings)
        {
            const string keywords = @"\b(?:attribute|const|uniform|varying|break|continue|do|for|while|if|else|in|out|inout|float|int|void|bool|true|false|invariant|discard|return|mat2|mat3|mat4|vec2|vec3|vec4|ivec2|ivec3|ivec4|bvec2|bvec3|bvec4|sampler1D|sampler2D|sampler3D|samplerCube|sampler1DShadow|sampler2DShadow|struct)\b";
            const string comments = @"(\/\/[^\n]*|\/\*[\s\S]*?\*\/)";
            const string strings = @"([""'])(?:\\\1|.)*?\1";
            const string numbers = @"\b-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?\b";

            return AssignCodeColours("glsl", code, keywords, comments, strings, NoMatchPattern, numbers, NoMatchPattern, settings);
        }

        static string Bash(string code, HighlightSettingsVo settings)
        {
            const string keywords = @"\b(?:alias|bg|break|case|cd|command|continue|do|done|elif|else|esac|eval|exec|exit|export|false|fg|for|function|getopts|hash|if|in|jobs|let|local|readonly|return|set|shift|test|then|time|trap|true|type|ulimit|umask|unalias|unset|until|wait|while)\b";
            const string comments = @"((?<!<color=)#[^\n]*)";
            const string strings = @"([""'])(?:\\\1|.)*?\1";
            const string numbers = @"\b-?(\d+(\.\d+)?|\.\d+)([eE][+-]?\d+)?\b";

            return AssignCodeColours("bash", code, keywords, comments, strings, NoMatchPattern, numbers, NoMatchPattern, settings);
        }

        static string AssignCodeColours (string lang, string code, string keys, string coms, string str, string props, string nums, string bools, HighlightSettingsVo settings)
        {
            code = code.Replace(lang, "").Trim();
            code = $"<color={settings.BaseColor}>{code}</color>";
            code = Regex.Replace(code, keys, m => $"<color={settings.KeywordColor}>{m.Value}</color>");
            code = Regex.Replace(code, coms, m => $"<color={settings.CommentColor}>{m.Value}</color>");
            code = Regex.Replace(code, str, m => $"<color={settings.StringColor}>{m.Value}</color>");
            code = Regex.Replace(code, props, m => $"<color={settings.PropertyNameColor}>{m.Value}</color>");
            code = Regex.Replace(code, nums, m => $"<color={settings.NumberColor}>{m.Value}</color>");
            code = Regex.Replace(code, bools, m => $"<color={settings.BooleanOrNullColor}>{m.Value}</color>");

            return code;
        }
    }
}

/*
python
[seeing markup here]# Python Code Block with Various Data Types

# Integer
    integer_var = 42

# Float
float_var = 3.14

# String
string_var = "Hello, World!"

# List
list_var = [1, 2, 3, 4, 5]

# Tuple
tuple_var = (6, 7, 8, 9, 10)

# Dictionary
dict_var = {"key1": "value1", "key2": "value2"}

# Boolean
bool_var = True

# Function
[seeing markup here]def greet(name):
[seeing markup here]return f"Hello, {name}!"

print(greet("SearchBot"))


*/