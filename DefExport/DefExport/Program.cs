﻿using System.Collections.Generic;
using System.IO;

namespace DefExport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string p = @"C:\Users\luckh\Desktop\exports.txt";
            string vstr;
            StreamReader streamReader = File.OpenText(p);
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                    return;

                vstr = streamReader.ReadLine();
                int v1 = vstr.IndexOf("File Type: DLL");
                if (v1 >= 0)
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                if (i == 2)
                    return;

                vstr = streamReader.ReadLine();
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

                vstr = streamReader.ReadLine();
                int v1 = vstr.IndexOf("ordinal");
                if (v1 >= 0)
                {
                    iname = vstr.IndexOf("name");
                    irva = vstr.IndexOf("RVA");
                    lenRva = iname - irva;
                    ihi = vstr.IndexOf("hint");
                    lenHi = irva - ihi;
                    lenOrd = ihi;
                    vstr = streamReader.ReadLine();
                    break;
                }
            }

            string tmpPath = "tmp.def";
            StreamWriter streamWriter = File.CreateText(tmpPath);
            streamWriter.WriteLine("EXPORTS");
            
            List<MyDef2> syms = new List<MyDef2>();
            for (int i = 0; i < num; i++)
            {
                vstr = streamReader.ReadLine();
                string strName = vstr.Substring(iname);
                if(true){
                    streamWriter.WriteLine(strName);
                }else{
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
