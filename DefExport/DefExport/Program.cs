using System;
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
            for (int i = 0; i < 10; i++)
            {
                if (i == 9)
                    return;

                string v = streamReader.ReadLine();
                int v1 = v.IndexOf("File Type: DLL");
                if (v1 > 0)
                    break;
            }

            int num = 0;
            for (int i = 0; i < 10; i++)
            {
                if (i == 9)
                    return;

                string v = streamReader.ReadLine();
                int v1 = v.IndexOf("number of functions");
                if (v1 > 0)
                {
                    string v2 = v.Substring(0, v1);
                    num= int.Parse(v2);
                    break;
                }
            }

            int idx = 0;
            for (int i = 0; i < 10; i++)
            {
                if (i == 9)
                    return;

                string v = streamReader.ReadLine();
                int v1 = v.IndexOf("ordinal");
                if (v1 > 0)
                {
                    idx= v.IndexOf("name");
                    streamReader.ReadLine();
                    break;
                }
            }

            List<string> syms = new List<string>();
            for (int i = 0; i < num; i++)
            {
                string v = streamReader.ReadLine();
                string v1 = v.Substring(idx);
                syms.Add(v1);
            }


            streamReader.Close();
        }
    }
}
