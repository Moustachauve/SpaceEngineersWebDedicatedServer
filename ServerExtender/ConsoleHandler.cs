using Microsoft.AspNet.SignalR;
using ServerExtender.Hubs;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ServerExtender
{
    public class ConsoleHandler : TextWriter
    {
		const int VK_RETURN = 0x0D;
		const int WM_KEYDOWN = 0x100;

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
		public bool StayOpen { get; private set; }

        public string Log { get { return consoleLog.ToString(); } }
        public override Encoding Encoding { get { return new ASCIIEncoding(); } }

        private TextWriter consoleWriter;
        private StringBuilder consoleLog;

		[DllImport("User32.Dll", EntryPoint = "PostMessageA")]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		private ConsoleHandler()
        {
            consoleLog = new StringBuilder();
            consoleWriter = Console.Out;
			StayOpen = true;
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

		public void ExecuteCommand(string sender, string value)
		{
			if(string.IsNullOrWhiteSpace(value))
			{
				return;
			}

			WriteLine(sender + "->" + value);

			if(value == "quit")
			{
				StayOpen = false;
				//Cancel any Console.Read* that might be running
				var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
				PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0);
				return;
			}

			WriteLine("Command not found");
		}
	}
}
