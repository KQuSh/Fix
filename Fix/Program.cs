using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace DFix
{
    internal class Program
    {
        private bool starte = false;

        private static bool isProcessing = false;

        static void Main()
        {


            #region Запуск с правами администратора
            if (!IsRunAsAdministrator())
            {
                RestartAsAdministrator();
                return;
            }
            #endregion


            #region Текст
            Console.WriteLine("");
            DisplayAnimation();
            Main2();
        }

        static void Main2()
        {
            string applicationName = "winws";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "cfg", "SVCInst.cmd");//СТАРТ
            string filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "cfg", "SVCRem.cmd");//СТОП
            #region Меню выбора
            string[] stroki = {
            "ЗАПУСТИТЬ FIX",
            "ОСТАНОВИТЬ FIX",
            };

            int strokiSelection = 0;
            #endregion

            while (true)
            {
                Console.Clear();
                Menu();
                DisplayMenu(stroki, strokiSelection);

                if (isProcessing)
                {
                    Thread.Sleep(100);
                    continue;
                }


                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.UpArrow)
                {
                    strokiSelection--;
                    if (strokiSelection < 0) strokiSelection = stroki.Length - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    strokiSelection++;
                    if (strokiSelection >= stroki.Length) strokiSelection = 0;
                }
                else if (key == ConsoleKey.Enter)
                {
                    if (strokiSelection == 1)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(" ОСТАНОВКА FIX");
                        LoadingAnimation();
                        isProcessing = true;
                        StopApplication(filePath1);
                        isProcessing = false;
                        Console.WriteLine("\n Fix остановлен. Для продолжения нажмите любую клавишу...");
                        Console.ReadKey();
                        Main2();
                        Console.ReadKey();
                        break;
                    }
                    else if (strokiSelection == 0)
                    {
                        if (IsApplicationRunning(applicationName))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("");
                            Console.WriteLine(" FIX УЖЕ ЗАПУЩЕН");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine(" ЗАПУСК FIX");
                            isProcessing = true;
                            StartApplication(filePath, applicationName);
                            isProcessing = false;
                            Console.WriteLine("\n Fix запущен. Для продолжения нажмите любую клавишу...");
                            Console.ReadKey();
                            Main2();
                        }
                        Console.ReadKey();
                        break;
                    }
                }
            }
            Console.ReadKey();
        }


        #region Проверка запущено ли приложение
        static void Winws()
        {
            string applicationName = "winws";

            if (IsApplicationRunning(applicationName))
            {

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" Fix запущен.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Fix не запущен.");
                Console.ResetColor();
            }
        }
        static bool IsApplicationRunning(string applicationName)
        {
            Process[] processes = Process.GetProcessesByName(applicationName);
            return processes.Length > 0;
        }
        #endregion

        #region Анимация
        static void LoadingAnimation()
        {
            string message = " Загрузка...";

            Console.ForegroundColor = ConsoleColor.Cyan;

            foreach (char letter in message)
            {
                Console.Write($"{letter}");
                Thread.Sleep(150);
            }
            Console.WriteLine();

            Console.ResetColor();
        }
        static void DisplayAnimation()
        {
            string message = " ДОБРО ПОЖАЛОВАТЬ В FIX!";

            Console.ForegroundColor = ConsoleColor.Cyan;

            foreach (char letter in message)
            {
                Console.Write(letter);
                Thread.Sleep(70);
            }

            Console.WriteLine();

            Console.ResetColor();
        }
        #endregion

        static void Menu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            DisplayMessage("");
            DisplayMessage("ДОБРО ПОЖАЛОВАТЬ В FIX!");
            DisplayMessage("");
            Console.ResetColor();

            DisplayMessage("\n Fix — это приложение, которое улучшает ваше взаимодействие с двумя популярными сервисами: YouTube и Discord.");
            DisplayMessage("Оно предлагает простые и эффективные способы ускорить работу YouTube и помогает обходить блокировки и ограничения,");
            DisplayMessage("мешающие нормальному использованию Discord.");
            DisplayMessage("");
            Winws();
            DisplayMessage("");
            DisplayMessage("\n В этом приложении можно выбирать действия с помощью стрелок ↑ и ↓ на клавиатуре.");
            DisplayMessage("Выберите нужную опцию и нажмите Enter для активации.");
            DisplayMessage("================================================");

        }
        static void DisplayMessage(string message)
        {
            Console.WriteLine(" " + message);
        }
        #endregion

        #region Меню выбора
        static void DisplayMenu(string[] options, int currentSelection)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (i == currentSelection)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($" > {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"   {options[i]}");
                }
            }
        }
        #endregion

        #region Запуск с правами администратора
        static bool IsRunAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        static void RestartAsAdministrator()
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = exePath,
                    Verb = "runas",
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Запуск и остановка приложения
        static void StartApplication(string filePath, string applicationName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(filePath)
            {
                Verb = "runas", // Запуск от имени администратора
                UseShellExecute = true
            };

            try
            {
                Process process = Process.Start(startInfo);


                LoadingAnimation();

                if (IsApplicationRunning(applicationName))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" Fix успешно запущен!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Произошла ошибка. Нажмите «ОСТАНОВИТЬ FIX», затем снова выберите «ЗАПУСТИТЬ FIX».");
                    Console.WriteLine(" В статусе должно быть указано «Fix запущен».");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Произошла ошибка при запуске: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void StopApplication(string filePath1)
        {
            ProcessStartInfo startInfo1 = new ProcessStartInfo(filePath1)
            {
                Verb = "runas",
                UseShellExecute = true
            };
            Process.Start(startInfo1);
        }
        #endregion
    }
}
