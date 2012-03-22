
namespace Khrussk.Sockets {
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Diagnostics;

	/// <summary>Socket.</summary>
	public sealed partial class Socket : IDisposable {
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

		/// <summary>Releases the unmanaged resources used by the current socket, and optionally releases the managed resources also.</summary>
		public void Dispose() {
			_socket.Dispose();
		}

		/// <summary>Connects socket to specified endpoint.</summary>
		/// <param name="endpoint">EndPoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint", "Endpoint can not be null.");
			if (_socket.Connected) throw new InvalidOperationException("Socket connected already.");
			BeginConnect(endpoint);
		}

		/// <summary>Disconnects socket.</summary>
		public void Disconnect() {
			#if !SILVERLIGHT
			BeginDisconnect();
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

		/// <summary>Connection state has been changed.</summary>
		public event EventHandler<SocketEventArgs> ConnectionStateChanged;
		
		/// <summary>Data from remote host has been received.</summary>
		public event EventHandler<SocketEventArgs> DataReceived;

		void BeginConnect(EndPoint endpoint) {
			var evnt = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
			evnt.Completed += OnConnectComplete;
			_socket.ConnectAsync(evnt);
		}

		#if !SILVERLIGHT
		void BeginDisconnect() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += OnDisconnectComplete;
			_socket.DisconnectAsync(evnt);
		}
		#endif

		void BeginReceive() {
			var evnt = new SocketAsyncEventArgs();
			evnt.SetBuffer(_receiveBuffer, 0, _receiveBuffer.Length);
			evnt.Completed += OnReceiveComplete;
			_socket.ReceiveAsync(evnt);
		}

		void OnConnectComplete(object sender, SocketAsyncEventArgs e) {
			if (e.SocketError == SocketError.Success) {
				OnConnectionStateChanged(ConnectionState.Connected);
				BeginReceive();
			} else
				OnConnectionStateChanged(ConnectionState.Failed);
		}

		void OnDisconnectComplete(object sender, SocketAsyncEventArgs e) {
			var evnt = ConnectionStateChanged;
			if (evnt != null) evnt(this, new SocketEventArgs(this, ConnectionState.Disconnected));
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
			} else
				OnConnectionStateChanged(ConnectionState.Disconnected);
		}

		void OnConnectionStateChanged(ConnectionState connectionState) {
			var evnt = ConnectionStateChanged;
			if (evnt != null) evnt(this, new SocketEventArgs(this, connectionState));
		}
		
		/// <summary>Underlaying socket.</summary>
		System.Net.Sockets.Socket _socket;

		/// <summary>Receive buffer.</summary>
		byte[] _receiveBuffer = new byte[1024 * 8];
	}
}
