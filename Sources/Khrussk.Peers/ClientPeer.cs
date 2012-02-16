
namespace Khrussk.Peers {
	using System;
	using System.IO;
	using System.Net;
	using Core;
	using Sockets;

	/// <summary>Client to connect to Service.</summary>
	public sealed class ClientPeer : IDisposable {
		/// <summary>Initializes a new instance of the ClientPeer class using the specified socket and protocol.</summary>
		/// <param name="socket">Socket.</param>
		/// <param name="protocol">Protocol.</param>
		internal ClientPeer(ClientSocket socket, IProtocol protocol) {
			_protocol = protocol;
			_socket = socket;
			StartThreads();
		}

		/// <summary>Initializes a new instance of the ClientPeer class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public ClientPeer(IProtocol protocol) {
			_protocol = protocol;
		}

		/// <summary>Releases the unmanaged resources used by the current socket, and optionally releases the managed resources also.</summary>
		public void Dispose() {
			if (_socket == null) return;
			_socket.Dispose();
			_sendStream.Dispose();
		}

		/// <summary>Connects client to service.</summary>
		public void Connect(EndPoint host) {
			if (IsConnected) throw new InvalidOperationException("Peer already connected");

			_socket = new ClientSocket();
			_socket.Connect(host);
			StartThreads();
			OnConnected();
		}

		/// <summary>Sends packet to service.</summary>
		/// <param name="packet">Packet to send.</param>
		public void Send(IPacket packet) {
			lock (_sendStream) {
				_protocol.Write(_sendStream, packet);
			}
		}

		/// <summary>Disconnects client from service.</summary>
		public void Disconnect() {
			if (_socket != null) {
				_socket.Close();
				OnDisconnected();
			}
		}

		/// <summary>Gets connection state.</summary>
		public bool IsConnected {
			get { return _socket != null && _socket.IsActive; }
		}

		/// <summary>Connection established.</summary>
		public event EventHandler<PeerEventArgs> Connected;

		/// <summary>Connection closed.</summary>
		public event EventHandler<PeerEventArgs> Disconnected;

		/// <summary>Packet received.</summary>
		public event EventHandler<PeerEventArgs> PacketReceived;

		/// <summary>Connection established.</summary>
		private void OnConnected() {
			var evnt = Connected;
			if (evnt != null) evnt(this, new PeerEventArgs(this));
		}

		/// <summary>Connection closed.</summary>
		private void OnDisconnected() {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new PeerEventArgs(this));
		}

		/// <summary>Packed received.</summary>
		/// <param name="packet">Packet.</param>
		private void OnPacketReceived(IPacket packet) {
			var evnt = PacketReceived;
			if (evnt != null) evnt(this, new PeerEventArgs(this, packet));
		}

		/// <summary>Underlying socket.</summary>
		ClientSocket _socket;

		/// <summary>Protocol.</summary>
		readonly IProtocol _protocol;

		/// <summary>Stream handles data to write to socket.</summary>
		readonly MemoryStream _sendStream = new MemoryStream();
	}
}
