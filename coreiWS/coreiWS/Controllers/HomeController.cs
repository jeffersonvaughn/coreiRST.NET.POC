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
//             The IBMi COREIRST tool will allow IBMi admins/developers to auto generate API's (and provide them with
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
        public GetListOfAPIs              g_listOfAPIs { get; set; }
        public EndPointExecutionTimeOnly  g_executionTime { get; set; }
        public ModifyAPIRequest           g_apiRequest { get; set; }
        public GetTableLayout             g_tableLayout { get; set; }
        public GetCustomerBankAccountInfo g_bankAccountInfo { get; set; }
        public GetListOfCustomers         g_customerList { get; set; }

        // the IBMi server COREIRST install will allow an IBMi admin/dev to create/deploy the server instance
        // it will be referenced as follows with routing to the IBMi Core-iRST webservice middleware.
        const string g_Url = "https://yourIBMi.com/rest/rst00001r/";
        // traditionally IBMi userProfiles and passwords are 10 long all caps, but more modern configs include 
        // 128length passwords that are case sensitive.
        const string g_userProfile = "XXXXXXXXXX";
        const string g_password = "XxxxXxxxxx";
        const string g_coreiErrorJSON   = "{\"success\":0,\"resultMessage\":\"" + "Corei-Rst API modifyAPIRequest json response appears to be invalid\"}";
        const string g_coreiErrorServer = "{\"success\":0,\"resultMessage\":\"" + "Error connecting to IBMi Http Endpoint.  Ensure the server is up and running and try your request again.\"}";
        const string g_coreiErrorServerUnauth = "{\"success\":0,\"resultMessage\":\"" + "Error connecting to IBMi Http Endpoint.  " +
                                                  "Check your IBMi UserProfile/Password credentials and also ensure that the " +
                                                  "user profile is of class *PGMR.\"}";

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
            // specified that includes the library(s) of those referenced file(s)... (basic IBMi admin stuff)...
            // the library list can be set from within the IBMi CoreiRST (Maintain API Library - F7=Maintain ENV Libl)
            const string jsonRequest = "{\"env\":\"xxx\",\"command\":\"getListOfAPIs\",\"payload\":[" +
                                              "{\"apiLibrary\":\"COREIRST\",\"apiCommand\":\"endPointExecutionTimeOnly\"}" +
                                              ",{\"apiLibrary\":\"COREIRST\",\"apiCommand\":\"getTableLayout\"}" +
                                              ",{\"apiLibrary\":\"COREIRST\",\"apiCommand\":\"getCustomerBankAccountInfo\"}" +
                                              ",{\"apiLibrary\":\"COREIRST\",\"apiCommand\":\"getListOfCustomers\"}" +
                                              "]}";

            g_listOfAPIs = (GetListOfAPIs) await ExecuteCoreiHttpRequest<GetListOfAPIs>(jsonRequest);
            return View(g_listOfAPIs);



        }   // end getListOfAPIs API
        //----------------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------------------------
        // corei API endPointExecutionTimeOnly
        //------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> endPointExecutionTimeOnly(string jsonRequest)
        {

            g_executionTime = (EndPointExecutionTimeOnly)await ExecuteCoreiHttpRequest<EndPointExecutionTimeOnly>(jsonRequest);
            return View(g_executionTime);

        }   
        // end getTableLayout API
        //----------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------
        // corei API getTableLayout
        //------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> getTableLayout(string jsonRequest)
        {

            g_tableLayout = (GetTableLayout)await ExecuteCoreiHttpRequest<GetTableLayout>(jsonRequest);
            return View(g_tableLayout);

        }   
        // end getTableLayout API
        //----------------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------------------------
        // corei API call getCustomerBankAccountInfo
        //------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> getCustomerBankAccountInfo( string jsonRequest)
        {

            g_bankAccountInfo = (GetCustomerBankAccountInfo)await ExecuteCoreiHttpRequest<GetCustomerBankAccountInfo>(jsonRequest);
            return View(g_bankAccountInfo);

        }
        // end getCustomerBankAccountInfo API
        //----------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------
        // corei API call getListOfCustomers
        //------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> getListOfCustomers(string jsonRequest)
        {

            g_customerList = (GetListOfCustomers)await ExecuteCoreiHttpRequest<GetListOfCustomers>(jsonRequest);
            return View(g_customerList);

        }
        // end getCustomerBankAccountInfo API
        //----------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------
        // corei API call modifyAPIRequest
        //------------------------------------------------------------------------------------------------------------
        [HttpGet]
        [HttpPost]
        public IActionResult modifyAPIRequest(string apiLibrary, string apiCommand, string requestExample)
        {

            if (requestExample is null)
            {
                requestExample = "";
            };
            requestExample = JsonConvert.ToString(requestExample);

                var viewJson = "{\"success\":1,\"resultMessage\":\"Success\",\"list\":[" +
                                         "{\"apiLibrary\":\"" + apiLibrary + 
                                          "\",\"apiCommand\":\"" + apiCommand + 
                                          "\",\"requestExample\":" + requestExample + "}]}";
                g_apiRequest = JsonConvert.DeserializeObject<ModifyAPIRequest>(viewJson);
                return View(g_apiRequest);

        }   
        // end modifyAPIRequest API
        //----------------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------------------------
        // display corei overview
        //------------------------------------------------------------------------------------------------------------
        public IActionResult coreiOverview()
        {
            return View();
        }


        //------------------------------------------------------------------------------------------------------------
        // display corei overview
        //------------------------------------------------------------------------------------------------------------
        public async Task<object> ExecuteCoreiHttpRequest<T>(string jsonRequest)
        {


            // Get = jsonRequest/REST parm passed in url
            //  - Example REST with customer number parm - http://wwww.yourdomain.com/rest/rst00001r/commandName/203
            // Post = jsonRequest passed in body
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                //RequestUri = new Uri(Url + jsonRequest), 
                RequestUri = new Uri(g_Url),
                Content = new StringContent(jsonRequest, System.Text.Encoding.Default, "text/plain"),
            };

            // use named client from startup
            // NOTE: startup disregards any un-trusted certs and allows connections
            //       this is fine with the jeffersonvaughn.com website as long as the application 
            //       controls EXACTLY which API's are called from jvaughn1.powerbunker.com .
            //       For ANY customer implementation of CoreiRST, they will need to use LetsEncrypt or
            //       a paid SSL certificate solution to implement a truted SSL certificate on the IBMi server.
            var client = _clientFactory.CreateClient("coreiClient");

            // for userProfile/password auth...
            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue(
                     "Basic", Convert.ToBase64String(
                                                     System.Text.ASCIIEncoding.ASCII.GetBytes(
                                                                                           $"{g_userProfile}:{g_password}")));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await client.SendAsync(request);

                // determine if server/credentials issue...
                if (response.StatusCode == (HttpStatusCode)401)
                {
                    return (JsonConvert.DeserializeObject<T>(g_coreiErrorServerUnauth));

                }

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
                    //---------------------------------------------------------------
                    // good response
                    //---------------------------------------------------------------
                    var responseString = await response.Content.ReadAsStringAsync();
                    return (JsonConvert.DeserializeObject<T>(responseString));
                }
                    //---------------------------------------------------------------
                    // api json response issue
                    //---------------------------------------------------------------
                    catch (Exception e)
                    {
                   
                    return (JsonConvert.DeserializeObject<T>(g_coreiErrorJSON));
                }

            }
               //---------------------------------------------------------------
               // server endpoint connection issue
               //---------------------------------------------------------------
               catch (Exception e)
               {

                return (JsonConvert.DeserializeObject<T>(g_coreiErrorServer));

            }
        }

    }
}
