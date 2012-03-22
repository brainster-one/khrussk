
namespace Khrussk.Peers {
	using System;
	
	/// <summary>Services event args.</summary>
	public sealed class PeerEventArgs : EventArgs {
		/// <summary>Initializes a new instance of the PeerEventArgs class using the specified peer and connection state.</summary>
		/// <param name="peer">Peer.</param>
		/// <param name="connectionState">Socket connection state.</param>
		internal PeerEventArgs(Peer peer, ConnectionState connectionState) {
			Peer = peer;
			ConnectionState = connectionState;
		}

		/// <summary>Initializes a new instance of the PeerEventArgs class using the specified peer and packet.</summary>
		/// <param name="socket">Peer.</param>
		/// <param name="buffer">Packet.</param>
		internal PeerEventArgs(Peer peer, object packet) {
			Peer = peer;
			Packet = packet;
		}
		
		/// <summary>Gets client.</summary>
		public ConnectionState ConnectionState { get; private set; }

		/// <summary>Gets client.</summary>
		public Peer Peer { get; private set; }

		/// <summary>Gets packet.</summary>
		public object Packet { get; private set; }
	}
}
