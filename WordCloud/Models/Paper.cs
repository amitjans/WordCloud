using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordCloud.Models
{
    public class Paper
    {
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Publisher { get; set; }
        public string KeyWords { get; set; }
    }
}