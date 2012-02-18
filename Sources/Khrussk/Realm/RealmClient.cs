
namespace Khrussk.Realm {
	using System;
	using System.Net;
	using Khrussk.Peers;
	using Khrussk.Realm.Protocol;

	public sealed class RealmClient {
		public RealmClient() {
			_peer = new Peer(new RealmProtocol(_serializer));
			_peer.Connected += new EventHandler<PeerEventArgs>(_peer_Connected);
			_peer.Disconnected += new EventHandler<PeerEventArgs>(_peer_Disconnected);
			_peer.PacketReceived += new EventHandler<PeerEventArgs>(_peer_PacketReceived);
		}

		/// <summary>Connects to remote RealmService.</summary>
		/// <param name="endpoint">Endpoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			_peer.Connect(endpoint);
		}

		public event EventHandler<RealmServiceEventArgs> Connected;
		public event EventHandler<RealmServiceEventArgs> EntityAdded;
		public event EventHandler<RealmServiceEventArgs> EntityRemoved;
		public event EventHandler<RealmServiceEventArgs> EntityModified;


		void _peer_Connected(object sender, PeerEventArgs e) {
			_peer.Send(new HandshakePacket(Guid.Empty));
		}

		void _peer_Disconnected(object sender, PeerEventArgs e) {
			throw new NotImplementedException();
		}

		void _peer_PacketReceived(object sender, PeerEventArgs e) {
			if (e.Packet is HandshakePacket) {
				var session = ((HandshakePacket)e.Packet).Session;
				var evnt = Connected;
				if (evnt != null) evnt(this, new RealmServiceEventArgs(session));
			}
		}

		private Peer _peer;
		private IEntitySerializer _serializer = new EntitySerializer();
	}
}
