using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DefExport
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return -1;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine($"File not exists: \"{args[1]}\"");
                return -1;
            }

            int exitCode = StartProcess("where", "dumpbin");
            if (exitCode != 0)
            {
                Console.WriteLine("Not found dumpbin tool.");
                return -1;
            }

            string dllFilePath = Path.GetFullPath(args[1]);
            if (args[0] == "-create")
            {
                return CreateLib(dllFilePath);
            }
            else if (args[0] == "-copy")
            {
                return CopyDll(dllFilePath);
            }

            PrintUsage();
            return -1;
        }

        static void PrintUsage()
        {
            string Usage = String.Join(Environment.NewLine,
                "Dependencies.exe : command line tool for create an import library utilities.",
                "",
                "Usage : Dependencies.exe [OPTIONS] <FILE>",
                "",
                "Options :",
                "  -h -help : display this help",
                "  -create : create an import library",
                "  -copy : copy dependency"
            );

            Console.WriteLine(Usage);
        }

        static int CreateLib(string dllFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(dllFilePath);
            string tmpFilePath = $"{fileName}.tmp.txt";
            string defFilePath = $"{fileName}.def";
            string outputFilePath = $"{fileName}.lib";

            int exitCode = StartProcess("dumpbin", $"/HEADERS /EXPORTS /OUT:\"{tmpFilePath}\" \"{dllFilePath}\"");
            if (exitCode != 0)
                return -1;

            string str;
            int idx;
            string arch = null;
            StreamReader streamReader = File.OpenText(tmpFilePath);
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                    return -1;

                str = streamReader.ReadLine();
                idx = str.IndexOf("File Type: DLL");
                if (idx >= 0)
                {
                    str = streamReader.ReadLine();
                    str = streamReader.ReadLine();
                    idx = str.IndexOf("FILE HEADER VALUES");
                    if (idx == -1)
                        return -1;

                    str = streamReader.ReadLine();
                    idx = str.IndexOf("machine");
                    if (idx == -1)
                        return -1;

                    idx = str.IndexOf("(") + 1;
                    int idxEnd = str.IndexOf(")");
                    arch = str.Substring(idx, idxEnd - idx);
                    break;
                }
            }

            while (true)
            {
                str = streamReader.ReadLine();
                if (str == null)
                    return -1;

                idx = str.IndexOf("Section contains the following exports for");
                if (idx >= 0)
                    break;
            }

            int number = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                    return -1;

                str = streamReader.ReadLine();
                idx = str.IndexOf("number of functions");
                if (idx >= 0)
                {
                    string strNumber = str.Substring(0, idx);
                    number = int.Parse(strNumber);
                    break;
                }
            }

            str = streamReader.ReadLine();
            str = streamReader.ReadLine();
            str = streamReader.ReadLine();
            int idxOrdinal = str.IndexOf("ordinal");
            if (idxOrdinal == -1)
                return -1;

            int idxName = str.IndexOf("name");
            int idxRVA = str.IndexOf("RVA");
            int lenRVA = idxName - idxRVA;
            int idxHint = str.IndexOf("hint");
            int lenHint = idxRVA - idxHint;
            int lenOrdinal = idxHint;
            str = streamReader.ReadLine();

            List<Symbol> symbols = new List<Symbol>();
            StreamWriter streamWriter = File.CreateText(defFilePath);
            streamWriter.WriteLine("EXPORTS");

            for (int i = 0; i < number; i++)
            {
                str = streamReader.ReadLine();
                string strName = str.Substring(idxName);
                string strRVA = str.Substring(idxRVA, lenRVA);
                if (string.IsNullOrWhiteSpace(strRVA))
                    continue;

                if (true)
                {
                    streamWriter.WriteLine(strName);
                }
                else
                {
                    string strOrdinal = str.Substring(0, lenOrdinal);
                    string strHint = str.Substring(idxHint, lenHint);
                    symbols.Add(new Symbol { ordinal = strOrdinal, hint = strHint, RVA = strRVA, name = strName });
                }
            }

            streamWriter.Close();
            streamReader.Close();

            exitCode = StartProcess("lib", $"/NOLOGO /DEF:\"{defFilePath}\" /MACHINE:{arch} /OUT:\"{outputFilePath}\"", new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardError = true
            });
            if (exitCode != 0)
                return -1;

            return 0;
        }

        static HashSet<string> s_dlls = new HashSet<string>();

        static int CopyDll(string dllFilePath)
        {
            FindDll(dllFilePath);

            s_dlls.Add(dllFilePath);
            foreach (var item in s_dlls)
            {
                string fname = Path.GetFileName(item);
                Console.WriteLine($"Copying... {fname}");
                File.Copy(item, fname, true);
            }
            return 0;
        }

        static int FindDll(string dllFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(dllFilePath);
            string tmpFilePath = $"{fileName}.tmp.txt";
            string dir = Path.GetDirectoryName(dllFilePath);

            int exitCode = StartProcess("dumpbin", $"/DEPENDENTS /OUT:\"{tmpFilePath}\" \"{dllFilePath}\"");
            if (exitCode != 0)
                return -1;

            string str;
            int idx;
            StreamReader streamReader = File.OpenText(tmpFilePath);
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                    return -1;

                str = streamReader.ReadLine();
                if (str == null)
                    return -1;

                idx = str.IndexOf("Image has the following dependencies:");
                if (idx >= 0)
                    break;
            }

            List<string> dlls = new List<string>();

            str = streamReader.ReadLine();
            while (true)
            {
                str = streamReader.ReadLine();
                if (str == "")
                    break;

                dlls.Add(str.Trim());
            }

            str = streamReader.ReadLine();
            idx = str.IndexOf("Image has the following delay load dependencies:");
            if (idx >= 0)
            {
                str = streamReader.ReadLine();
                while (true)
                {
                    str = streamReader.ReadLine();
                    if (str == "")
                        break;

                    dlls.Add(str.Trim());
                }
            }

            streamReader.Close();

            foreach (var item in dlls)
            {
                string fullPath = Path.Combine(dir, item);
                if (!s_dlls.Contains(fullPath) && File.Exists(fullPath))
                {
                    s_dlls.Add(fullPath);
                    FindDll(fullPath);
                }
            }

            return 0;
        }

        static int StartProcess(string fileName, string arguments, ProcessStartInfo startInfo = null)
        {
            Process process = new Process();
            if (startInfo == null)
                startInfo = new ProcessStartInfo();

            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            return process.ExitCode;
        }
    }

    public class Symbol
    {
        public string ordinal;
        public string hint;
        public string RVA;
        public string name;
    }
}
