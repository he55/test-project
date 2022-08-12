using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DefExport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            if (!File.Exists(args[0]))
                return;

            string toolPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\MSVC\14.32.31326\bin\Hostx86\x64";
            string dumpbinPath = Path.Combine(toolPath, "dumpbin.exe");
            string dllPath = Path.GetFullPath(args[0]);
            string fname = Path.GetFileNameWithoutExtension(dllPath);
            string tmpFilePath = fname + ".tmp.txt";
            string defPath = fname + ".def";
            string libPath = Path.Combine(toolPath, "lib.exe");
            string outputLib = fname + ".lib";

            Process process = Process.Start(new ProcessStartInfo
            {
                FileName = dumpbinPath,
                Arguments = $"/HEADERS /EXPORTS /OUT:\"{tmpFilePath}\" \"{dllPath}\"",
                WindowStyle = ProcessWindowStyle.Hidden
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
                return;

            string vstr;
            string arch = null;
            StreamReader streamReader = File.OpenText(tmpFilePath);
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                    return;

                vstr = streamReader.ReadLine();
                int v1 = vstr.IndexOf("File Type: DLL");
                if (v1 >= 0)
                {
                    vstr = streamReader.ReadLine();
                    vstr = streamReader.ReadLine();
                    int v2 = vstr.IndexOf("FILE HEADER VALUES");
                    if (v2 == -1)
                        return;

                    vstr = streamReader.ReadLine();
                    int v3 = vstr.IndexOf("machine");
                    if (v3 == -1)
                        return;

                    int idxs = vstr.IndexOf("(") + 1;
                    int idxe = vstr.IndexOf(")");
                    arch = vstr.Substring(idxs, idxe - idxs);
                    break;
                }
            }

            while (true)
            {
                vstr = streamReader.ReadLine();
                if (vstr == null)
                    return;

                int v1 = vstr.IndexOf("Section contains the following exports for");
                if (v1 >= 0)
                    break;
            }

            int num = 0;
            for (int i = 0; i < 7; i++)
            {
                if (i == 6)
                    return;

                vstr = streamReader.ReadLine();
                int v1 = vstr.IndexOf("number of functions");
                if (v1 >= 0)
                {
                    string v2 = vstr.Substring(0, v1);
                    num = int.Parse(v2);
                    break;
                }
            }

            vstr = streamReader.ReadLine();
            vstr = streamReader.ReadLine();
            vstr = streamReader.ReadLine();
            int vv1 = vstr.IndexOf("ordinal");
            if (vv1 == -1)
                return;

            int iname = vstr.IndexOf("name");
            int irva = vstr.IndexOf("RVA");
            int lenRva = iname - irva;
            int ihi = vstr.IndexOf("hint");
            int lenHi = irva - ihi;
            int lenOrd = ihi;
            vstr = streamReader.ReadLine();

            StreamWriter streamWriter = File.CreateText(defPath);
            streamWriter.WriteLine("EXPORTS");

            List<MyDef2> syms = new List<MyDef2>();
            for (int i = 0; i < num; i++)
            {
                vstr = streamReader.ReadLine();
                string strName = vstr.Substring(iname);
                string strRva = vstr.Substring(irva, lenRva);
                if (string.IsNullOrWhiteSpace(strRva))
                    continue;

                if (true)
                {
                    streamWriter.WriteLine(strName);
                }
                else
                {
                    string strOrd = vstr.Substring(0, lenOrd);
                    string strHi = vstr.Substring(ihi, lenHi);
                    syms.Add(new MyDef2 { ordinal = strOrd, hint = strHi, RVA = strRva, name = strName });
                }
            }

            streamWriter.Close();
            streamReader.Close();

            Process process1 = Process.Start(new ProcessStartInfo
            {
                FileName = libPath,
                Arguments = $"/DEF:\"{defPath}\" /MACHINE:{arch} /OUT:\"{outputLib}\"",
                WindowStyle = ProcessWindowStyle.Hidden
            });
            process1.WaitForExit();
            if (process1.ExitCode != 0)
                return;

            Console.WriteLine("OK");
        }
    }

    public class MyDef2
    {
        public string ordinal;
        public string hint;
        public string RVA;
        public string name;
    }
}
