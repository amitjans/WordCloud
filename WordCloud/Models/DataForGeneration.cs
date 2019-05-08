using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordCloud.Models
{
    public class DataForGeneration
    {
        public string TitlesAbstract { get; set; }
        public List<Paper> papers { get; set; }

        public DataForGeneration()
        {
            this.papers = new List<Paper>();
        }
    }
}