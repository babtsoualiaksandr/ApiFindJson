using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFindJson.Model
{
    public class Result_List
    {
        public int Amount { get; set; }
        public List<Result_Data> Results { get; set; }

    }
}