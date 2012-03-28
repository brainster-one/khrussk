
namespace Khrussk.Examples.Chat {
	using System;
	using System.Net;
	using Protocol;
	using NetworkRealm;
	using System.Diagnostics;




	class Program {
		static void Main(string[] args) {
			Debug.Listeners.Add(new ConsoleTraceListener());
			Debug.AutoFlush = true;

			var service = new Service();
			var client = new Client();

			service.Start(new IPEndPoint(IPAddress.Any, 9876));
			client.Connect(new IPEndPoint(IPAddress.Loopback, 9876));
			
			System.Threading.Thread.Sleep(1000);
			client.Say("Hello world!");

			Console.WriteLine("All done. Press any key to quit.");
			Console.ReadLine();
		}

	}
}
