
namespace Khrussk.Peers.Events {
	/// <summary>Peer event handler.</summary>
	public interface IPeerEventHandler {
		/// <summary>Handle event.</summary>
		/// <param name="e">Event args.</param>
		void Handle(PeerEventArgs e);
	}
}
