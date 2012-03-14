
namespace Khrussk.Tests.Peers {
	using System.Collections.Generic;
	using System.Linq;
	using Khrussk.Peers;
	using Khrussk.Tests.Peers.Protocol;

	/// <summary>Context for tests.</summary>
	sealed class PeerTestContext : BasicTestContext {
		/// <summary>Initializes a new instance of the SocketTestContext class.</summary>
		public PeerTestContext() {
			// Client
			var peer = new Peer(new TestProtocol());
			peer.Connected += OnClientSocketConnected;
			peer.PacketReceived += OnPacketReceived;
			peer.Disconnected += OnClientPeerDisconnected;
			_clientPeers.Add(peer);
			
			// Listener socket
			Listener = new Listener(new TestProtocol());
			Listener.ClientPeerConnected += OnClientPeerConnected;
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
			_clientPeers.Add(np);
			return np;
		}

		/// <summary>Gets listener.</summary>
		public Listener Listener { get; private set; }

		/// <summary>Gets client.</summary>
		public Peer Peer {
			get { return _clientPeers.First(); }
		}

		/// <summary>Gets list of client's peers.</summary>
		public IEnumerable<Peer> ClientPeers {
			get { return _clientPeers.AsReadOnly(); }
		}

		/// <summary>Gets list of accepted peers.</summary>
		public IEnumerable<Peer> AcceptedPeers {
			get { return _acceptedPeers.AsReadOnly(); }
		}

		/// <summary>Gets list of received packets.</summary>
		public IEnumerable<object> Packets {
			get { return _packetReceived.AsReadOnly(); }
		}

		/// <summary>On client socket connected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientSocketConnected(object sender, PeerEventArgs e) {
			Wait.Set();
		}

		/// <summary>On connection accepted</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientPeerConnected(object sender, PeerEventArgs e) {
			e.Peer.PacketReceived+= OnPacketReceived;
			e.Peer.Disconnected += OnAcceptedPeerDisconnected;
			_acceptedPeers.Add(e.Peer);
			Wait.Set();
		}

		/// <summary>On socket disconnected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnAcceptedPeerDisconnected(object sender, PeerEventArgs e) {
			if (_acceptedPeers.Contains(e.Peer)) _acceptedPeers.Remove(e.Peer);
		}


		/// <summary>On socket disconnected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientPeerDisconnected(object sender, PeerEventArgs e) {
			if (_clientPeers.Contains(e.Peer)) _clientPeers.Remove(e.Peer);
		}

		/// <summary>On data received.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			_packetReceived.Add(e.Packet);
		}

		/// <summary>List of accepted peers.</summary>
		private List<Peer> _acceptedPeers = new List<Peer>();

		/// <summary>List of client's peers.</summary>
		private List<Peer> _clientPeers = new List<Peer>();

		/// <summary>List of received packets.</summary>
		private List<object> _packetReceived = new List<object>();
	}
}
