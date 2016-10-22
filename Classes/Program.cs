using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using AudioSwitch.CoreAudioApi;


namespace AudioSwitch.Classes
{
    internal static class Program
    {
        static readonly Mutex mutex = new Mutex(true, "{579A9A19-7AE5-42CD-8147-E587F5C9DD50}");
        internal static string Root;
        internal static readonly string AppDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\AudioSwitch\\";

        internal static Settings settings;

        [STAThread]
        private static void Main(string[] args)
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            Root = Path.GetDirectoryName(path) + "\\";

            if (!Directory.Exists(AppDataRoot))
                Directory.CreateDirectory(AppDataRoot);

            try { 
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
                settings = Settings.Load();

                if (args.Length > 0)
                {
                    var rType = settings.DefaultDataFlow;

                    for (var index = 0; index < args.Length; index++)
                    {
                        var arg = args[index];
                        if (string.IsNullOrWhiteSpace(arg))
                            continue;

                        switch (arg.Substring(1, arg.Length - 1))
                        {
                            case "help":
                            case "h":
                            case "?":
                                Console.WriteLine();
                                Console.WriteLine("AudioSwitch v2.1 command-line help");
                                Console.WriteLine("----------------------------------");
                                Console.WriteLine("Available commands:");
                                Console.WriteLine("  /i - switch to input devices for the command. Default taken from settings.");
                                Console.WriteLine("  /o - switch to output devices for the command. Default taken from settings.");
                                Console.WriteLine();
                                Console.WriteLine("  /l - list all available devices of the selected type, < > is current default.");
                                Console.WriteLine("  /s - set a device as default: number for index, string for name.");
                                Console.WriteLine();
                                break;

                            case "i":
                                rType = EDataFlow.eCapture;
                                Console.WriteLine("Device group changed to input devices.");
                                break;

                            case "o":
                                rType = EDataFlow.eRender;
                                Console.WriteLine("Device group changed to output devices.");
                                break;

                            case "l":
                                Console.WriteLine();
                                Console.WriteLine(" Devices available:");
                                EndPoints.RefreshDeviceList(rType);

                                var i = 0;
                                foreach (var device in EndPoints.DeviceNames.Values)
                                    Console.WriteLine(device == EndPoints.DefaultDeviceName ? " <{0}> {1}" : "  {0}  {1}", i++, device);
                                Console.WriteLine();
                                break;

                            case "s":
                                if (++index < args.Length)
                                {
                                    int devID;
                                    if (int.TryParse(args[index], out devID))
                                    {
                                        EndPoints.RefreshDeviceList(rType);
                                        if (devID <= EndPoints.DeviceNames.Count - 1)
                                        {
                                            EndPoints.SetDefaultDeviceByID(devID);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error changing device!");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        EndPoints.RefreshDeviceList(rType);
                                        if (!EndPoints.SetDefaultDeviceByName(args[index]))
                                        {
                                            Console.WriteLine("Error changing device!");
                                            return;
                                        }
                                    }
                                    Console.WriteLine("Device changed to \"" + EndPoints.DefaultDeviceName + "\"");
                                }
                                else
                                {
                                    Console.WriteLine("Error changing device - s option requires a string or number");
                                    return;
                                }
                                break;

                        }
                    }
                    return;
                }
            }
            catch
            {
                return;
            }
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = (Exception)e.ExceptionObject;
                using (var w = File.AppendText(AppDataRoot + "ErrorLog.txt"))
                {
                    w.WriteLine("Log Entry: {0}", DateTime.Now);
                    w.WriteLine("");
                    w.WriteLine(ex.ToString());
                    w.WriteLine("-------------------------------");
                    w.Flush();
                    w.Close();
                }
            }
            catch
            {
                Console.WriteLine("Failed to update log");
            }
        }
    }
}

