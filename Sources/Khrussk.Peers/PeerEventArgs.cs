
namespace Khrussk.Peers {
	using System;

	/// <summary>Services event args.</summary>
	public sealed class PeerEventArgs : EventArgs {
		/// <summary>Initializes a new instance of the PeerEventArgs class.</summary>
		/// <param name="client">Client.</param>
		public PeerEventArgs(ClientPeer client) {
			Client = client;
		}

		/// <summary>Initializes a new instance of the PeerEventArgs class.</summary>
		/// <param name="client">Client.</param>
		/// <param name="packet">Packet.</param>
		public PeerEventArgs(ClientPeer client, IPacket packet) {
			Client = client;
			Packet = packet;
		}
		
		/// <summary>Gets client.</summary>
		public ClientPeer Client { get; private set; }

		/// <summary>Gets packet.</summary>
		public IPacket Packet { get; private set; }
	}
}
