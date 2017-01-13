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

		public void WriteLocal(string value)
		{
			consoleWriter.Write(value);
		}

		public void WriteLineLocal(string value)
		{
			WriteLocal(value + NewLine);
		}

		public void WriteRemote(string value)
		{
			consoleLog.Append(value);
			GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.consoleWrite(value);
			WriteEvent?.Invoke(this, value);
		}

		public void WriteLineRemote(string value)
		{
			WriteRemote(value + NewLine);
		}

		public override void Write(string value)
		{
			WriteRemote(value);
			WriteLocal(value);
		}

		public override void WriteLine(string value)
		{
			this.Write(value + NewLine);
		}

		public void ClearLineLocal()
		{
			Console.SetCursorPosition(0, Console.CursorTop);
			WriteLocal(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(0, Console.CursorTop - (Console.WindowWidth >= Console.BufferWidth ? 1 : 0));
		}

		public void ExecuteCommand(string sender, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return;
			}

			if (sender == "Console")
			{
				WriteLineRemote(sender + ">" + value);
			}
			else
			{
				ClearLineLocal();
				WriteLine(sender + ">" + value);
			}

			switch (value)
			{
				case "restart":
					System.Diagnostics.Process.Start(Environment.GetCommandLineArgs()[0]);
					Environment.Exit(0);

					break;
				case "ping":
					WriteLine("Pong!");
					break;
				case "quit":
					StayOpen = false;
					//Cancel any Console.Read* that might be running
					var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
					PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0);
					return;
				default:
					WriteLine("Command not found");
					break;
			}

			if (sender != "Console")
			{
				WriteLocal("Console>");
			}
		}
	}
}
