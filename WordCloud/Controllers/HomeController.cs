using System.Net;
using System.Text;
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
            WebClient client = new WebClient();
            //client.Proxy = proxy;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string baseHtml = "";

            var data = new DataForGeneration();

            var url = "https://dl.acm.org/results.cfm?query=" + query + "&filtered=";
            if (!string.IsNullOrEmpty(publisher))
            {
                url += "acmdlPublisherName.raw=" + publisher;
            }
            url += "&within=owners.owner%3DHOSTED&dte=" + year + "&bfr=" + year + "&srt=" + srt;

            byte[] pageContent = client.DownloadData(url);

            UTF8Encoding utf = new UTF8Encoding();
            baseHtml = utf.GetString(pageContent);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(baseHtml);

            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='details']");

            foreach (var node in htmlNodes)
            {
                //foreach (var hnode in htmlDoc.DocumentNode.SelectNodes(".//div/span"))

                var title = node.SelectSingleNode(".//div[@class='title']/a").InnerText.Replace("\n", "");
                var resumee = node.SelectSingleNode(".//div[@class='abstract']")?.InnerText.Replace("\n", "") ?? "";
                var publishername = node.SelectSingleNode(".//div[@class='publisher']").InnerText;
                var kw = node.SelectSingleNode(".//div[@class='kw']")?.InnerText ?? "";
                if (kw.Contains(":"))
                {
                    kw = kw.Split(":"[0])[1].Trim();
                }
                if (publishername.Contains(";"))
                {
                    publishername = publishername.Split(";"[0])[1];
                }
                else if (publishername.Contains(";")) {
                    publishername = publishername.Split(":"[0])[1].Trim();
                }

                data.TitlesAbstract += title + " " + resumee + " " + kw + " ";
                data.papers.Add(new Paper() { Title = title, Abstract = resumee, Publisher = publishername, KeyWords = kw });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}