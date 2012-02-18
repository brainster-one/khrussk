
namespace Khrussk.Tests {
	using System.Net;
	using System.Threading;
	using Khrussk.Sockets;
	using System;

	/// <summary>Thread handler.</summary>
	/// <param name="exception">Exception.</param>
	public delegate void ThreadHandler();

	sealed class Context {
		/// <summary></summary>
		public Context() {
			ListenerSocket = new Socket();
			ClientSocket = new Socket();

			ClientSocket.Connected += callback;
			ClientSocket.DataReceived += callback;
			ClientSocket.Disconnected += callback;
			ListenerSocket.ConnectionAccepted += callback;

			EndPoint = new IPEndPoint(IPAddress.Loopback, ++_port);
			Wait = new ManualResetEvent(false);
		}

		public void Cleanup() {
			if (Accepted != null) Accepted.Disconnect();

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

		public IPEndPoint EndPoint { get; set; }

		public Socket ListenerSocket { get; set; }
		public Socket ClientSocket { get; set; }
		public Socket Accepted { get; set; }
		
		private ManualResetEvent Wait { get; set; }


		public SocketEventArgs SocketEventArgs { get; set; }
		static int _port = 1025;
		static object _lock = new object();
	}
}
