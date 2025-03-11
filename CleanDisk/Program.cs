using System;
using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;


namespace CleanDiskRework
{
    internal class Program
    {
#pragma warning disable CA1416 // Проверка совместимости платформы
        static string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
#pragma warning restore CA1416 // Проверка совместимости платформы
        static string virusDirectoryPath = $"C:\\Users\\{user}\\AppData\\Roaming\\WindowsServices";
        static string roamingPath = $"C:\\Users\\{user}\\AppData\\Roaming\\";
        static string path;
        static DriveInfo[] disks = DriveInfo.GetDrives();

        static async Task StartProgram(string user)
        {
            if (!Directory.Exists(roamingPath))
                Console.WriteLine(false);
            await DeletingVirus(user);
            await DeletingVirusFromFlash();
        }

        static Task DeletingVirus(string user = "")
        {

            Process[] procs = Process.GetProcessesByName("wscript");
            foreach (Process proc in procs)
                if (proc.ProcessName == "wscript")
                {
                    Console.WriteLine("Убил процесс вируса");
                    proc.Kill();
                }
            TryAgain:
            if (Directory.Exists(virusDirectoryPath))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(virusDirectoryPath))
                        File.Delete(file);
                    Directory.Delete(virusDirectoryPath);
                }
                catch (Exception)
                {
                    goto TryAgain;
                }
            }
            else
                Console.WriteLine("На компьютере нету вируса");
            return Task.CompletedTask;
        }

        static async Task DeletingVirusFromFlash()
        {
            var disk_letter = disks.Select(x => x.IsReady && x.DriveType == DriveType.Removable ? x.Name.Substring(0, 2) : null).ToList();
            disk_letter.RemoveAll(x => x == null);
            Console.WriteLine(string.Join(", ", disk_letter));
            foreach (var flash in disk_letter)
            {
                path = flash;
                string virusFlashPath = $"{path}\\WindowsServices";
            Link2:
                if (Directory.Exists(path))
                    GetFilesBack(path);
                if (Directory.Exists($"{virusFlashPath}"))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(virusFlashPath);
                        foreach (string file in files)
                            File.Delete(file);
                        Directory.Delete(virusFlashPath);
                    }
                    catch (Exception)
                    {
                        goto Link2;
                    }
                }
                else
                {
                    Console.WriteLine("Папка с вирусом не найдена!");
                }
            }
            await Task.CompletedTask;

        }

        static void GetFilesBack(string pathToFileMove)
        {
            string filename = "";
            string name = disks.Select(x => path == x.Name.Substring(0, 2) ? x.VolumeLabel : null).SkipWhile(x => x == null).ToArray()[0];
            Console.WriteLine(pathToFileMove + "\\" + $"{name}.lnk");
            if (File.Exists($"{pathToFileMove}\\{name}.lnk"))
            {
                filename = Path.GetFileName(pathToFileMove + name + ".lnk");
                Console.WriteLine(filename);
                File.Delete($"{path}{filename}");
            }
            else if (File.Exists($"{pathToFileMove}\\{name} ({pathToFileMove[0]}).lnk"))
            {
                filename = Path.GetFileName($"{pathToFileMove}\\{name} ({pathToFileMove[0]}).lnk");
                Console.WriteLine(filename);
                File.Delete($"{pathToFileMove}{filename}");
            }
        
            try
            {
                string targetDir = $@"{pathToFileMove}_";
                string destDir = $@"{pathToFileMove}test";
                if (Directory.Exists(targetDir))
                {
                    Directory.Move(targetDir, destDir);
                    FileAttributes attributes = File.GetAttributes(destDir);

                    if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        // Show the file.
                        attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                        File.SetAttributes(destDir, attributes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        static async Task Main(string[] args)
        {
            await StartProgram(user);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Всё проверено и сделано успешно!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Big thanks K0T3LL0!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Нажмите любую клавишу чтобы закрыть.");
            Console.ReadKey();
        }

    }
}