using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParserTest
{
    public class GetRequest
    {
        HttpWebRequest _request;
        string _address;
        
        public Dictionary<string, string> Headers { get; set; }
        public string Response { get; set; }
        public string Accept { get; set; }
        public string Host { get; set; }
        public WebProxy Proxy { get; set; }



        public GetRequest(string address)
        {
            _address= address;
            Headers = new Dictionary<string, string>();
        }

        public void Run()
        {
            _request = (HttpWebRequest)WebRequest.Create(_address);
            _request.Method = "GET";
            
            try
            {
                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                var stream = response.GetResponseStream();
                if (stream != null) { Response = new StreamReader(stream).ReadToEnd(); }
            }
            catch (Exception) { }
        }

        public void Run(CookieContainer cookieContainer)
        {
            _request = (HttpWebRequest)WebRequest.Create(_address);
            _request.Method = "GET";
            _request.CookieContainer = cookieContainer;
            _request.Proxy = Proxy;
            _request.Accept = Accept;
            _request.Host = Host;

            foreach(var pair in Headers)
            {
                _request.Headers.Add(pair.Key, pair.Value);
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                var stream = response.GetResponseStream();
                if (stream != null) { Response = new StreamReader(stream).ReadToEnd(); }
            }
            catch (Exception) { }
        }

    }
}
