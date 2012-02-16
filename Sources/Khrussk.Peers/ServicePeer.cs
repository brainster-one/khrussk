

namespace Khrussk.Peers {
	using System;
	using System.Net;
	using Core;
	using Sockets;

	/// <summary>Service peer.</summary>
	public sealed class ServicePeer : IDisposable {
		/// <summary>Initializes a new instance of the ServicePeer class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public ServicePeer(IProtocol protocol) {
			_protocol = protocol;
		}

		/// <summary>Releases the unmanaged resources used by the current socket, and optionally releases the managed resources also.</summary>
		public void Dispose() {
			if (_socket != null) _socket.Dispose();
		}

		/// <summary>Associates a peer with a local endpoint.</summary>
		/// <param name="endpoint">Endpoint.</param>
		public void Listen(IPEndPoint endpoint) {
			_socket = new ServiceSocket();
			_socket.Listen(endpoint);
			Thread.Start("ServicePeer." + endpoint + "." + GetHashCode(), x => {
				var connectedSocket = _socket.Accept();
				var client = new ClientPeer(connectedSocket, _protocol);
				OnClinetConnected(client);
			});
		}

		/// <summary>Closes peer.</summary>
		public void Close() {
			if (_socket != null) _socket.Close();
		}

		/// <summary>Gets connection state.</summary>
		public bool IsActive {
			get { return _socket != null && _socket.IsActive; }
		}

		/// <summary>On peer connected.</summary>
		public event EventHandler<PeerEventArgs> ClientConnected;

		/// <summary>On peer connected.</summary>
		/// <param name="client">Client's peer.</param>
		private void OnClinetConnected(ClientPeer client) {
			var evnt = ClientConnected;
			if (evnt != null) evnt(this, new PeerEventArgs(client));
		}

		/// <summary>Protocol.</summary>
		readonly IProtocol _protocol;

		/// <summary>Underlying socket.</summary>
		ServiceSocket _socket;
	}
}
