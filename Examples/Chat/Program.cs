
namespace Khrussk.Examples.Chat {
	using System;
	using System.Net;

	class Program {
		static void Main() {
			Console.WriteLine("[S]erver or [C]lient: ");
			var ans = Console.ReadLine();
			if (ans == "S") {
				var service = new Service();
				service.Start(new IPEndPoint(IPAddress.Any, 9876));
				Console.WriteLine("Service has been started. Press any key to quit.");
				Console.ReadKey();
			} else {
				Console.WriteLine("Client has been started. Type 'quit' to close application.");
				var message = string.Empty;
				var client = new Client();
				client.Connect(new IPEndPoint(IPAddress.Loopback, 9876));

				while (message != "quit") {
					message = Console.ReadLine();
					client.Say(message);
				}
			}
		}

	}
}
