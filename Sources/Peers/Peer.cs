
namespace Khrussk.Peers {
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using Sockets;

	/// <summary>Peer to connect to Listener.</summary>
	public sealed class Peer : IDisposable {
		/// <summary>Initializes a new instance of the Peer class using the specified socket and protocol.</summary>
		/// <param name="socket">Socket.</param>
		/// <param name="protocol">Protocol.</param>
		internal Peer(Socket socket, IProtocol protocol) {
			_socket = socket;
			_protocol = protocol;
			_socket.ConnectionStateChanged += OnSocketConnectionStateChanged;
			_socket.DataReceived += OnSocketDataReceived;
		}

		/// <summary>Initializes a new instance of the Peer class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public Peer(IProtocol protocol) {
			_protocol = protocol;
			_socket = new Socket();
			_socket.ConnectionStateChanged += OnSocketConnectionStateChanged;
			_socket.DataReceived += OnSocketDataReceived;
		}

		/// <summary>Releases the unmanaged resources used by the current peer, and optionally releases the managed resources also.</summary>
		public void Dispose() {
			_socket.Dispose();
			_receiveStream.Dispose();
		}

		/// <summary>Connects client to service.</summary>
		public void Connect(EndPoint host) {
			_socket.Connect(host);
		}

		/// <summary>Sends packet to service.</summary>
		/// <param name="packet">Packet to send.</param>
		public void Send(object packet) {
			try {
				var m = new MemoryStream();
				_protocol.Write(m, packet);
				_socket.Send(m.ToArray(), 0, (int)m.Length);
			} catch {
				Disconnect();
			}
		}

		/// <summary>Disconnects client from service.</summary>
		public void Disconnect() {
			if (!_disconnected) {
				_disconnected = true;
				_socket.Disconnect();
			}
		}

		/// <summary>Connection state changed.</summary>
		public event EventHandler<PeerEventArgs> ConnectionStateChanged;

		/// <summary>Packet received.</summary>
		public event EventHandler<PeerEventArgs> PacketReceived;

		void OnSocketConnectionStateChanged(object sender, SocketEventArgs e) {
			if (ConnectionStateChanged == null) return;
			var evnt = ConnectionStateChanged;
			evnt(this, new PeerEventArgs(this, e.ConnectionState));
		}

		void OnSocketDataReceived(object sender, SocketEventArgs e) {
			lock (_receiveLock) {
				try {
					// Stores data into temporary stream
					var oldPos = _receiveStream.Position;
					_receiveStream.Write(e.Buffer, 0, e.Buffer.Length);
					_receiveStream.Position = oldPos;
				} catch (Exception ex) {
					Debug.WriteLine(ex.Message);
					Disconnect();
				}

				while (true) {
					object packet = null;
					try {
						packet = _protocol.Read(_receiveStream);
						if (packet == null) break;

						var evnt = PacketReceived;
						if (evnt != null) evnt(this, new PeerEventArgs(this, packet));
					} catch (Exception ex) {
						Debug.WriteLine(packet + ": " + ex.Message);
						Disconnect();
					}
				}
			}
		}
		/// <summary>Underlying socket.</summary>
		readonly Socket _socket;

		/// <summary>Protocol.</summary>
		readonly IProtocol _protocol;

		/// <summary>Stream handles data to write to socket.</summary>
		readonly MemoryStream _receiveStream = new MemoryStream();

		bool _disconnected;
		readonly object _receiveLock = new object();
	}
}
