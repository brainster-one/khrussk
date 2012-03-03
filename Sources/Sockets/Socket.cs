
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

		/// <summary>Initializes a new instance of the Socket class.</summary>
		public Socket() {
			_socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
				NoDelay = true
			};
		}

		/// <summary>Connects socket to specified endpoint.</summary>
		/// <param name="endpoint">EndPoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (_socket.Connected) throw new InvalidOperationException("Socket connected already.");
			BeginConnect(endpoint);
		}

		#if !SILVERLIGHT
		/// <summary>Associates a socket with a local endpoint.</summary>
		/// <param name="endpoint">Endpoint to listen on.</param>
		public void Listen(IPEndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (_socket.IsBound) throw new InvalidOperationException("Socket in listen state already.");
			_socket.Bind(endpoint);
			_socket.Listen(5);
			BeginAccept();
		}
		#endif

		/// <summary>Disconnects socket.</summary>
		public void Disconnect() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += OnDisconnectComplete;
			#if !SILVERLIGHT
			_socket.DisconnectAsync(evnt);
			#else
			_socket.Close();
			#endif
		}

		/// <summary>Sends data to remote host.</summary>
		/// <param name="buffer">Data to send.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="count">Amount of bytes to send.</param>
		public void Send(byte[] buffer, int offset, int count) {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += OnSendComplete;
			evnt.SetBuffer(buffer, offset, count);
			_socket.SendAsync(evnt);
		}

		/// <summary>Connection to remote host has been established.</summary>
		public event EventHandler<SocketEventArgs> Connected;

		/// <summary>Connection to remote host failed.</summary>
		public event EventHandler<SocketEventArgs> ConnectionFailed;

		/// <summary>Connection to remote host has been closed.</summary>
		public event EventHandler<SocketEventArgs> Disconnected;

		/// <summary>Data from remote host has been received.</summary>
		public event EventHandler<SocketEventArgs> DataReceived;

		#if !SILVERLIGHT
		/// <summary>New connection has been accepted.</summary>
		public event EventHandler<SocketEventArgs> ConnectionAccepted;
		
		void BeginAccept() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += OnAcceptComplete;
			_socket.AcceptAsync(evnt);
		}
		#endif

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
				if (evnt != null) {
					var buffer = new byte[e.BytesTransferred];
					Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesTransferred);
					evnt(this, new SocketEventArgs(this, buffer));
				}
				BeginReceive();
			} else /*if (e.SocketError != SocketError.Success)*/ {
				var evnt = Disconnected;
				if (evnt != null) evnt(this, new SocketEventArgs(this));
			}
			
		}
		
		#if !SILVERLIGHT
		void OnAcceptComplete(object sender, SocketAsyncEventArgs e) {
			var clientSocket = new Socket(e.AcceptSocket);
			var evnt = ConnectionAccepted;
			if (evnt != null) evnt(this, new SocketEventArgs(clientSocket));
			BeginAccept();
		}
		#endif

		System.Net.Sockets.Socket _socket;
		byte[] _receiveBuffer = new byte[255];
	}
}
