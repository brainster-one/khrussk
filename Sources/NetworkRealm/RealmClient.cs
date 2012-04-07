
namespace Khrussk.NetworkRealm {
	using System;
	using System.Collections.Generic;
	using System.Net;
	using Peers;
	using Protocol;

	/// <summary>Realm client.</summary>
	public sealed class RealmClient {
		/// <summary>Initializes a new instance of the RealmClient class using the specified protocol.</summary>
		/// <param name="protocol">Protocol.</param>
		public RealmClient(RealmProtocol protocol) {
			if (protocol == null) throw new ArgumentNullException("protocol", "Protocol can not be null");

			_peer = new Peer(protocol);
			_peer.ConnectionStateChanged += OnConnectionStateChanged;
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
		public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;

		/// <summary>Entity state has been changed.</summary>
		public event EventHandler<EntityEventArgs> EntityStateChanged;

		/// <summary>Packet has been received.</summary>
		public event EventHandler<PacketEventArgs> PacketReceived;

		/// <summary>Just connected to remote service. Send handshake packet.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnConnectionStateChanged(object sender, PeerEventArgs e) {
			var evnt = ConnectionStateChanged;

			if (e.ConnectionState == ConnectionState.Connected) {
				_peer.Send(new HandshakePacket(Guid.Empty));
				return; // Do not raise ConnectionStateChanged when connected. (Wait for HandshakePacket)
			}

			// Failed to connect/Disconnected
			if (evnt != null) evnt(this, new ConnectionEventArgs(null, e.ConnectionState));
		}

		/// <summary>New packet has been received.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			// TODO Refactor this shit.
			if (e.Packet is HandshakePacket) {
				var evnt = ConnectionStateChanged;
				var packet = (HandshakePacket)e.Packet;
				if (evnt != null) evnt(this, new ConnectionEventArgs(new User(packet.Session), ConnectionState.Connected));

			} else if (e.Packet is AddEntityPacket) {
				var evnt = EntityStateChanged;
				var packet = (AddEntityPacket)e.Packet;
				_entities.Add(packet.EntityId, packet.Entity);
				if (evnt != null) evnt(this, new EntityEventArgs(packet.Entity, EntityNetworkAction.Added));

			} else if (e.Packet is RemoveEntityPacket) {
				var evnt = EntityStateChanged;
				var packet = (RemoveEntityPacket)e.Packet;
				var entity = _entities[packet.EntityId];
				_entities.Remove(packet.EntityId);
				if (evnt != null) evnt(this, new EntityEventArgs(entity, EntityNetworkAction.Removed));

			} else if (e.Packet is SyncEntityPacket) {
				var evnt = EntityStateChanged;
				var packet = (SyncEntityPacket)e.Packet;
				var entity = _entities[packet.EntityId];
				packet.Diff.ApplyChanges(entity);
				if (evnt != null) evnt(this, new EntityEventArgs(entity, EntityNetworkAction.Modified));

			} else {
				var evnt = PacketReceived;
				if (evnt != null) evnt(this, new PacketEventArgs(e.Packet));
			}
		}

		readonly Peer _peer;
		readonly Dictionary<int, object> _entities = new Dictionary<int, object>();
	}
}
