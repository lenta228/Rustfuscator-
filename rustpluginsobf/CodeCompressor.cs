using System;
using System.Collections.Generic;
using System.Linq;

namespace RustPluginObfuscator
{
    public static class CodeCompressor
    {
        public static string CompressCode(string code)
        {
            string[] lines = code.Split(new[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var processedLines = new List<string>();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Пропускаем пустые строки и комментарии
                if (ShouldSkipLine(trimmedLine))
                {
                    continue;
                }

                // Удаляем комментарии в конце строки
                string cleanLine = RemoveInlineComments(trimmedLine);
                
                // Добавляем строку, если она не пустая после очистки
                if (!string.IsNullOrWhiteSpace(cleanLine))
                {
                    processedLines.Add(cleanLine);
                }
            }

            // Собираем код обратно с переносами строк
            return string.Join(Environment.NewLine, processedLines);
        }

        private static bool ShouldSkipLine(string line)
        {
            return string.IsNullOrWhiteSpace(line) ||
                   line.StartsWith("//") ||
                   line.StartsWith("/*") ||
                   line.StartsWith("*") ||
                   line.StartsWith("*/");
        }

        private static string RemoveInlineComments(string line)
        {
            // Удаляем комментарии в конце строки (//)
            int commentIndex = line.IndexOf("//");
            if (commentIndex >= 0)
            {
                line = line.Substring(0, commentIndex);
            }

            // Удаляем многострочные комментарии (/* */)
            int startComment = line.IndexOf("/*");
            if (startComment >= 0)
            {
                int endComment = line.IndexOf("*/", startComment);
                if (endComment >= 0)
                {
                    line = line.Substring(0, startComment) + line.Substring(endComment + 2);
                }
                else
                {
                    line = line.Substring(0, startComment);
                }
            }

            return line.Trim();
        }
    }
} 