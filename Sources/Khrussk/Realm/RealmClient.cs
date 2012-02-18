using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khrussk.Peers;
using Khrussk.Realm.Protocol;
using System.Net;

namespace Khrussk.Realm {
	public sealed class RealmClient {
		public RealmClient() {
			_peer = new Peer(new RealmProtocol());
			_peer.Connected += new EventHandler<PeerEventArgs>(_peer_Connected);
			_peer.Disconnected += new EventHandler<PeerEventArgs>(_peer_Disconnected);
			_peer.PacketReceived += new EventHandler<PeerEventArgs>(_peer_PacketReceived);
		}

		public void Connect(EndPoint endpoint) {
			_peer.Connect(endpoint);
		}

		public event EventHandler<RealmServiceEventArgs> Connected;

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
	}
}
