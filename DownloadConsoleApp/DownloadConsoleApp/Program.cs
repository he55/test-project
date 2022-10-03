using System;
using System.IO;
using System.Net;

namespace DownloadConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WebClient webClient = new WebClient();
            string[] urls = File.ReadAllLines("dl.txt");
            foreach (var url in urls)
            {
                Uri uri=new Uri(url);
                string host = uri.Host;
                string apath=uri.AbsolutePath.Substring(1);
                string fpath=apath.Substring(0,apath.LastIndexOf('/'));
                if(!Directory.Exists(fpath))
                    Directory.CreateDirectory(fpath);

                webClient.DownloadFile(url, apath);
            }
        }
    }
}
