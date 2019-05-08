using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
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

        public JsonResult Data(int year, string query)
        {
            WebClient client = new WebClient();
            //client.Proxy = proxy;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string baseHtml = "";

            var data = new DataForGeneration();

            byte[] pageContent = client.DownloadData("https://dl.acm.org/results.cfm?query=" + query + "&filtered=&within=owners.owner%3DHOSTED&dte=" + year + "&bfr=&srt=_score");

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

                data.TitlesAbstract += title + " " + resumee + " ";
                data.papers.Add(new Paper() { Title = title, Abstract = resumee });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}