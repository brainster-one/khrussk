
namespace Khrussk.Peers.Events {
	/// <summary>Default peer event dispatcher.</summary>
	public sealed class PeerEventDispatcher : IPeerEventDispatcher {
		/// <summary>Dispatch event.</summary>
		/// <param name="e">Peer event.</param>
		/// <param name="handler">Event handler to dispatch event to.</param>
		public void Dispatch(PeerEventArgs e, IPeerEventHandler handler) {
			handler.Handle(e);
		}
	}
}
