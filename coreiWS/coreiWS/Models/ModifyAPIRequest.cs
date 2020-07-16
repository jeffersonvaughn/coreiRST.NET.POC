using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreiWS.Models
{
    public class ModifyAPIRequest
    {

        public int success { get; set; }
        public string resultMessage { get; set; }

        public List[] list { get; set; }

        public class List
        {
             public string apiLibrary { get; set; }
             public string apiCommand { get; set; }
             public string requestExample { get; set; }


            public string firstChar
            {
                get { return requestExample != null ? requestExample.Substring(0, 1) : ""; }
            }


        }


    }
}
