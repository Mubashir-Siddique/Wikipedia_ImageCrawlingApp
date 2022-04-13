using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wikipedia_ImageCrawlingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string webAddress = "https://de.wikipedia.org/wiki/Wappen_der_deutschen_Stadt-_und_Landkreise";
            string web = "https://de.wikipedia.org";

            var response = Get(webAddress);

            HtmlDocument Doc = new HtmlDocument();

            Doc.LoadHtml(response);

            try
            {
                var nodes = Doc.DocumentNode.SelectNodes("//table[@class='wikitable']/tbody/tr/td/a[@class='image']");
                List<String> ImagesLinks = new List<String>();

                foreach (var item in nodes)
                {
                    if (item.Attributes["href"].Value != null)
                    {
                        if (true || item.Attributes["href"].Value.Contains("product") || item.Attributes["href"].Value.Contains(".jpg")) // && item.Attributes["href"].Value.Contains("category")
                        {

                            var result = Get(web + item.Attributes["href"].Value);
                            Doc.LoadHtml(result);
                            var Imagenode = Doc.DocumentNode.SelectSingleNode("//div[@class='fullImageLink']/a");

                            if (Imagenode.Attributes["href"].Value != null)
                            {
                                ImagesLinks.Add(Imagenode.Attributes["href"].Value);     // Attributes["href"].Value.Replace("&amp;", "&")
                            }
                        }
                    }
                }

                Console.WriteLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine();
        }

        static void GetImages(List<string> imagesURLs)
        {
            try
            {
                Directory.CreateDirectory("../Products");

                foreach (var url in imagesURLs)
                {
                    HttpWebRequest _WebRequest = (HttpWebRequest)WebRequest.Create(url);
                    _WebRequest.Method = "GET";
                    _WebRequest.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse responce = (HttpWebResponse)_WebRequest.GetResponse();


                    var stream = responce.GetResponseStream();
                    //var image = Image.FromStream(stream).Save("");


                    int pos = url.LastIndexOf("/") + 1;

                    string strFile = url.Substring(pos, url.Length - pos);

                    string ProductName = strFile.Substring(0, strFile.IndexOf("Price") - 1);

                    Directory.CreateDirectory("../Products/" + ProductName);

                    using (Stream file = File.Create("../Products/" + ProductName + "/" + strFile.Replace("-Price-in-Pakistan-ZahComputers", "")))
                    {
                        CopyStream(stream, file);
                    }

                    ////FileStream log = new FileStream(;
                    //byte[] buffer = new byte[1024];
                    //int c;
                    //while ((c = _WebRequest.GetRequestStream().Read(buffer, 0, buffer.Length)) > 0)
                    //{
                    //    //log.Write(buffer, 0, c);
                    //}
                    ////Write jpg filename to be picked up by regex and displayed on flash html page.
                    //Console.Write(strFile);
                    ////log.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
            }
        }

        static String Get(String url)
        {
            String resul = String.Empty;
            try
            {
                HttpWebRequest _WebRequest = (HttpWebRequest)WebRequest.Create(url);

                //WebProxy myproxy = new WebProxy("127.0.0.1:8888", false);
                //myproxy.BypassProxyOnLocal = false;
                //_WebRequest.Proxy = myproxy;
                _WebRequest.Method = "GET";

                _WebRequest.Credentials = CredentialCache.DefaultCredentials;

                //GetResponce
                HttpWebResponse responce = (HttpWebResponse)_WebRequest.GetResponse();
                Console.WriteLine(responce.StatusDescription);

                if (responce.StatusDescription == "OK")
                {
                    //Read Responce Stream
                    using (Stream dataStream = responce.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            resul = reader.ReadToEnd();
                            //Console.WriteLine(responcefromServer);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return resul;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

    }
}
