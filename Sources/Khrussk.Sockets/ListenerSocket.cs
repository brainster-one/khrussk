using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Uberball.Engine.Network.Sockets {
	sealed class ListenerSocket {
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
			evnt.Completed += new EventHandler<SocketAsyncEventArgs>(qqq);
			_socket.DisconnectAsync(evnt);
		}

		public event EventHandler<SocketEventArgs> ClientSocketAccepted;
		public event EventHandler<SocketEventArgs> Disconnected;

		void BeginAccept() {
			var evnt = new SocketAsyncEventArgs();
			evnt.Completed += evnt_Completed;
			_socket.AcceptAsync(evnt);
		}

		void evnt_Completed(object sender, SocketAsyncEventArgs e) {
			var clientSocket = new ClientSocket(e.AcceptSocket);
			var evnt = ClientSocketAccepted;
			if (evnt != null) evnt(this, new SocketEventArgs(clientSocket));
			BeginAccept();
		}

		void qqq(object sender, SocketAsyncEventArgs e) {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new SocketEventArgs());
		}

		Socket _socket;
	}
}
