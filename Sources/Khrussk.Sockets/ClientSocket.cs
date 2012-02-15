
namespace Khrussk.Sockets {
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Collections.Generic;
	using System.Linq;
	using System.Diagnostics;

	public sealed class ClientSocket {
		internal ClientSocket(Socket socket) {
			_socket = socket;
			_socket.NoDelay = true;
			BeginReceive();
			//BeginSend();
		}

		public ClientSocket() {
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
			_socket.NoDelay = true;
		}

		public void Connect(EndPoint endpoint) {
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (IsConnected) throw new InvalidOperationException("Socket connected already.");
			BeginConnect(endpoint);
		}

		public void Disconnect() {
			if (_socket == null) return;

			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(OnDisconnectComplete);
			_socket.DisconnectAsync(evnt);
		}

		public void Send(byte[] buffer, int count) {
			var evnt = new SocketAsyncEventArgs();
			evnt.SetBuffer(buffer, 0, count);
			//evnt.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplete);
			_socket.SendAsync(evnt);
		}

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

		/*void BeginSend() {
			Debug.Print("SENDED: " + _send.Count);
			var evnt = new SocketAsyncEventArgs();
			evnt.BufferList = _send;
			//evnt.SetBuffer(new byte[200], 0, 200);
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplete);
			_socket.SendAsync(evnt);
		}*/

		void OnConnectComplete(object sender, SocketAsyncEventArgs e) {
			Debug.Print("cc");
			var evnt = e.SocketError == SocketError.Success ? Connected : ConnectionFailed;
			if (evnt != null) evnt(this, new SocketEventArgs(this));
			BeginReceive();
			//BeginSend();
		}

		void OnDisconnectComplete(object sender, SocketAsyncEventArgs e) {
			Debug.Print("dc");
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new SocketEventArgs(this));
		}

		void OnReceiveComplete(object sender, SocketAsyncEventArgs e) {
			Debug.Print("rc" + e.BytesTransferred);
			var evnt = e.SocketError == SocketError.Success ? DataReceived : Disconnected;
			if (evnt != null && e.BytesTransferred > 0) evnt(this, new SocketEventArgs(this, e.Buffer, e.BytesTransferred));
			System.Threading.Thread.Sleep(500);
			BeginReceive();
		}
		/*
		void OnSendComplete(object sender, SocketAsyncEventArgs e) {
			lock (_send) { _send.Clear(); }
			BeginSend();
		}
		*/
		
		Socket _socket;
		List<ArraySegment<byte>> _send = new List<ArraySegment<byte>>();
		byte[] _receiveBuffer = new byte[255];
	}
}
