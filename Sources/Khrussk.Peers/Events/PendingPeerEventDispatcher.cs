
namespace Khrussk.Peers.Events {
	using System.Collections.Generic;

	/// <summary>Peer's event default handler handler.</summary>
	public sealed class PendingPeerEventDispatcher : IPeerEventDispatcher {
		/// <summary>Dispatch event.</summary>
		/// <param name="e">Peer event.</param>
		/// <param name="handler">Event handler to dispatch event to.</param>
		public void Dispatch(PeerEventArgs e, IPeerEventHandler handler) {
			lock (_pendingEvents) {
				_pendingEvents.Add(new KeyValuePair<PeerEventArgs, IPeerEventHandler>(e, handler));
			}
		}

		/// <summary>Dispatches pending events.</summary>
		public void DispatchPendingEvents() {
			lock (_pendingEvents) {
				_pendingEvents.ForEach(x => x.Value.Handle(x.Key));
				_pendingEvents.Clear();
			}
		}

		/// <summary>Pending events.</summary>
		readonly List<KeyValuePair<PeerEventArgs, IPeerEventHandler>> _pendingEvents =
			new List<KeyValuePair<PeerEventArgs, IPeerEventHandler>>();
	}
}
