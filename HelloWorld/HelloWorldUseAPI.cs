using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DllTest;
using System.Configuration;
using System.Net;
using System.IO;

namespace HelloWorld
{
    class HelloWorldUseAPI: HelloWorld
    {   
        /// <summary>
        /// there are 2 APIs to do the insert method
        /// </summary>
        /// <param name="value"></param>
        public override void DisplayValue(string value)
        {
            // base method will display the message in console screen
            base.DisplayValue(value);            
            
            // 1. use the web service to insert into database
            if ((Convert.ToBoolean(ConfigurationManager.AppSettings["UseWebService"])))
            {
                // a. REST service call
                if ((Convert.ToBoolean(ConfigurationManager.AppSettings["RESTfulService"])))
                {
                    try
                    {
                        // this is my localhost uri
                        string uri = Properties.Settings.Default.RESTServiceUriLocal + value;   // "http://localhost:25118/restservice/write/" + value;   
                        // this is my dev server uri
                        uri = Properties.Settings.Default.RESTServiceUriDevServer + value;      //"https://ca-dev-ws14:440/restservice/write/" + value;

                        HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;
                        req.KeepAlive = false;
                        req.Method = "GET";
                        req.ContentType = "application/xml";
                        req.Accept = "application/xml";

                        // since I created a self-signed certificate on my dev server so it's not trusted 
                        // this code it to ignore the SSL error:
                        // "Could not establish trust relationship for the SSL/TLS secure channel with authority"
                        // comment the code below when we have a valid SSL certificate installed on the server for this service
                        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                        (se, cert, chain, sslerror) =>
                        {
                            return true;
                        };    

                        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                        Encoding enc = System.Text.Encoding.GetEncoding(1252);
                        StreamReader loResponseStream = new StreamReader(resp.GetResponseStream(), enc);
                        string Response = loResponseStream.ReadToEnd();
                        loResponseStream.Close();
                        resp.Close();                        
                        System.Xml.XmlDocument listXML = new System.Xml.XmlDocument();
                        listXML.LoadXml(Response);
                        System.Xml.XmlElement root = listXML.DocumentElement;
                        string result = root.InnerText;
                        Console.WriteLine("Inserted into database by RESTService: " + result);
                        Thread.Sleep(5000);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                // b. WCF service call
                else
                {
                    try
                    {
                        // when the service is added as a local reference
                        WcfService1.Service1 service = new WcfService1.Service1();
                        bool result = service.WriteValue(value);
                        Console.WriteLine("Inserted into database by WCFService: " + result);
                        Thread.Sleep(5000);

                        // when the service is added as a web reference
                        WCFServiceHW.Service1 svc = new WCFServiceHW.Service1();
                        bool writeValueResult = false;
                        bool writeValueResultSpecified = true;
                        svc.WriteValue(value, out writeValueResult, out writeValueResultSpecified);
                        Console.WriteLine("Inserted into database by WCFService: " + writeValueResult);
                        Thread.Sleep(5000);
                    }
                    catch
                    { 
                    
                    }
                }
            }

            // 2. use the dll to insert into database
            else
            {
                DllHW dll = new DllHW();
                bool result = dll.WriteValue(value);
                Console.WriteLine("Inserted into database by DLL: " + result);
                Thread.Sleep(5000);
            } 
        }
    }
}
