
namespace Khrussk.Peers {
	using System;
	using System.Net;
	using Sockets;

	/// <summary>Service peer.</summary>
	public sealed class Listener : IDisposable {
		/// <summary>Initializes a new instance of the Listener class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public Listener(IProtocol protocol) {
			_protocol = protocol;
			_socket = new Socket();
			_socket.ConnectionAccepted += OnSocketClientSocketAccepted;
		}

		
		/// <summary>Releases the unmanaged resources used by the current listener, and optionally releases the managed resources also.</summary>
		public void Dispose() {
			_socket.Dispose();
		}
		
		/// <summary>Associates a peer with a local endpoint.</summary>
		/// <param name="endpoint">Endpoint.</param>
		public void Listen(IPEndPoint endpoint) {
			_socket.Listen(endpoint);
		}

		/// <summary>Closes peer.</summary>
		public void Disconnect() {
			_socket.Disconnect();
		}

		/// <summary>On peer connected.</summary>
		public event EventHandler<PeerEventArgs> PeerConnected;

		/// <summary>On connection accepted.</summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnSocketClientSocketAccepted(object sender, SocketEventArgs e) {
			var evnt = PeerConnected;
			if (evnt != null) evnt(this, new PeerEventArgs(new Peer(e.Socket, _protocol), ConnectionState.Connected));
		}

		/// <summary>Protocol.</summary>
		readonly IProtocol _protocol;

		/// <summary>Underlying socket.</summary>
		readonly Socket _socket;
	}
}
