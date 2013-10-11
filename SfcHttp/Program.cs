using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SfcHttp
{
    class Program
    {

        private string sessionId;
        private string host;

        static void Main(string[] args)
        {
            var url = @"http://192.168.200.26:8888/tcw/";
            string itemNumber = "77777";
            string itemRevision = "A";
            string itemName = "CAM-Example";
            string workPlace = "CAM-Example";
            string ncProgram = "nc-program";
            Program m = new Program();
            m.Download(itemNumber, itemRevision, itemName, workPlace, ncProgram, url);
        }

        private string CreateGet(string url, string get = "")
        {
            Console.Write(".");
            if (!string.IsNullOrEmpty(sessionId))
                url = url + ";jsessionid=" + sessionId;
            if (!string.IsNullOrEmpty(get))
                url = url + "?" + get;            
            var wc = new WebClient();
            wc.Headers.Add("Host", "192.168.200.26:8888");
            wc.Headers.Add("Cache-Control", "max-age=0");
            wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.69 Safari/537.36");
            wc.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            wc.Headers.Add("Accept-Language", "en,en-US;q=0.8");
            var s = wc.DownloadData(host + url);
            return Encoding.Default.GetString(s);
        }

        private string CreatePost(string url, string post = "")
        {
            Console.Write(".");
            if (!string.IsNullOrEmpty(sessionId))
                url = url + ";jsessionid=" + sessionId;
            var wc = new WebClient();
            wc.Headers.Add("Host", "192.168.200.26:8888");
            wc.Headers.Add("Cache-Control", "max-age=0");
            wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.69 Safari/537.36");
            wc.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            wc.Headers.Add("Accept-Language", "en,en-US;q=0.8");
            var s = wc.UploadString(host + url, post);
            return s;
        }

        private void Download(string itemNumber, string itemRevision, string itemName, string workPlace, string ncProgram, string url)
        {
            host = url;
            string s = CreateGet("index.jsp");
            sessionId = FindSessionId(s);
            s = CreateGet("logon");
            s = CreateGet("selectworkplace");
//            CreateGet("selectworkplace", "Number=0"); //todo find correct number
            s = CreateGet("selectworkplace", "Number=0");
            s = CreateGet("st", "tn=mainFrameSet.vm");
            s = CreatePost("requestOrder", "item1=77777&item2=A&item3=CAM-Example&item7=MENCMachining*%3BItem*&filled=true");
            s = CreateGet("detail");
            s = CreateGet("showDatasetInfo", "pos=NCP|sdla|4");
            s = CreatePost("transferFiles", "dsn_NCP%7Csdla%7C4=NCP%7Csdla%7C4%7C0&NCP%7Csdla%7C4%7C0=ma&counter=1");

        }



        private string FindSessionId(string s)
        {
            var m = Regex.Match(s, "jsessionid=([A-Z0-9]+)",RegexOptions.IgnoreCase);
            return m.Groups[1].ToString();
        }
    }
}
