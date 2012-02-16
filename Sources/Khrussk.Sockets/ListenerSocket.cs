
namespace Khrussk.Sockets {
	using System;
	using System.Net;
	using System.Net.Sockets;

	/// <summary>Socket for server side.</summary>
	sealed class ListenerSocket {
		/// <summary>Initializes a new instance of the ListenerSocket class.</summary>
		public ListenerSocket() {
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		/// <summary>Associates a socket with a local endpoint.</summary>
		/// <param name="endpoint">Endpoint.</param>
		public void Listen(IPEndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (_socket != null && _socket.IsBound) throw new InvalidOperationException("Socket in listen state already.");

			_socket.Bind(endpoint);
			_socket.Listen(5);

			BeginAccept();
		}

		public void Disconnect() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(OnDisconnectComplete);
			_socket.DisconnectAsync(evnt);
		}

		public event EventHandler<SocketEventArgs> ClientSocketAccepted;
		public event EventHandler<SocketEventArgs> Disconnected;

		void BeginAccept() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += OnAcceptComplete;
			_socket.AcceptAsync(evnt);
		}

		void OnAcceptComplete(object sender, SocketAsyncEventArgs e) {
			var clientSocket = new ClientSocket(e.AcceptSocket);
			var evnt = ClientSocketAccepted;
			if (evnt != null) evnt(this, new SocketEventArgs(clientSocket));
			BeginAccept();
		}

		void OnDisconnectComplete(object sender, SocketAsyncEventArgs e) {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new SocketEventArgs());
		}

		Socket _socket;
	}
}
