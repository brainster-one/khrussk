
namespace Khrussk.Peers {
	using System;
	using System.IO;
	using System.Net;
	using Sockets;

	/// <summary>ClientPeer to connect to ListenerPeer.</summary>
	public sealed class Peer /*: IDisposable */ {
		/// <summary>Initializes a new instance of the ClientPeer class using the specified socket and protocol.</summary>
		/// <param name="socket">Socket.</param>
		/// <param name="protocol">Protocol.</param>
		internal Peer(Socket socket, IProtocol protocol) {
			_socket = socket;
			_protocol = protocol;
			_socket.Connected += new EventHandler<SocketEventArgs>(_socket_Connected);
			_socket.Disconnected += new EventHandler<SocketEventArgs>(_socket_Disconnected);
			_socket.ConnectionFailed += new EventHandler<SocketEventArgs>(_socket_ConnectionFailed);
			_socket.DataReceived += new EventHandler<SocketEventArgs>(_socket_DataReceived);
		}

		/// <summary>Initializes a new instance of the ClientPeer class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public Peer(IProtocol protocol) {
			_protocol = protocol;
			_socket = new Socket();
			_socket.Connected += new EventHandler<SocketEventArgs>(_socket_Connected);
			_socket.Disconnected += new EventHandler<SocketEventArgs>(_socket_Disconnected);
			_socket.ConnectionFailed += new EventHandler<SocketEventArgs>(_socket_ConnectionFailed);
			_socket.DataReceived += new EventHandler<SocketEventArgs>(_socket_DataReceived);
		}
		/*
		/// <summary>Releases the unmanaged resources used by the current socket, and optionally releases the managed resources also.</summary>
		public void Dispose() {
			if (_socket == null) return;
			_socket.Dispose();
			_sendStream.Dispose();
		}
		*/
		/// <summary>Connects client to service.</summary>
		public void Connect(EndPoint host) {
			if (IsConnected) throw new InvalidOperationException("Peer already connected");
			_socket.Connect(host);
		}

		/// <summary>Sends packet to service.</summary>
		/// <param name="packet">Packet to send.</param>
		public void Send(IPacket packet) {
			var m = new MemoryStream();
			_protocol.Write(m, packet);
			_socket.Send(m.ToArray(), (int)m.Length);
		}

		/// <summary>Disconnects client from service.</summary>
		public void Disconnect() {
			_socket.Disconnect();
		}

		/// <summary>Gets connection state.</summary>
		public bool IsConnected {
			get { return _socket.IsConnected; }
		}

		/// <summary>Connection established.</summary>
		public event EventHandler<PeerEventArgs> Connected;
		public event EventHandler<PeerEventArgs> ConnectionFailed;

		/// <summary>Connection closed.</summary>
		public event EventHandler<PeerEventArgs> Disconnected;

		/// <summary>Packet received.</summary>
		public event EventHandler<PeerEventArgs> PacketReceived;

		void _socket_Connected(object sender, SocketEventArgs e) {
			var evnt = Connected;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.Connection, this));
		}

		void _socket_ConnectionFailed(object sender, SocketEventArgs e) {
			var evnt = ConnectionFailed;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.Connection, this));
		}

		void _socket_Disconnected(object sender, SocketEventArgs e) {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.Disconnection, this));
		}

		void _socket_DataReceived(object sender, SocketEventArgs e) {
			var oldPos = _receiveStream.Position;
			_receiveStream.Write(e.Buffer, 0, e.Buffer.Length);
			_receiveStream.Position = oldPos;
			//if (_protocol.canRead)
			var packet = _protocol.Read(_receiveStream);
			
			var evnt = PacketReceived;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.PacketReceived, this, packet));
		}

		/// <summary>Underlying socket.</summary>
		Socket _socket;

		/// <summary>Protocol.</summary>
		readonly IProtocol _protocol;

		/// <summary>Stream handles data to write to socket.</summary>
		readonly MemoryStream _receiveStream = new MemoryStream();
	}
}
