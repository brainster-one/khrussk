
namespace Khrussk.Tests.Sockets {
	using System.Collections.Generic;
	using System.Linq;
	using Khrussk.Sockets;

	/// <summary>Context for tests.</summary>
	sealed class SocketTestContext : TestContext {
		/// <summary>Initializes a new instance of the SocketTestContext class.</summary>
		public SocketTestContext() {
			// Client
			var client = new Socket();
			client.Connected += OnClientSocketConnected;
			client.DataReceived += OnDataReceived;
			client.Disconnected += OnClientSocketDisconnected;
			_clientSockets.Add(client);
			
			// Listener socket
			ListenerSocket = new Socket();
			ListenerSocket.ConnectionAccepted += OnClientSocketAccepted;
		}

		/// <summary>Cleanup environment.</summary>
		public void Cleanup() {
			ClientSockets.ToList().ForEach(x => x.Disconnect());
			AcceptedSockets.ToList().ForEach(x => x.Disconnect());
			ListenerSocket.Disconnect();
		}

		/// <summary>Creates new socket.</summary>
		/// <returns>New socket.</returns>
		public Socket NewSocket() {
			var ns = new Socket();
			_clientSockets.Add(ns);
			return ns;
		}

		/// <summary>Gets listener socket.</summary>
		public Socket ListenerSocket { get; private set; }

		/// <summary>Gets client socket.</summary>
		public Socket ClientSocket {
			get { return _clientSockets.First(); }
		}

		/// <summary>Gets list of client's sockets.</summary>
		public IEnumerable<Socket> ClientSockets {
			get { return _clientSockets.AsReadOnly(); }
		}

		/// <summary>Gets list of accepted sockets.</summary>
		public IEnumerable<Socket> AcceptedSockets {
			get { return _acceptedSockets.AsReadOnly(); }
		}

		/// <summary>Gets list of received data chunks.</summary>
		public IEnumerable<byte[]> DataReceived {
			get { return _dataReceived.AsReadOnly(); }
		}

		/// <summary>On client socket connected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientSocketConnected(object sender, SocketEventArgs e) {
			Wait.Set();
		}

		/// <summary>On connection accepted</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientSocketAccepted(object sender, SocketEventArgs e) {
			e.Socket.DataReceived += OnDataReceived;
			e.Socket.Disconnected += OnAcceptedSocketDisconnected;
			_acceptedSockets.Add(e.Socket);
			Wait.Set();
		}

		/// <summary>On socket disconnected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnAcceptedSocketDisconnected(object sender, SocketEventArgs e) {
			if (_acceptedSockets.Contains(e.Socket)) _acceptedSockets.Remove(e.Socket);
		}


		/// <summary>On socket disconnected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientSocketDisconnected(object sender, SocketEventArgs e) {
			if (_clientSockets.Contains(e.Socket)) _clientSockets.Remove(e.Socket);
		}

		/// <summary>On data received.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnDataReceived(object sender, SocketEventArgs e) {
			_dataReceived.Add(e.Buffer);
		}

		/// <summary>List of accepted sockets.</summary>
		private List<Socket> _acceptedSockets = new List<Socket>();

		/// <summary>List of client's sockets</summary>
		private List<Socket> _clientSockets = new List<Socket>();

		/// <summary>List of received data chunks.</summary>
		private List<byte[]> _dataReceived = new List<byte[]>();
	}
}
