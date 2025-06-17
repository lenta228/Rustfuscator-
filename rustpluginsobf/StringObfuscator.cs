using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RustPluginObfuscator
{
    public static class StringObfuscator
    {
        public static string ObfuscateStrings(string code)
        {
            // Обфусцируем обычные строки, НО пропускаем $"" интерполяции
            return Regex.Replace(code, @"(?<!\$)""([^""]*)""", m =>
            {
                string original = m.Groups[1].Value;
                string obfuscated = string.Concat(original.Select(c => $"\\u{(int)c:X4}"));
                return $"\"{obfuscated}\"";
            });
        }
    }
} 