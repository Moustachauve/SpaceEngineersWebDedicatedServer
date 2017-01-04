using Microsoft.AspNet.SignalR;
using ServerExtender.Hubs;
using System;
using System.IO;
using System.Text;

namespace ServerExtender
{
    public class ConsoleHandler : TextWriter
    {
        private static ConsoleHandler instance;
        public static ConsoleHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConsoleHandler();
                return instance;
            }
        }

        public EventHandler<string> WriteEvent;

        public string Log { get { return consoleLog.ToString(); } }
        public override Encoding Encoding { get { return new ASCIIEncoding(); } }

        private TextWriter consoleWriter;
        private StringBuilder consoleLog;

        private ConsoleHandler()
        {
            consoleLog = new StringBuilder();
            consoleWriter = Console.Out;
        }

        public override void Write(string value)
        {
            consoleLog.Append(value);
            consoleWriter.Write(value);
            base.Write(value);

            WriteEvent?.Invoke(this, value);
        }

        public override void WriteLine(string value)
        {
            consoleLog.AppendLine(value);
            consoleWriter.WriteLine(value);
            base.WriteLine(value);

            GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.consoleWrite(value + NewLine);
            WriteEvent?.Invoke(this, value + NewLine);
        }
    }
}
