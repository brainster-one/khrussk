
namespace Khrussk.Peers {
	using System;
	
	/// <summary>Peer event type.</summary>
	public enum PeerEventType {
		Connection,
		Disconnection,
		PacketReceived
	}

	// todo rename to networkeventargs
	/// <summary>Services event args.</summary>
	public sealed class PeerEventArgs : EventArgs {
		/// <summary>Initializes a new instance of the PeerEventArgs class.</summary>
		/// <param name="type">Event type.</param>
		/// <param name="peer">Peer.</param>
		public PeerEventArgs(PeerEventType type, Peer peer) {
			EventType = type;
			Peer = peer;
		}

		/// <summary>Initializes a new instance of the PeerEventArgs class.</summary>
		/// <param name="type">Event type.</param>
		/// <param name="peer">Peer.</param>
		/// <param name="packet">Packet.</param>
		public PeerEventArgs(PeerEventType type, Peer peer, object packet) {
			EventType = type;
			Peer = peer;
			Packet = packet;
		}

		/// <summary>Gets client.</summary>
		public PeerEventType EventType { get; private set; }

		/// <summary>Gets client.</summary>
		public Peer Peer { get; private set; }

		/// <summary>Gets packet.</summary>
		public object Packet { get; private set; }
	}
}
