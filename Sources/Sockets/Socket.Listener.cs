
namespace Khrussk.Sockets {
	using System;
	using System.Net;
	using System.Net.Sockets;
	
	/// <summary>Socket.</summary>
	public sealed partial class Socket {
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


		/// <summary>New connection has been accepted.</summary>
		public event EventHandler<SocketEventArgs> ConnectionAccepted;
		
		void BeginAccept() {
			var evnt = new SocketAsyncEventArgs();
			_socket.AcceptAsync(evnt);
			evnt.Completed += OnAcceptComplete;
			
		}

		void OnAcceptComplete(object sender, SocketAsyncEventArgs e) {
			var clientSocket = new Socket(e.AcceptSocket);
			var evnt = ConnectionAccepted;
			if (evnt != null) evnt(this, new SocketEventArgs(clientSocket, ConnectionState.Connected));
			BeginAccept();
		}
		#endif
	}
}
