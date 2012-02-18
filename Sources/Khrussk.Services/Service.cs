using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khrussk.Peers;
using System.Net;

namespace Khrussk.Services {
	public class Service {
		public Service(IProtocol protocol) {
			_listener = new Listener(protocol);
			_listener.ClientPeerConnected += new EventHandler<PeerEventArgs>(_listener_ClientPeerConnected);
		}

		public void Start(IPEndPoint endpoint) {
			_listener.Listen(endpoint);
		}

		event EventHandler<PeerEventArgs> ClientConnected;
		event EventHandler<PeerEventArgs> ClientDisconnected;
		event EventHandler<PeerEventArgs> PacketReceived;

		void _listener_ClientPeerConnected(object sender, PeerEventArgs e) {
			e.Peer.Disconnected += new EventHandler<PeerEventArgs>(peer_Disconnected);
			e.Peer.PacketReceived += new EventHandler<PeerEventArgs>(peer_PacketReceived);
			_peers.Add(e.Peer); // todo lock

			var evnt = ClientConnected;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.Connection, e.Peer));
		}

		void peer_PacketReceived(object sender, PeerEventArgs e) {
			var evnt = PacketReceived;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.PacketReceived, e.Peer, e.Packet));
		}

		void peer_Disconnected(object sender, PeerEventArgs e) {
			_peers.Remove(e.Peer); // todo lock
			
			var evnt = ClientDisconnected;
			if (evnt != null) evnt(this, new PeerEventArgs(PeerEventType.Disconnection, e.Peer));
		}

		private Listener _listener;
		private List<Peer> _peers = new List<Peer>();
	}
}
