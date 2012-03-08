
namespace Khrussk.Realm {
	using System;
	using System.Net;
	using Khrussk.Peers;
	using Khrussk.Realm.Protocol;

	/// <summary>Realm client.</summary>
	public sealed class RealmClient {
		/// <summary>Initializes a new instance of the RealmService class.</summary>
		public RealmClient() {
			Protocol = new RealmProtocol();
			_peer = new Peer(Protocol);
			_peer.Connected += new EventHandler<PeerEventArgs>(_peer_Connected);
			_peer.Disconnected += new EventHandler<PeerEventArgs>(_peer_Disconnected);
			_peer.PacketReceived += new EventHandler<PeerEventArgs>(_peer_PacketReceived);
		}

		/// <summary>Connects to remote RealmService.</summary>
		/// <param name="endpoint">Endpoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			_peer.Connect(endpoint);
		}

		public void Disconnect() {
			_peer.Disconnect();
		}

		public event EventHandler<RealmEventArgs> Connected;
		public event EventHandler<RealmEventArgs> Disconnected;
		public event EventHandler<RealmEventArgs> EntityAdded;
		public event EventHandler<RealmEventArgs> EntityRemoved;
		public event EventHandler<RealmEventArgs> EntityModified;


		void _peer_Connected(object sender, PeerEventArgs e) {
			_peer.Send(new HandshakePacket(Guid.Empty));
		}

		void _peer_Disconnected(object sender, PeerEventArgs e) {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new RealmEventArgs());
		}

		void _peer_PacketReceived(object sender, PeerEventArgs e) {
			if (e.Packet is HandshakePacket) {
				var session = ((HandshakePacket)e.Packet).Session;
				var evnt = Connected;
				if (evnt != null) evnt(this, new RealmEventArgs(session));
			} else if (e.Packet is AddEntityPacket) {
				var evnt = EntityAdded;
				if (evnt != null) evnt(this, new RealmEventArgs(((AddEntityPacket)e.Packet).Entity));
			} else if (e.Packet is RemoveEntityPacket) {
				var evnt = EntityRemoved;
				if (evnt != null) evnt(this, new RealmEventArgs(((RemoveEntityPacket)e.Packet).EntityId));
			} else if (e.Packet is SyncEntityPacket) {
				var evnt = EntityModified;
				var packet = (SyncEntityPacket)e.Packet;
				if (evnt != null) evnt(this, new RealmEventArgs(packet.EntityId, packet.Diff));
			}
		}

		/// <summary>Gets protocol.</summary>
		public RealmProtocol Protocol { get; private set; }

		private Peer _peer;
	}
}
