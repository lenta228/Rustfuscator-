using System;
using System.IO;

namespace RustPluginObfuscator
{
    public static class ObfuscationEngine
    {
        public static string ObfuscateCode(string code)
        {
            // Вставим обычный комментарий
            string header = "// protected by Rustfuscator" + Environment.NewLine;

            // 1. Обфусцируем строки
            code = StringObfuscator.ObfuscateStrings(code);

            // 2. Обфусцируем переменные
            code = VariableObfuscator.ObfuscateVariables(code);
            code = VariableObfuscator.ReplaceVariableUsage(code);

            // 3. Сжимаем код
            code = CodeCompressor.CompressCode(code);

            return header + code;
        }

        public static void ProcessFile(string inputPath, string outputPath = null)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("Файл не найден!", inputPath);
            }

            string code = File.ReadAllText(inputPath);
            string obfuscatedCode = ObfuscateCode(code);

            // Если путь вывода не указан, создаем автоматически
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Path.Combine(Path.GetDirectoryName(inputPath), "obfuscated_" + Path.GetFileName(inputPath));
            }

            File.WriteAllText(outputPath, obfuscatedCode);
        }
    }
} 