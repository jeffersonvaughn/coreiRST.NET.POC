# coreiRST.NET.POC
Fully functional ASP.NET core 3.0 MVC integrated with iSeries coreiRST Webservice API's

More about me and my vast IBMi experience here... www.jeffersonvaughn.com
IBMi side CoreiRST framework can be downloaded here and used for free (with trial key)... https://jeffersonvaughn.com/Core-i-Product-Downloads/ 

Corei-RST provides an IBMi http framework (using Apache) and IBMi side API library that assists with
            traditional iSeries shops in creating/maintaining/implementing webservice API programs over
            their DB2 data.  Traditional RPG developers do not need to learn anything related to cross platform
            integration when creating new APIs.  The IBMi COREIRST server side component will allow an IBMi admin/dev
            to create/deploy an Apache HTTP server instance with a single option execution.  No manual configuration
            is required.  For SSL/TLS, the instance can allow either user/password authentication or certificate
            authentication.  Steps to configure the IBMi server side certificate and export to .NET side can be
            found in documentation link below.

            The IBMi COREIRST tool will allow admins/developers to auto generate API's (and provide them with
            the SQLRPGLE code) with a few parameter inputs.  COREIRST utilizes an IBMi middleware program that
            handles all the http cross platform confusion a traditional RPG developer may have to face in normal
            implementations.  All they need to do is create SQLRPGLE programs just like they have always done.
            In fact, COREIRST can utilize any IBMi *PGM as an API, but SQLRPGLE is highly recommended as it allows
            the ability to easily transform relational database data into a json response.
            Also, within the auto generate is the ability to specify simple sql queries that pull relational database
            data and produce a JSON string in the exact same sql statement.  This could possibly be the only
            minor learning curve they face, however it is an extremely clean and efficient way to produce the data
            server side.  Additionally, within the API auto generate feature the admin/developer can also
            make calls to existing legacy programs that contain complex processing.  Lastly, the COREIRST is not
            limited to pulling data.  Any API can be developed to execute ANYTHING on the iseries (ie. commands,
            start/end processes, call programs, etc.)  The sky is the limit.
            
            NOTE: 
            Happy to help with any implementation/configuration of the Core-i RST Framework
            coreibmi.solutions@gmail.com
            Documentation can be found at https://jeffersonvaughn.com/Core-i-Product-Downloads/ 
