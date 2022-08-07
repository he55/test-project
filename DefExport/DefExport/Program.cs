using System.Collections.Generic;
using System.IO;

namespace DefExport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string p = @"C:\Users\luckh\Desktop\exports.txt";
            StreamReader streamReader = File.OpenText(p);
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                    return;

                string v = streamReader.ReadLine();
                int v1 = v.IndexOf("File Type: DLL");
                if (v1 >= 0)
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                if (i == 2)
                    return;

                string v = streamReader.ReadLine();
                int v1 = v.IndexOf("Section contains the following exports for");
                if (v1 >= 0)
                    break;
            }

            int num = 0;
            for (int i = 0; i < 7; i++)
            {
                if (i == 6)
                    return;

                string v = streamReader.ReadLine();
                int v1 = v.IndexOf("number of functions");
                if (v1 >= 0)
                {
                    string v2 = v.Substring(0, v1);
                    num = int.Parse(v2);
                    break;
                }
            }

            int iname = 0;
            int irva = 0;
            int lenRva = 0;
            int ihi = 0;
            int lenHi = 0;
            int lenOrd = 0;
            for (int i = 0; i < 4; i++)
            {
                if (i == 3)
                    return;

                string v = streamReader.ReadLine();
                int v1 = v.IndexOf("ordinal");
                if (v1 >= 0)
                {
                    iname = v.IndexOf("name");
                    irva = v.IndexOf("RVA");
                    lenRva = iname - irva;
                    ihi = v.IndexOf("hint");
                    lenHi = irva - ihi;
                    lenOrd = ihi;
                    streamReader.ReadLine();
                    break;
                }
            }

            List<MyDef2> syms = new List<MyDef2>();
            for (int i = 0; i < num; i++)
            {
                string v = streamReader.ReadLine();
                string strOrd = v.Substring(0, lenOrd);
                string strHi = v.Substring(ihi, lenHi);
                string strRva = v.Substring(irva, lenRva);
                string strName = v.Substring(iname);
                syms.Add(new MyDef2 { ordinal = strOrd, hint = strHi, RVA = strRva, name = strName });
            }


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
