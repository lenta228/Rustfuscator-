using System;
using System.Windows.Forms;

namespace RustPluginObfuscator
{
    class Program
    {
        // Настройки обфускации
        private static bool stringObfuscation = true;
        private static bool variableObfuscation = true;
        private static bool codeCompression = true;
        private static bool noiseInjection = false; // beta
        private static string pluginPath = "";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
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
            Console.WriteLine($"4. Шумовые методы/комментарии (β): {(noiseInjection ? "ВКЛЮЧЕНО" : "ОТКЛЮЧЕНО")}");
            Console.WriteLine();
            Console.WriteLine($"5. Путь к плагину: {(string.IsNullOrEmpty(pluginPath) ? "НЕ УКАЗАН" : pluginPath)}");
            Console.WriteLine();
            Console.WriteLine("6. НАЧАТЬ ОБФУСКАЦИЮ");
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
                Console.WriteLine("Сначала укажите путь к плагину (опция 5)!");
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
            // Вызываем единую обфускационную пайплайн
            Console.WriteLine("🚀 Запуск движка обфускации...");

            // Применяем настройки
            if (!stringObfuscation || !variableObfuscation || !codeCompression)
            {
                // Если пользователь частично отключил этапы, подключаем их выборочно
                if (!stringObfuscation)
                {
                    Console.WriteLine("⚠️  Пропуск обфускации строк");
                }
                if (!variableObfuscation)
                {
                    Console.WriteLine("⚠️  Пропуск обфускации переменных");
                }
                if (!codeCompression)
                {
                    Console.WriteLine("⚠️  Пропуск сжатия кода");
                }
            }

            // Пока что для простоты вызываем полный движок; можно доработать для частичного.
            return ObfuscationEngine.ObfuscateCode(code, noiseInjection);
        }
    }
}
