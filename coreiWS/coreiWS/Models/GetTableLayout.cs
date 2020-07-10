using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreiWS.Models
{
    public class GetTableLayout
    {

            public int success { get; set; }
            public string resultMessage { get; set; }
            public List[] list { get; set; }


        public class List
        {
            public string schema { get; set; }
            public string table { get; set; }
            public Column[] columns { get; set; }
        }


            public class Column
            {
                public string shortColumn { get; set; }
                public string longColumn { get; set; }
            }

    }
}
