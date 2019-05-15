using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using HtmlAgilityPack;
using WordCloud.Models;

namespace WordCloud.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Data(int year, string query, string publisher, string srt)
        {
            string baseHtml = "";
            var client = new WebClient();
            var utf = new UTF8Encoding();
            var htmlDoc = new HtmlDocument();
            var data = new DataForGeneration();
            //client.Proxy = proxy;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            for (int i = 0; i < 100; i += 20)
            {
                var url = "https://dl.acm.org/results.cfm?query=" + query + (i == 0 ? "" : "&start=" + i) + "&filtered=";
                if (!string.IsNullOrEmpty(publisher))
                {
                    url += "acmdlPublisherName.raw=" + publisher;
                }
                url += "&within=owners.owner%3DHOSTED&dte=" + year + "&bfr=" + year + "&srt=" + srt;

                byte[] pageContent = client.DownloadData(url);
                baseHtml = utf.GetString(pageContent);
                htmlDoc.LoadHtml(baseHtml);
                var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='details']");
                if (htmlNodes == null)
                    break;
                data = GetData(htmlNodes, data, url);
            }
            var temp = data.TitlesAbstract.ToLowerInvariant();
            foreach (var item in query.Split(" "[0]))
            {
                temp = temp.Replace(" " + item.ToLowerInvariant().Trim() + " ", " ");
            }
            data.TitlesAbstract = Regex.Replace(temp, @"[ ]+", " ");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private DataForGeneration GetData(HtmlNodeCollection htmlNodes, DataForGeneration temp, string url)
        {
            foreach (var node in htmlNodes)
            {
                var title = node.SelectSingleNode(".//div[@class='title']/a").InnerText.Replace("\n", "");
                var resumee = node.SelectSingleNode(".//div[@class='abstract']")?.InnerText.Replace("\n", "") ?? "";
                var publishername = node.SelectSingleNode(".//div[@class='publisher']")?.InnerText ?? "";
                var kw = node.SelectSingleNode(".//div[@class='kw']")?.InnerText ?? "";
                if (kw.Contains(":"))
                {
                    kw = kw.Split(":"[0])[1].Trim();
                }
                if (publishername.Contains(";"))
                {
                    publishername = publishername.Split(";"[0])[1];
                }
                else if (publishername.Contains(";"))
                {
                    publishername = publishername.Split(":"[0])[1].Trim();
                }
                //temp.TitlesAbstract += title + " " + resumee + " " + kw + " ";
                temp.TitlesAbstract += title + " " + kw + " ";
                temp.papers.Add(new Paper() { Title = title, Abstract = resumee, Publisher = publishername, KeyWords = kw });
            }
            return temp;
        }
    }
}