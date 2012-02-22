
namespace Khrussk.Tests.Sockets {
	using System;
	using Khrussk.Sockets;
	using System.Collections.Generic;
	using System.Linq;
	using System.Collections.ObjectModel;

	/// <summary>Thread handler.</summary>
	/// <param name="exception">Exception.</param>
	public delegate void ThreadHandler();

	/// <summary>Context for tests.</summary>
	sealed class SocketTestContext : TestContext {
		/// <summary>Initializes a new instance of the SocketTestContext class.</summary>
		public SocketTestContext() {
			// Client
			var client = new Socket();
			client.Connected += callback;
			client.DataReceived += callback;
			client.Disconnected += callback4;
			_clientSockets.Add(client);
			
			// Listener socket
			ListenerSocket = new Socket();
			ListenerSocket.ConnectionAccepted += callback2;
		}

		/// <summary>Cleanup environment.</summary>
		public void Cleanup() {
			ClientSockets.ToList().ForEach(x => x.Disconnect());
			AcceptedSockets.ToList().ForEach(x => x.Disconnect());
			ListenerSocket.Disconnect();
		}

		void callback(object sender, SocketEventArgs e) {
			SocketEventArgs = e;
			Wait.Set();
		}

		void callback2(object sender, SocketEventArgs e) {
			e.Socket.DataReceived += callback;
			e.Socket.Disconnected += callback3;
			_acceptedSockets.Add(e.Socket);
			SocketEventArgs = e;
			Wait.Set();
		}

		void callback3(object sender, SocketEventArgs e) {
			if (_acceptedSockets.Contains(e.Socket)) _acceptedSockets.Remove(e.Socket);
			SocketEventArgs = e;
		}

		void callback4(object sender, SocketEventArgs e) {
			if (_clientSockets.Contains(e.Socket)) _clientSockets.Remove(e.Socket);
			SocketEventArgs = e;
		}

		public Socket NewSocket() {
			var ns = new Socket();
			_clientSockets.Add(ns);
			return ns;
		}

		public Socket ListenerSocket { get; set; }
		public Socket ClientSocket {
			get { return _clientSockets.First(); }
		}
		public IEnumerable<Socket> ClientSockets { 
			get { return _clientSockets.AsReadOnly(); } 
		}
		public IEnumerable<Socket> AcceptedSockets { 
			get { return _acceptedSockets.AsReadOnly(); } 
		}
		public SocketEventArgs SocketEventArgs { get; set; }

		private List<Socket> _acceptedSockets = new List<Socket>();
		private List<Socket> _clientSockets = new List<Socket>();

		static object _lock = new object();
	}
}
