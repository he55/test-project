using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DefExport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dumpbinPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\MSVC\14.32.31326\bin\Hostx86\x86\dumpbin.exe";
            string tmpFilePath = "exp.txt";
            string dllPath = @"C:\Program Files (x86)\Common Files\Apple\Mobile Device Support\CFNetwork.dll";

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
            string arc;
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

                    arc = vstr.Substring(v3);
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

            string tmpPath = "tmp.def";
            StreamWriter streamWriter = File.CreateText(tmpPath);
            streamWriter.WriteLine("EXPORTS");

            List<MyDef2> syms = new List<MyDef2>();
            for (int i = 0; i < num; i++)
            {
                vstr = streamReader.ReadLine();
                string strName = vstr.Substring(iname);
                if (true)
                {
                    streamWriter.WriteLine(strName);
                }
                else
                {
                    string strOrd = vstr.Substring(0, lenOrd);
                    string strHi = vstr.Substring(ihi, lenHi);
                    string strRva = vstr.Substring(irva, lenRva);
                    syms.Add(new MyDef2 { ordinal = strOrd, hint = strHi, RVA = strRva, name = strName });
                }
            }

            streamWriter.Close();
            streamReader.Close();
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
