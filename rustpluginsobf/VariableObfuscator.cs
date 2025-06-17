using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace RustPluginObfuscator
{
    public static class VariableObfuscator
    {
        private static readonly Dictionary<string, string> variableNames = new Dictionary<string, string>();
        private static readonly Random random = new Random();
        private static readonly HashSet<string> reservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Поля Oxide / часто используемые параметры
            "config","permission","player","oven","fuel","burnable","entity","server","timer","rate","charcoal"
        };

        public static string ObfuscateVariables(string code)
        {
            variableNames.Clear();

            // Соберём имена параметров методов, чтобы исключить их из обфускации
            foreach (Match methodMatch in Regex.Matches(code, @"[a-zA-Z0-9_<>]+\s+[a-zA-Z0-9_<>]+\s*\(([^)]*)\)", RegexOptions.Multiline))
            {
                string paramList = methodMatch.Groups[1].Value;
                foreach (Match param in Regex.Matches(paramList, @"[a-zA-Z0-9_<>\[\]]+\s+(?<name>[a-zA-Z_][a-zA-Z0-9_]*)"))
                {
                    reservedNames.Add(param.Groups["name"].Value);
                }
            }

            // Находим простые объявления локальных переменных и обфусцируем их
            return Regex.Replace(code, @"\b(string|int|float|double|bool|var)\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=", m =>
            {
                string type = m.Groups[1].Value;
                string varName = m.Groups[2].Value;

                // Пропускаем системные или зарезервированные имена
                if (ShouldSkipVariable(varName))
                {
                    return m.Value;
                }

                if (!variableNames.ContainsKey(varName))
                {
                    // Генерируем Unicode имя переменной
                    string obfuscatedName = string.Concat(varName.Select(c => $"\\u{(int)c:X4}"));
                    variableNames[varName] = obfuscatedName;
                }

                return $"{type} {variableNames[varName]} =";
            });
        }

        public static string ReplaceVariableUsage(string code)
        {
            // Заменяем использование переменных
            foreach (var kvp in variableNames)
            {
                code = Regex.Replace(code, $@"\b{kvp.Key}\b", kvp.Value);
            }
            return code;
        }

        private static bool ShouldSkipVariable(string varName)
        {
            return reservedNames.Contains(varName) ||
                   varName.StartsWith("_") ||
                   varName.Length <= 2;
        }
    }
} 