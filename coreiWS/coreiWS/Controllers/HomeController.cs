using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using coreiWS.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.Net;
using System.Runtime.CompilerServices;

//-----------------------------------------------------------------  
// core-i Solutions Confidential                                     
// __________________                                                
//                                                                   
//  [2018]         Core-i Solutions Incorporated                    
//  All Rights Reserved.                                             
// NOTICE:  All information contained herein is, and remains         
// the property of Core-i Solutions Incorporated and its suppliers,  
// if any.  The intellectual and technical concepts contained        
// herein are proprietary to Core-i Solutions Incporporated          
// and its suppliers and may be covered by U.S. and Foreign Patents, 
// patents in process, and are protected by trade secret or copyright
// law.                                                              
// Dissemination of this information or reproduction of this material
// is strictly forbidden unless prior written permission is obtained 
// from Core-i Solutions Incorporated.                               
//-----------------------------------------------------------------  

// NOTE: happy to help with any implementation/configuration of the Core-i RST Framework
//       coreibmi.solutions@gmail.com
//       Documentation can be found at https://jeffersonvaughn.com/Core-i-Product-Downloads/
//
// Overiview:  Corei-RST provides an IBMi http framework (using Apache) and IBMi side API library that assists with 
//             traditional iSeries shops in creating/maintaining/implementing webservice API programs over 
//             their DB2 data.  Traditional RPG developers do not need to learn anything related to cross platform
//             integration when creating new APIs.  The IBMi COREIRST server side component will allow an IBMi admin/dev
//             to create/deploy an Apache HTTP server instance with a single option execution.  No manual configuration
//             is required.  For SSL/TLS, the instance can allow either user/password authentication or certificate 
//             authentication.  Steps to configure the IBMi server side certificate and export to .NET side can be
//             found in documentation link above.
//             The IBMi COREIRST tool will allow admins/developers to auto generate API's (and provide them with
//             the SQLRPGLE code) with a few parameter inputs.  COREIRST utilizes an IBMi middleware program that
//             handles all the http cross platform confusion a traditional RPG developer may have to face in normal
//             implementations.  All they need to do is create SQLRPGLE programs just like they have always done.
//             In fact, COREIRST can utilize any IBMi *PGM as an API, but SQLRPGLE is highly recommended as it allows
//             the ability to easily transorm relational database data into a json response.
//             Also, within the auto generate is the ability to specify simple sql queries that pull relational database
//             data and produce a JSON string in the exact same sql statement.  This could possibly be the only
//             minor learning curve they face, however it is an extremely clean and efficient way to produce the data
//             server side.  Additionaly, within the API auto generate feature the admin/developer can also
//             make calls to existing legacy programs that contain complex processing.  Lastly, the COREIRST is not
//             limited to pulling data.  Any API can be developed to execute ANYTHING on the iseries (ie. commands, 
//             start/end processes, call programs, etc.)  The sky is the limit.


namespace coreiWS.Controllers
{
    public class HomeController : Controller
    {

        private readonly IHttpClientFactory _clientFactory;

        public GetListOfAPIs listOfAPIs { get; set; }
        public GetTableLayout tableLayout { get; set; }
        public GetCustomerBankAccountInfo bankAccountInfo { get; set; }

        // the IBMi server COREIRST install will allow an IBMi admin/dev to create/deploy the server instance
        // it will be referenced as follows with routing to the IBMi Core-iRST webservice middleware.
        string Url = "https://yourIBMiServer.com/rest/rst00001r/";
        // traditionally IBMi userProfiles and passwords are 10 long all caps, but more modern configs include 
        // 128length passwords that are case sensitive.
        const string userProfile = "xxxxxxxx";
        const string password = "xxxxxx";

        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        //------------------------------------------------------------------------------------------------------------
        // getListOfAPIs API called when program called and displays response data
        //------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            // env allows capability of telling IBMi server job to set a library list for this particular API call
            // xxx does not set any library list at all (unless one is setup for xxx) - if API is in COREIRST and
            // does not reference any files outside of COREIRST, use xxx.
            // NOTE to COREIRST IBMi API developer - if any tables are used outside of the COREIRST library, then those
            // tables must be qualified by library name (schema.table) within the API OR a library list must be
            // specified that includes the library(s) of those referenced file(s)...
            // the library list can be set from within the IBMi CoreiRST (Maintain API Library - F7=Maintain ENV Libl)
            const string jsonRequest = "{\"env\":\"xxx\",\"command\":\"getListOfAPIs\",\"payload\":[{\"library\":\"COREIRST\",\"api\":\"getTableLayout\"},{\"library\":\"COREIRST\",\"api\":\"getCustomerBankAccountInfo\"}]}";

            // Get = jsonRequest/REST parm passed in url
            //  - Example REST with customer number parm - http://wwww.yourdomain.com/rest/rst00001r/203
            // Post = jsonRequest passed in body
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                //RequestUri = new Uri(Url + jsonRequest), 
                RequestUri = new Uri(Url),
                Content = new StringContent(jsonRequest, System.Text.Encoding.Default, "text/plain"),
            };

            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue(
                     "Basic", Convert.ToBase64String(
                                                     System.Text.ASCIIEncoding.ASCII.GetBytes(
                                                                                           $"{userProfile}:{password}")));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/plain"));

