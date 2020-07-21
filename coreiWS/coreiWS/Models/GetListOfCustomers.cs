using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreiWS.Models
{
    public class GetListOfCustomers
    {

            public int success { get; set; }
            public string resultMessage { get; set; }
            public List[] list { get; set; }

            public class List
            {
                public string custNo { get; set; }
                public string firstName { get; set; }
                public string lastName { get; set; }
                public string address1 { get; set; }
                public string address2 { get; set; }
                public string city { get; set; }
                public string state { get; set; }
                public string zip { get; set; }
                public string routing { get; set; }
                public string accountNo { get; set; }
            }

        }

}
