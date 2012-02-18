
namespace Khrussk.Peers.Events {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Diagnostics;

	public sealed class PeerEventsConfiguration {
		private Peer _peer;
		private IPeerEventDispatcher _dispatcher;
		readonly List<PeerEventHandlerInfo> _handlers = new List<PeerEventHandlerInfo>();

		public PeerEventsConfiguration(Peer peer, IPeerEventDispatcher dispatcher) {
			_peer = peer;
			_dispatcher = dispatcher;
			
			_peer.Connected += OnConnectedHandler;
			_peer.Disconnected += OnDisconnectedHandler;
			_peer.PacketReceived += OnPacketReceivedHandler;
		}

		void Bind(PeerEventType eventType, IPeerEventHandler eventHandler) {
			_handlers.Add(new PeerEventHandlerInfo(eventType, eventHandler));
		}

		void Bind(Type packetType, IPeerEventHandler eventHandler) {
			_handlers.Add(new PeerEventHandlerInfo(PeerEventType.PacketReceived, eventHandler, packetType));
		}


		void OnConnectedHandler(object sender, PeerEventArgs e) {
			_handlers.Where(x => x.EventType == PeerEventType.Connection)
				.ToList().ForEach(x => _dispatcher.Dispatch(e, x.Handler));
		}

		void OnDisconnectedHandler(object sender, PeerEventArgs e) {
			_handlers.Where(x => x.EventType == PeerEventType.Disconnection)
				.ToList().ForEach(x => _dispatcher.Dispatch(e, x.Handler));
		}

		void OnPacketReceivedHandler(object sender, PeerEventArgs e) {
			_handlers
				.Where(x => x.EventType == PeerEventType.PacketReceived)
				.Where(x => x.PacketType == null || x.PacketType.Equals(e.Packet.GetType())).ToList()
				.ForEach(x => _dispatcher.Dispatch(e, x.Handler));
		}
	}
}
