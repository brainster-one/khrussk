
namespace Khrussk.Sockets {
	using System;
	using System.Net;
	using System.Net.Sockets;

	/// <summary>Socket for client side.</summary>
	public sealed class ClientSocket {
		/// <summary>Initializes a new instance of the ClientSocket class using the specified socket.</summary>
		/// <param name="socket">.net socket instance.</param>
		internal ClientSocket(Socket socket) {
			_socket = socket;
			_socket.NoDelay = true;
			BeginReceive();
		}

		/// <summary>Initializes a new instance of the ClientSocket class using.</summary>
		public ClientSocket() {
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
			_socket.NoDelay = true;
		}

		/// <summary>Connects socket to specified endpoint.</summary>
		/// <param name="endpoint">Endpoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (IsConnected) throw new InvalidOperationException("Socket connected already.");
			BeginConnect(endpoint);
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

		/// <summary>Gets socket connection state.</summary>
		public bool IsConnected {
			get { return _socket != null && _socket.Connected; }
		}

		public event EventHandler<SocketEventArgs> Connected;
		public event EventHandler<SocketEventArgs> ConnectionFailed;
		public event EventHandler<SocketEventArgs> Disconnected;
		public event EventHandler<SocketEventArgs> DataReceived;

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
		
		
		Socket _socket;
		byte[] _receiveBuffer = new byte[255];
	}
}
