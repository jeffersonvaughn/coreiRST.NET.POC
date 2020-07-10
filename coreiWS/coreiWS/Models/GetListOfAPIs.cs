using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreiWS.Models
{
    public class GetListOfAPIs
    {


            public int success { get; set; }
            public string resultMessage { get; set; }
            public List[] list { get; set; }


        public class List
        {
            public string library { get; set; }
            public string api { get; set; }
            public string ibmiPgm { get; set; }
            public string requestExample { get; set; }
        }

    }

}
