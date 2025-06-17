using System;

namespace RustPluginObfuscator
{
    class Program
    {
        // Настройки обфускации
        private static bool stringObfuscation = true;
        private static bool variableObfuscation = true;
        private static bool codeCompression = true;
        private static string pluginPath = "";

        static void Main(string[] args)
        {
            while (true)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        stringObfuscation = !stringObfuscation;
                        break;
                    case "2":
                        variableObfuscation = !variableObfuscation;
                        break;
                    case "3":
                        codeCompression = !codeCompression;
                        break;
                    case "4":
                        SetPluginPath();
                        break;
                    case "5":
                        StartObfuscation();
                        break;
                    case "0":
                        Console.WriteLine("Выход из программы...");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    НАСТРОЙКИ ОБФУСКАЦИИ                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"1. Обфускация строк: {(stringObfuscation ? "ВКЛЮЧЕНО" : "ОТКЛЮЧЕНО")}");
            Console.WriteLine($"2. Обфускация переменных: {(variableObfuscation ? "ВКЛЮЧЕНО" : "ОТКЛЮЧЕНО")}");
            Console.WriteLine($"3. Сжатие кода: {(codeCompression ? "ВКЛЮЧЕНО" : "ОТКЛЮЧЕНО")}");
            Console.WriteLine();
            Console.WriteLine($"4. Путь к плагину: {(string.IsNullOrEmpty(pluginPath) ? "НЕ УКАЗАН" : pluginPath)}");
            Console.WriteLine();
            Console.WriteLine("5. НАЧАТЬ ОБФУСКАЦИЮ");
            Console.WriteLine("0. Выход");
            Console.WriteLine();
            Console.Write("Выберите опцию: ");
        }

        static void SetPluginPath()
        {
            Console.Clear();
            Console.WriteLine("Введите путь к .cs файлу плагина:");
            string path = Console.ReadLine();
            
            if (!string.IsNullOrWhiteSpace(path))
            {
                pluginPath = path;
                Console.WriteLine("Путь установлен!");
            }
            else
            {
                Console.WriteLine("Путь не может быть пустым!");
            }
            
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void StartObfuscation()
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                Console.WriteLine("Сначала укажите путь к плагину (опция 4)!");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            try
            {
                Console.Clear();
                Console.WriteLine("Начинаем обфускацию...");
                Console.WriteLine($"Файл: {pluginPath}");
                Console.WriteLine();

                // Читаем исходный код
                string code = System.IO.File.ReadAllText(pluginPath);
                string obfuscatedCode = ObfuscateCode(code);

                // Создаем путь для выходного файла
                string outputPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(pluginPath), 
                    "obfuscated_" + System.IO.Path.GetFileName(pluginPath)
                );

                // Сохраняем обфусцированный код
                System.IO.File.WriteAllText(outputPath, obfuscatedCode);

                Console.WriteLine("✅ Обфускация завершена успешно!");
                Console.WriteLine($"📁 Файл сохранён: {outputPath}");
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        static string ObfuscateCode(string code)
        {
            string header = "// protected by Rustfuscator" + Environment.NewLine;

            // Применяем настройки обфускации
            if (stringObfuscation)
            {
                Console.WriteLine("🔤 Обфусцируем строки...");
                code = StringObfuscator.ObfuscateStrings(code);
            }

            if (variableObfuscation)
            {
                Console.WriteLine("📝 Обфусцируем переменные...");
                code = VariableObfuscator.ObfuscateVariables(code);
                code = VariableObfuscator.ReplaceVariableUsage(code);
            }

            if (codeCompression)
            {
                Console.WriteLine("🗜️ Сжимаем код...");
                code = CodeCompressor.CompressCode(code);
            }

            return header + code;
        }
    }
}
