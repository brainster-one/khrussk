
namespace Khrussk.Tests.Peers {
	using System.Collections.Generic;
	using System.Linq;
	using Khrussk.Peers;
	using Protocol;

	/// <summary>Context for tests.</summary>
	sealed class PeerTestContext : BasicTestContext {
		/// <summary>Initializes a new instance of the SocketTestContext class.</summary>
		public PeerTestContext() {
			AcceptedPeers = new List<Peer>();
			ClientPeers = new List<Peer>();
			Packets = new List<object>();

			Peer = new Peer(new TestProtocol());
			Peer.PacketReceived += OnPacketReceived;
			
			Listener = new Listener(new TestProtocol());
			Listener.PeerConnected += OnPeerAccepted;

		}

		/// <summary>Cleanup environment.</summary>
		public void Cleanup() {
			ClientPeers.ToList().ForEach(x => x.Disconnect());
			AcceptedPeers.ToList().ForEach(x => x.Disconnect());
			Listener.Disconnect();
		}

		/// <summary>Creates new socket.</summary>
		/// <returns>New socket.</returns>
		public Peer NewSocket() {
			var np = new Peer(new TestProtocol());
			ClientPeers.Add(np);
			return np;
		}

		/// <summary>Gets client.</summary>
		public Peer Peer { get; private set; }

		/// <summary>Gets listener.</summary>
		public Listener Listener { get; private set; }

		/// <summary>List of accepted peers.</summary>
		public List<Peer> AcceptedPeers { get; private set; }

		/// <summary>List of client's peers.</summary>
		public List<Peer> ClientPeers { get; private set; }

		/// <summary>List of received packets.</summary>
		public List<object> Packets { get; private set; }

		/// <summary>On connection accepted</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPeerAccepted(object sender, PeerEventArgs e) {
			e.Peer.PacketReceived += OnPacketReceived;
			e.Peer.ConnectionStateChanged += OnAcceptedPeerConnectionStateChanged;
			AcceptedPeers.Add(e.Peer);
		}

		/// <summary>On data received.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			Packets.Add(e.Packet);
		}

		/// <summary>On socket disconnected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnAcceptedPeerConnectionStateChanged(object sender, PeerEventArgs e) {
			if (e.ConnectionState == ConnectionState.Disconnected) {
				if (AcceptedPeers.Contains(e.Peer)) AcceptedPeers.Remove(e.Peer);
			}
		}
	}
}
