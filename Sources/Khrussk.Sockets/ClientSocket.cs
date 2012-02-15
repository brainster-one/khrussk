using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Khrussk.Sockets {
	public sealed class ClientSocket {
		internal ClientSocket(Socket socket) {
			_socket = socket;
		}

		public ClientSocket() {
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
		}

		public void Connect(EndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (IsConnected) throw new InvalidOperationException("Socket connected already.");
			BeginConnect(endpoint);
		}

		public void Disconnect() {
			if (_socket == null) return;

			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(disconnect_Completed);
			_socket.DisconnectAsync(evnt);
		}

		public bool IsConnected {
			get { return _socket != null && _socket.Connected; }
		}

		public event EventHandler<SocketEventArgs> Connected;
		public event EventHandler<SocketEventArgs> ConnectionFailed;
		public event EventHandler<SocketEventArgs> Disconnected;

		void BeginConnect(EndPoint endpoint) {
			var evnt = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(evnt_Completed);
			_socket.ConnectAsync(evnt);
		}

		void evnt_Completed(object sender, SocketAsyncEventArgs e) {
			var evnt = e.SocketError == SocketError.Success ? Connected : ConnectionFailed;
			if (evnt != null) evnt(this, new SocketEventArgs(this));
		}

		void disconnect_Completed(object sender, SocketAsyncEventArgs e) {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new SocketEventArgs(this));
		}

		Socket _socket;
	}
}
