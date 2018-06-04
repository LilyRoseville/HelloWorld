using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DllTest;
using System.Configuration;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            DllHW dll = new DllHW();
            string value = dll.GetValue();

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["WriteToDatabase"]))
            {
                HelloWorldUseAPI hwuapi = new HelloWorldUseAPI();                
                hwuapi.DisplayValue(value);
            }
            else
            {
                HelloWorld hw = new HelloWorld();
                hw.DisplayValue(value);
            }
        }
    }
}
