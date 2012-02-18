
namespace Khrussk.Peers.Events {
	using System;

	/// <summary>Peer event dispatch info.</summary>
	sealed class PeerEventHandlerInfo {
		public PeerEventHandlerInfo(PeerEventType eventType, IPeerEventHandler handler) {
			EventType = eventType;
			Handler = handler;
		}
		public PeerEventHandlerInfo(PeerEventType eventType, IPeerEventHandler behavior, Type packetType) {
			EventType = eventType;
			Handler = behavior;
			PacketType = packetType;
		}
		public PeerEventType EventType { get; private set; }
		public IPeerEventHandler Handler { get; private set; }
		public Type PacketType { get; private set; }
	}
}