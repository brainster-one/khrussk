
namespace Khrussk.NetworkRealm {
	using System;
	using System.Net;
	using Khrussk.NetworkRealm.Protocol;
	using Khrussk.Peers;

	/// <summary>Realm client.</summary>
	public sealed class RealmClient {
		/// <summary>Initializes a new instance of the RealmClient class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public RealmClient(RealmProtocol protocol) {
			_peer = new Peer(protocol);
			_peer.ConnectionStateChanged += OnConnected;
			_peer.PacketReceived += OnPacketReceived;
		}

		/// <summary>Connects to remote RealmService.</summary>
		/// <param name="endpoint">Endpoint to connect to.</param>
		public void Connect(EndPoint endpoint) {
			_peer.Connect(endpoint);
		}

		/// <summary>Sends packet to remote service.</summary>
		public void Send(object packet) {
			_peer.Send(packet);
		}

		/// <summary>Disconnects from service.</summary>
		public void Disconnect() {
			_peer.Disconnect();
		}

		/// <summary>Connection state has been changed.</summary>
		public event EventHandler<RealmClientEventArgs> ConnectionStateChanged;

		/// <summary>Entity state has been changed.</summary>
		public event EventHandler<RealmClientEventArgs> EntityStateChanged;

		/// <summary>Just connected to remote service. Send handshake packet.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnConnected(object sender, PeerEventArgs e) {
			_peer.Send(new HandshakePacket(Guid.Empty));
		}

		/// <summary>On connection failed.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnConnectionFailed(object sender, PeerEventArgs e) {
			_connectionState = ConnectionState.Failed;
			var evnt = ConnectionStateChanged;
			if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, ConnectionState = _connectionState });
		}

		/// <summary>Connection with remote service has been closed.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnDisconnected(object sender, PeerEventArgs e) {
			_connectionState = ConnectionState.Disconnected;
			var evnt = ConnectionStateChanged;
			if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, ConnectionState = _connectionState });
		}

		/// <summary>New packet has been received.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			// TODO Refactor this shit.
			if (e.Packet is HandshakePacket) {
				_connectionState = ConnectionState.Connected;
				_session = ((HandshakePacket)e.Packet).Session;
				var evnt = ConnectionStateChanged;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, ConnectionState = _connectionState });
			} else if (e.Packet is AddEntityPacket) {
				var packet = (AddEntityPacket)e.Packet;
				var evnt = EntityStateChanged;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, EntityInfo = new EntityInfo { Id = packet.EntityId, Entity = packet.Entity, Action = EntityNetworkAction.Added } });
			} else if (e.Packet is RemoveEntityPacket) {
				var packet = (RemoveEntityPacket)e.Packet;
				var evnt = EntityStateChanged;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, EntityInfo = new EntityInfo { Id = packet.EntityId, Action = EntityNetworkAction.Removed } });
			} else if (e.Packet is SyncEntityPacket) {
				var packet = (SyncEntityPacket)e.Packet;
				var evnt = EntityStateChanged;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, EntityInfo = new EntityInfo { Id = packet.EntityId, Diff = packet.Diff, Action = EntityNetworkAction.Modified } });
			}
		}

		/// <summary>Session.</summary>
		private Guid _session;
		private ConnectionState _connectionState;

		/// <summary>Underlaying peer.</summary>
		private Peer _peer;
	}
}
