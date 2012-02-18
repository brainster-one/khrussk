
namespace Khrussk.Peers.Events {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	// TODO Не все ивенты отвязываются
	public sealed class ListenerEventConfiguration {
		public ListenerEventConfiguration(Listener listener, IPeerEventDispatcher dispatcher) {
			_listener = listener;
			_dispatcher = dispatcher;
			_listener.ClientPeerConnected += OnPeerConnectedHandler;
		}

		void Bind(PeerEventType eventType, IPeerEventHandler eventHandler) {
			_handlers.Add(new PeerEventHandlerInfo(eventType, eventHandler));
		}

		void Bind(Type packetType, IPeerEventHandler eventHandler) {
			_handlers.Add(new PeerEventHandlerInfo(PeerEventType.PacketReceived, eventHandler, packetType));
		}
		
		// ----- Handlers -----

		void OnPeerConnectedHandler(object sender, PeerEventArgs e) {
			e.Peer.Disconnected += OnPeerDisconnectedHandler;
			e.Peer.PacketReceived += OnPacketReceivedHandler;
			_handlers.Where(x => x.EventType == PeerEventType.Connection)
				.ToList().ForEach(x => _dispatcher.Dispatch(e, x.Handler));
		}

		void OnPeerDisconnectedHandler(object sender, PeerEventArgs e) {
			e.Peer.Disconnected -= OnPeerDisconnectedHandler;
			e.Peer.PacketReceived -= OnPacketReceivedHandler;
			_handlers.Where(x => x.EventType == PeerEventType.Disconnection)
				.ToList().ForEach(x => _dispatcher.Dispatch(e, x.Handler));
		}

		void OnPacketReceivedHandler(object sender, PeerEventArgs e) {
			_handlers
				.Where(x => x.EventType == PeerEventType.PacketReceived)
				.Where(x => x.PacketType == null || x.PacketType.Equals(e.Packet.GetType())).ToList()
				.ForEach(x => _dispatcher.Dispatch(e, x.Handler));
		}

		Listener _listener;
		IPeerEventDispatcher _dispatcher;
		readonly List<PeerEventHandlerInfo> _handlers = new List<PeerEventHandlerInfo>();

	}
}
