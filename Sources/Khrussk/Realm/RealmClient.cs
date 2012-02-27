
namespace Khrussk.Realm {
	using System;
	using System.Net;
	using Khrussk.Peers;
	using Khrussk.Realm.Protocol;
	using System.Collections.Generic;

	public sealed class RealmClient {
		public RealmClient() {
			_protocol = new RealmProtocol();
			_peer = new Peer(_protocol);
			_peer.Connected += new EventHandler<PeerEventArgs>(_peer_Connected);
			_peer.Disconnected += new EventHandler<PeerEventArgs>(_peer_Disconnected);
			_peer.PacketReceived += new EventHandler<PeerEventArgs>(_peer_PacketReceived);
		}

		/// <summary>Connects to remote RealmService.</summary>
		/// <param name="endpoint">Endpoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			_peer.Connect(endpoint);
		}

		public void RegisterEntityType(Type type, IEntitySerializer serializer) {
			_protocol.RegisterEntityType(type, serializer);
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
			} else if (e.Packet is AddEntityPacket) {
				var evnt = EntityAdded;
				if (evnt != null) evnt(this, new RealmServiceEventArgs(((AddEntityPacket)e.Packet).Entity));
			} else if (e.Packet is RemoveEntityPacket) {
				var evnt = EntityRemoved;
				if (evnt != null) evnt(this, new RealmServiceEventArgs(((RemoveEntityPacket)e.Packet).EntityId));
			} else if (e.Packet is SyncEntityPacket) {
				var evnt = EntityModified;
				var packet = (SyncEntityPacket)e.Packet;
				if (evnt != null) evnt(this, new RealmServiceEventArgs(packet.EntityId, packet.Diff));
			}
		}

		private Peer _peer;
		private RealmProtocol _protocol;
	}
}
