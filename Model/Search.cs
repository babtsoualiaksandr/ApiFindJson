using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFindJson.Model
{
    public class Search
    {
        public string Root { get; set; }
        public List<Search_Content> Contents { get; set; }

    }

        public class Search_Content
    {
        public string Name { get; set; }
        public string Content { get; set; }

    }


}