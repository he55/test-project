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
            if (args.Length == 0)
            {
                Console.WriteLine("Use: DefExport.exe <dllfile>");
                return -1;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"File not exists: \"{args[0]}\"");
                return -1;
            }

            int exitCode;
            exitCode = StartProcess("where", "dumpbin");
            if (exitCode != 0)
            {
                Console.WriteLine("Not found dumpbin tool.");
                return -1;
            }

            string dllFilePath = Path.GetFullPath(args[0]);
            string fileName = Path.GetFileNameWithoutExtension(dllFilePath);
            string tmpFilePath = Path.GetFullPath($"{fileName}.tmp.txt");
            string defFilePath = Path.GetFullPath($"{fileName}.def");
            string outputFilePath = Path.GetFullPath($"{fileName}.lib");

            exitCode = StartProcess("dumpbin", $"/NOLOGO /HEADERS /EXPORTS /OUT:\"{tmpFilePath}\" \"{dllFilePath}\"");
            if (exitCode != 0)
                return -1;

            string str;
            string arch = null;
            StreamReader streamReader = File.OpenText(tmpFilePath);
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                    return -1;

                str = streamReader.ReadLine();
                int idx1 = str.IndexOf("File Type: DLL");
                if (idx1 >= 0)
                {
                    str = streamReader.ReadLine();
                    str = streamReader.ReadLine();
                    int idx2 = str.IndexOf("FILE HEADER VALUES");
                    if (idx2 == -1)
                        return -1;

                    str = streamReader.ReadLine();
                    int idx3 = str.IndexOf("machine");
                    if (idx3 == -1)
                        return -1;

                    int idxStart = str.IndexOf("(") + 1;
                    int idxEnd = str.IndexOf(")");
                    arch = str.Substring(idxStart, idxEnd - idxStart);
                    break;
                }
            }

            while (true)
            {
                str = streamReader.ReadLine();
                if (str == null)
                    return -1;

                int idx = str.IndexOf("Section contains the following exports for");
                if (idx >= 0)
                    break;
            }

            int number = 0;
            for (int i = 0; i < 7; i++)
            {
                if (i == 6)
                    return -1;

                str = streamReader.ReadLine();
                int idx = str.IndexOf("number of functions");
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

            Console.WriteLine("OK");
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
