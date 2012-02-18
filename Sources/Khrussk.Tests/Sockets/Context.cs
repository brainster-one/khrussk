
namespace Khrussk.Tests {
	using System.Net;
	using System.Threading;
	using Khrussk.Sockets;

	sealed class Context {
		/// <summary></summary>
		public Context() {
			ListenerSocket = new Socket();
			ClientSocket = new Socket();

			ClientSocket.Connected += callback;
			ClientSocket.DataReceived += callback;
			ClientSocket.Disconnected += callback;
			ListenerSocket.ClientSocketAccepted += callback;

			EndPoint = new IPEndPoint(IPAddress.Loopback, ++_port);
		}

		public void Cleanup() {
			if (Accepted != null) Accepted.Disconnect();

			ClientSocket.Disconnect();
			ListenerSocket.Disconnect();
		}

		void callback(object sender, SocketEventArgs e) {
			lock (_wait) {
				if (e.ClientSocket != null) {
					Accepted = e.ClientSocket;
					Accepted.DataReceived += callback;
				}
				SocketEventArgs = e;
				Wait.Set();
			}
		}

		public IPEndPoint EndPoint { get; set; }

		public Socket ListenerSocket { get; set; }
		public Socket ClientSocket { get; set; }
		public Socket Accepted { get; set; }
		
		public ManualResetEvent Wait {
			get { lock (_wait) { _wait.Reset(); return _wait; } }
		}

		public SocketEventArgs SocketEventArgs { get; set; }
		private ManualResetEvent _wait = new ManualResetEvent(false);
		static int _port = 1025;
	}
}
