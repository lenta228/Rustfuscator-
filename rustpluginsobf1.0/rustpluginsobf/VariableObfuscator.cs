using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RustPluginObfuscator
{
    public static class VariableObfuscator
    {
        private static readonly Dictionary<string, string> variableNames = new Dictionary<string, string>();
        private static readonly Random random = new Random();

        public static string ObfuscateVariables(string code)
        {
            variableNames.Clear();

            // Находим простые объявления переменных и обфусцируем их
            return Regex.Replace(code, @"\b(string|int|float|bool|var)\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=", m =>
            {
                string type = m.Groups[1].Value;
                string varName = m.Groups[2].Value;

                // Пропускаем системные имена
                if (ShouldSkipVariable(varName))
                {
                    return m.Value;
                }

                if (!variableNames.ContainsKey(varName))
                {
                    // Генерируем Unicode имя переменной
                    string obfuscatedName = "";
                    for (int i = 0; i < varName.Length; i++)
                    {
                        obfuscatedName += $"\\u{(int)varName[i]:X4}";
                    }
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
            return varName.StartsWith("_") || 
                   varName.Length <= 2 ||
                   varName == "player" || 
                   varName == "config" || 
                   varName == "data" ||
                   varName == "plugin" || 
                   varName == "container" || 
                   varName == "image";
        }
    }
} 