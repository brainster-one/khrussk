﻿
namespace Khrussk.NetworkRealm {
	using System;
	using System.Net;
	using Khrussk.Peers;
	using Khrussk.NetworkRealm.Protocol;

	/// <summary>Realm client.</summary>
	public sealed class RealmClient {
		/// <summary>Initializes a new instance of the RealmService class.</summary>
		public RealmClient() {
			Protocol = new RealmProtocol();
			_peer = new Peer(Protocol);
			_peer.Connected += OnConnected;
			_peer.ConnectionFailed += OnConnectionFailed;
			_peer.Disconnected += OnDisconnected;
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

		/// <summary>Connected to remote service.</summary>
		public event EventHandler<RealmClientEventArgs> Connected;

		/// <summary>Connection failed.</summary>
		public event EventHandler<RealmClientEventArgs> ConnectionFailed;

		/// <summary>Connection has been closed.</summary>
		public event EventHandler<RealmClientEventArgs> Disconnected;

		/// <summary>Entity has benn added into realm.</summary>
		public event EventHandler<RealmClientEventArgs> EntityAdded;

		/// <summary>Entity has been removed from realm.</summary>
		public event EventHandler<RealmClientEventArgs> EntityRemoved;

		/// <summary>Entity has been changed.</summary>
		public event EventHandler<RealmClientEventArgs> EntityModified;

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
			var evnt = ConnectionFailed;
			if (evnt != null) evnt(this, new RealmClientEventArgs());
		}

		/// <summary>Connection with remote service has been closed.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnDisconnected(object sender, PeerEventArgs e) {
			var evnt = Disconnected;
			if (evnt != null) evnt(this, new RealmClientEventArgs());
		}

		/// <summary>New packet has been received.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			// TODO Refactor this shit.
			if (e.Packet is HandshakePacket) {
				_session = ((HandshakePacket)e.Packet).Session;
				var evnt = Connected;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session });
			} else if (e.Packet is AddEntityPacket) {
				var packet = (AddEntityPacket)e.Packet;
				var evnt = EntityAdded;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, EntityInfo = new EntityInfo { Id = packet.EntityId, Entity = packet.Entity } });
			} else if (e.Packet is RemoveEntityPacket) {
				var packet = (RemoveEntityPacket)e.Packet;
				var evnt = EntityRemoved;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, EntityInfo = new EntityInfo { Id = packet.EntityId } });
			} else if (e.Packet is SyncEntityPacket) {
				var packet = (SyncEntityPacket)e.Packet;
				var evnt = EntityModified;
				if (evnt != null) evnt(this, new RealmClientEventArgs { Session = _session, EntityInfo = new EntityInfo { Id = packet.EntityId, Diff = packet.Diff } });
			}
		}

		/// <summary>Gets protocol.</summary>
		public RealmProtocol Protocol { get; private set; }

		/// <summary>Underlaying peer.</summary>
		private Peer _peer;

		private Guid _session;
	}
}
