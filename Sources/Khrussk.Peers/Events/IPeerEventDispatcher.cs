
namespace Khrussk.Peers.Events {
	/// <summary>Peer event dispatcher interface.</summary>
	public interface IPeerEventDispatcher {
		/// <summary>Dispatch event.</summary>
		/// <param name="e">Peer event.</param>
		/// <param name="handler">Event handler to dispatch event to.</param>
		void Dispatch(PeerEventArgs e, IPeerEventHandler handler);
	}
}
