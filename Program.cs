using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TEST2_Service_Skanuj_to
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        static void Main()
        {
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource("MySource", "MyNewLog");
                
                return;
            }
            EventLog myLog = new EventLog();
            myLog.Source = "MySource";
            myLog.WriteEntry("Writing to event log.");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new NewService()
            };
            ServiceBase.Run(ServicesToRun);


        }
    }
}
