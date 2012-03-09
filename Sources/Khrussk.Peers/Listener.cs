
namespace Khrussk.Peers {
	using System;
	using System.Net;
	using Sockets;

	/// <summary>Service peer.</summary>
	public sealed class Listener /*: IDisposable */ {
		/// <summary>Initializes a new instance of the ListenerPeer class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public Listener(IProtocol protocol) {
			_protocol = protocol;
			_socket = new Socket();
			_socket.ConnectionAccepted += new EventHandler<SocketEventArgs>(_socket_ClientSocketAccepted);
		}

		/*
		/// <summary>Releases the unmanaged resources used by the current socket, and optionally releases the managed resources also.</summary>
		public void Dispose() {
			if (_socket != null) _socket.Dispose();
		}
		*/
		/// <summary>Associates a peer with a local endpoint.</summary>
		/// <param name="endpoint">Endpoint.</param>
		public void Listen(IPEndPoint endpoint) {
			_socket.Listen(endpoint);
		}

		/// <summary>Closes peer.</summary>
		public void Disconnect() {
			_socket.Disconnect();
		}

		/*
		/// <summary>Gets connection state.</summary>
		public bool IsActive {
			get { return _socket.; }
		}
		*/

		/// <summary>On peer connected.</summary>
		public event EventHandler<PeerEventArgs> ClientPeerConnected;

		/// <summary>On peer connected.</summary>
		/// <param name="client">Client's peer.</param>
		void _socket_ClientSocketAccepted(object sender, SocketEventArgs e) {
			var evnt = ClientPeerConnected;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.Connection, new Peer(e.Socket, _protocol)));
		}

		/// <summary>Protocol.</summary>
		readonly IProtocol _protocol;

		/// <summary>Underlying socket.</summary>
		Socket _socket;
	}
}
