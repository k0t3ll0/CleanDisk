
using System;
using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;


namespace CleanDisk
{
    internal class Program
    {
        static string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        static string virusDirectoryPath = $"C:\\Users\\{user}\\AppData\\Roaming\\WindowsServices";
        static string roamingPath = $"C:\\Users\\{user}\\AppData\\Roaming\\";
        static string path;
        //C:\Users\Kotello\AppData\Roaming
        static void Main(string[] args)
        {

            Unhide(user);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Всё проверено и сделано успешно!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Big thanks K0T3LL0!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Нажмите любую клавишу чтобы закрыть.");
            Console.ReadKey();
        }

        static void Unhide(string user)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c cd {roamingPath} & attrib -r -a -s -h /d /s";
            if (!Directory.Exists(roamingPath))
                Console.WriteLine(false);
            else
                process.Start();
            DeletingVirus(user);
            DeletingVirusFromFlash();
            GetFilesBack(path);
        }


        static void DeletingVirus(string user = "")
        {

            Process[] procs = Process.GetProcessesByName("wscript");
            foreach (Process proc in procs)
                if (proc.ProcessName == "wscript")
                {
                    Console.WriteLine("Убил");
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

        }
        static void DeletingVirusFromFlash()
        {
        Link:
            Console.WriteLine("Укажите путь к флешке(пример E)");
            path = Console.ReadLine() + ":";
            string virusFlashPath = $"{path}\\WindowsServices";
        Link2:
            if (Directory.Exists(path))
            {
                Process.Start("cmd.exe", $"/c {path} && attrib -r -a -s -h /d /s");

                if (Directory.Exists($"{virusFlashPath}"))
                {
                    try
                    {

                        foreach (string files in Directory.GetFiles(virusFlashPath))
                            File.Delete(files);
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
            else
            {
                Console.WriteLine("Диск не найден. Попробуйте ещё раз");
                goto Link;
            }
        }
        static void GetFilesBack(string pathToFileMove)
        {
            DriveInfo[] info = DriveInfo.GetDrives();
            string name = info.First(x => x.IsReady && x.DriveType == DriveType.Removable).VolumeLabel;
            if (File.Exists(name + ".lnk"))
                File.Delete(pathToFileMove + "\\" + name + ".lnk");
            try
            {
                if (Directory.Exists($"{pathToFileMove}\\_"))
                    Directory.Move($"{pathToFileMove}\\_", $"{pathToFileMove}\\Мои Файлы");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
