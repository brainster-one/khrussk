
namespace Khrussk.Sockets {
	using System;
	using System.Net;
	using System.Net.Sockets;

	/// <summary>Socket.</summary>
	public sealed class Socket {
		/// <summary>Initializes a new instance of the Socket class using the specified socket.</summary>
		/// <param name="socket">.net socket instance.</param>
		internal Socket(System.Net.Sockets.Socket socket) {
			_socket = socket;
			_socket.NoDelay = true;
			BeginReceive();
		}

		/// <summary>Initializes a new instance of the ClientSocket class using.</summary>
		public Socket() {
			_socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
			_socket.NoDelay = true;
		}

		/// <summary>Connects socket to specified endpoint.</summary>
		/// <param name="endpoint">Endpoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (_socket.Connected) throw new InvalidOperationException("Socket connected already.");
			BeginConnect(endpoint);
		}

		/// <summary>Associates a socket with a local endpoint.</summary>
		/// <param name="endpoint">Endpoint.</param>
		public void Listen(IPEndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (_socket.IsBound) throw new InvalidOperationException("Socket in listen state already.");

			_socket.Bind(endpoint);
			_socket.Listen(5);

			BeginAccept();
		}

		/// <summary>Disconnects socket.</summary>
		public void Disconnect() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += OnDisconnectComplete;
			_socket.DisconnectAsync(evnt);
		}

		/// <summary>Sends data to remote host.</summary>
		/// <param name="buffer">Data to send.</param>
		/// <param name="count">Amount of bytes to send.</param>
		public void Send(byte[] buffer, int count) {
			// TODO copy buffer to temp storage
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplete);
			evnt.SetBuffer(buffer, 0, count);
			_socket.SendAsync(evnt);
		}

		public event EventHandler<SocketEventArgs> Connected;
		public event EventHandler<SocketEventArgs> ConnectionFailed;
		public event EventHandler<SocketEventArgs> Disconnected;
		public event EventHandler<SocketEventArgs> DataReceived;
		public event EventHandler<SocketEventArgs> ClientSocketAccepted;

		void BeginAccept() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += OnAcceptComplete;
			_socket.AcceptAsync(evnt);
		}

		void BeginConnect(EndPoint endpoint) {
			var evnt = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectComplete);
			_socket.ConnectAsync(evnt);
		}

		void BeginReceive() {
			var evnt = new SocketAsyncEventArgs();
			evnt.SetBuffer(_receiveBuffer, 0, 255);
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveComplete);
			_socket.ReceiveAsync(evnt);
		}

		void OnConnectComplete(object sender, SocketAsyncEventArgs e) {
			if (e.SocketError == SocketError.Success) {
				var evnt = Connected;
				if (evnt != null) evnt(this, new SocketEventArgs(this));
				BeginReceive();
			} else {
				var evnt = ConnectionFailed;
				if (evnt != null) evnt(this, new SocketEventArgs(this));
			}
		}

		void OnDisconnectComplete(object sender, SocketAsyncEventArgs e) {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new SocketEventArgs(this));
		}
		
		void OnSendComplete(object sender, SocketAsyncEventArgs e) {
			if (e.SocketError != SocketError.Success) Disconnect();
		}

		void OnReceiveComplete(object sender, SocketAsyncEventArgs e) {
			if (e.SocketError == SocketError.Success && e.BytesTransferred > 0) {
				var evnt = DataReceived;
				if (evnt != null) evnt(this, new SocketEventArgs(this, e.Buffer, e.BytesTransferred));
			} else if (e.SocketError != SocketError.Success) {
				var evnt = Disconnected;
				if (evnt != null) evnt(this, new SocketEventArgs(this));
			}
			BeginReceive();
		}

		void OnAcceptComplete(object sender, SocketAsyncEventArgs e) {
			var clientSocket = new Socket(e.AcceptSocket);
			var evnt = ClientSocketAccepted;
			if (evnt != null) evnt(this, new SocketEventArgs(clientSocket));
			BeginAccept();
		}
		
		
		System.Net.Sockets.Socket _socket;
		byte[] _receiveBuffer = new byte[255];
	}
}
