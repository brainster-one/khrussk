
namespace Khrussk.Tests.Sockets {
	using System;
	using Khrussk.Sockets;

	/// <summary>Thread handler.</summary>
	/// <param name="exception">Exception.</param>
	public delegate void ThreadHandler();

	/// <summary>Context for tests.</summary>
	sealed class SocketTestContext : TestContext {
		/// <summary>Initializes a new instance of the SocketTestContext class using the specified socket.</summary>
		public SocketTestContext() {
			ListenerSocket = new Socket();
			ClientSocket = new Socket();

			ClientSocket.Connected += callback;
			ClientSocket.DataReceived += callback;
			ClientSocket.Disconnected += callback;
			ListenerSocket.ConnectionAccepted += callback;
		}

		/// <summary>Cleanup environment.</summary>
		public void Cleanup() {
			if (Accepted != null)
				Accepted.Disconnect();
			ClientSocket.Disconnect();
			ListenerSocket.Disconnect();
		}

		public bool WaitFor(ThreadHandler h) {
			lock (_lock) {
				Wait.Reset();
				h();
				var res = Wait.WaitOne(TimeSpan.FromSeconds(10));
				return res;
			}
		}

		void callback(object sender, SocketEventArgs e) {
			if (e.Socket != null) {
				Accepted = e.Socket;
				Accepted.DataReceived += callback;
				Accepted.Disconnected += callback;
			}
			SocketEventArgs = e;
			Wait.Set();
		}

		public Socket ListenerSocket { get; set; }
		public Socket ClientSocket { get; set; }
		public Socket Accepted { get; set; }
		public SocketEventArgs SocketEventArgs { get; set; }
		
		static object _lock = new object();
	}
}
