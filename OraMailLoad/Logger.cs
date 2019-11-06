using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace OraMailLoad
{
    public class Logger
    {
        private string logfile = "oml.log";

        public void LogMessage(string message) => File.AppendAllText(logfile, string.Format("{0} {1} {2}", DateTime.Now.ToString(), message, Environment.NewLine));

        public void LogMessage(string message, string error)
        {
            File.AppendAllText(logfile, string.Format("{0} {1}. {2} {3}",DateTime.Now.ToString(), message, error, Environment.NewLine));
        }
    }
}