            try
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // corei will return a http status code stating whether API call successful or not, along with response data
                }
                else
                {
                    // corei will return a http status code, however, if an error, it will be provided in a user friendly message in the json response
                }

                // when not a client/server connection issue, always load response to the model... supports success/error messages
                try { 
                var responseString = await response.Content.ReadAsStringAsync();   
                listOfAPIs = JsonConvert.DeserializeObject<GetListOfAPIs>(responseString);
                return View(listOfAPIs);
            }
            // api json response issue
            catch (Exception e)
            {
                var responseString = "{\"success\":0,\"resultMessage\":\"Corei-Rst API getListOfAPIs json response appears to be invalid\"}";
                listOfAPIs = JsonConvert.DeserializeObject<GetListOfAPIs>(responseString);
                return View(listOfAPIs);
            }

        }
            // client/server connection issue
            catch (Exception e)
            {
                var responseString = "{\"success\":0,\"resultMessage\":\"Error connecting to " + Url + " to access API.  Ensure the server is up and running and try your request again.\"}";
                listOfAPIs = JsonConvert.DeserializeObject<GetListOfAPIs>(responseString);
                return View(listOfAPIs);
            }

        }   // end getListOfAPIs API
        //----------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------
        // corei API getTableLayout
        //------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> getTableLayout(string jsonRequest)
        {

            // Get = jsonRequest/REST parm passed in url
            //  - Example REST with customer number parm - http://wwww.yourdomain.com/rest/rst00001r/203
            // Post = jsonRequest passed in body
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                //RequestUri = new Uri(Url + jsonRequest), 
                RequestUri = new Uri(Url),
                Content = new StringContent(jsonRequest, System.Text.Encoding.Default, "text/plain"),
            };

            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue(
                     "Basic", Convert.ToBase64String(
                                                     System.Text.ASCIIEncoding.ASCII.GetBytes(
                                                                                           $"{userProfile}:{password}")));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.SendAsync(request);


                if (response.IsSuccessStatusCode)
                {
                    // corei will return a http status code stating whether API call successful or not, along with response data
                }
                else
                {
                    // corei will return a http status code, however, if an error, it will be provided in a user friendly message in the json response
                }

                // when not a client/server connection issue, always load response to the model... supports success/error messages
                try { 
                var responseString = await response.Content.ReadAsStringAsync();
                tableLayout = JsonConvert.DeserializeObject<GetTableLayout>(responseString);
                return View(tableLayout);
                  }
                   // api json response issue
                catch (Exception e)
                  {
                     var responseString = "{\"success\":0,\"resultMessage\":\"Corei-Rst API getTableLayout json response appears to be invalid\"}";
                     tableLayout = JsonConvert.DeserializeObject<GetTableLayout>(responseString);
                     return View(tableLayout);
                    }

        }
            // client/server connection issue
            catch (Exception e)
            {
                var responseString = "{\"success\":0,\"resultMessage\":\"Error connecting to " + Url + " to access API.  Ensure the server is up and running and try your request again.\"}";
                tableLayout = JsonConvert.DeserializeObject<GetTableLayout>(responseString);
                return View(tableLayout);
            }

        }   // end getTableLayout API
            //----------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------
        // corei API call getCustomerBankAccountInfo
        //------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> getCustomerBankAccountInfo( string jsonRequest)
        {

            // Get = jsonRequest/REST parm passed in url
            //  - Example REST with customer number parm - http://wwww.yourdomain.com/rest/rst00001r/203
            // Post = jsonRequest passed in body
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                //RequestUri = new Uri(Url + jsonRequest), 
                RequestUri = new Uri(Url),
                Content = new StringContent(jsonRequest, System.Text.Encoding.Default, "text/plain"),
            };

            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue(
                     "Basic", Convert.ToBase64String(
                                                     System.Text.ASCIIEncoding.ASCII.GetBytes(
                                                                                           $"{userProfile}:{password}")));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.SendAsync(request);


                if (response.IsSuccessStatusCode)
                {
                    // corei will return a http status code stating whether API call successful or not, along with response data
                }
                else
                {
                    // corei will return a http status code, however, if an error, it will be provided in a user friendly message in the json response
                }

                // when not a client/server connection issue, always load response to the model... supports success/error messages
                try
                { 

                var responseString = await response.Content.ReadAsStringAsync();
                bankAccountInfo = JsonConvert.DeserializeObject<GetCustomerBankAccountInfo>(responseString);
                return View(bankAccountInfo);
            }
            // api json response issue
            catch (Exception e)
            {
                var responseString = "{\"success\":0,\"resultMessage\":\"Corei-Rst API getCustomerBankAccountInfo json response appears to be invalid\"}";
                bankAccountInfo = JsonConvert.DeserializeObject<GetCustomerBankAccountInfo>(responseString);
                return View(bankAccountInfo);
            }

        }
            // client/server connection issue
            catch (Exception e)
            {
                var responseString = "{\"success\":0,\"resultMessage\":\"Error connecting to " + Url + " to access API.  Ensure the server is up and running and try your request again.\"}";
                bankAccountInfo = JsonConvert.DeserializeObject<GetCustomerBankAccountInfo>(responseString);
                return View(bankAccountInfo);
            }

        }   // end getCustomerBankAccountInfo API
            //----------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------
        // display corei overview
        //------------------------------------------------------------------------------------------------------------
        public IActionResult coreiOverview()
        {
            return View();
        }

    }
}
