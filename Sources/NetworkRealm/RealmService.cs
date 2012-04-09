
namespace Khrussk.NetworkRealm {
	using System;
	using System.Net;
	using Helpers;
	using Peers;
	using Protocol;

	/// <summary>Realm service.</summary>
	public sealed class RealmService {
		/// <summary>Initializes a new instance of the RealmService class.</summary>
		/// <param name="protocol">Protocol.</param>
		public RealmService(RealmProtocol protocol) {
			_protocol = protocol;
			_peer = new Listener(_protocol);
			_peer.PeerConnected += OnPeerConnectionAccepted;
		}

		/// <summary>Starts service.</summary>
		/// <param name="endpoint">Endpoint to listen on.</param>
		public void Start(IPEndPoint endpoint) {
			_peer.Listen(endpoint);
		}

		/// <summary>Stops the service.</summary>
		public void Stop() {
			_peer.Disconnect();
		}

		/// <summary>Disconnects user from service.</summary>
		/// <param name="user">User to disconnect.</param>
		public void Disconnect(User user) {
			_users.GetPeer(user).Disconnect();
		}

		public void Send(User user, object packet) {
			var peer = _users.GetPeer(user);
			peer.Send(packet);
		}

		/// <summary>Sends packet to all connected clients.</summary>
		/// <param name="packet">Packet to send.</param>
		public void SendAll(object packet) {
			foreach (Peer peer in _users.Peers) {
				peer.Send(packet);
			}
		}

		/// <summary>Adds antity to realm.</summary>
		/// <param name="entity">Entity to add.</param>
		public void AddEntity(object entity) {
			var id = _entities.Add(entity);
			SendAll(new AddEntityPacket(id, entity));
		}

		/// <summary>Removes entity from realm.</summary>
		/// <param name="entity">Entity to remove.</param>
		public void RemoveEntity(object entity) {
			var id = _entities.Remove(entity);
			SendAll(new RemoveEntityPacket(id));
		}

		/// <summary>Syncs entities for all users.</summary>
		/// <param name="entity">Entity to sync.</param>
		public void ModifyEntity(object entity) {
			var id = _entities.GetId(entity);
			SendAll(new SyncEntityPacket(id, entity));
		}

		/// <summary>New user connected to realm.</summary>
		public event EventHandler<ConnectionEventArgs> UserConnectionStateChanged;

		/// <summary>Custom packet received.</summary>
		public event EventHandler<PacketEventArgs> PacketReceived;

		/// <summary>New peer connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">event args.</param>
		void OnPeerConnectionAccepted(object sender, PeerEventArgs e) {
			e.Peer.PacketReceived += OnPacketReceived;
			e.Peer.ConnectionStateChanged += OnPeerConnectionStateChanged;
		}

		/// <summary>Client disconnected from service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPeerConnectionStateChanged(object sender, PeerEventArgs e) {
			if (e.ConnectionState != ConnectionState.Disconnected) return;
			if (!_users.IsPeerStored(e.Peer)) return;

			e.Peer.PacketReceived -= OnPacketReceived;
			e.Peer.ConnectionStateChanged -= OnPeerConnectionStateChanged;
			var evnt = UserConnectionStateChanged;
			if (evnt != null) evnt(this, new ConnectionEventArgs(_users.GetUser(e.Peer), ConnectionState.Disconnected));
		}

		/// <summary>Packet received from client.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			var peer = e.Peer;
			var packet = e.Packet;

			if (e.Packet is HandshakePacket) {
				peer.Send(new HandshakePacket(Guid.NewGuid()));
				var session = ((HandshakePacket)packet).Session;
				var user = new User(session);
				_users.Map(user, peer);

				// TODO Move it to another place
				foreach (var entity in _entities.Entities) {
					var id = _entities.GetId(entity);
					e.Peer.Send(new AddEntityPacket(id, entity));
				}
				//

				var evnt = UserConnectionStateChanged;
				if (evnt != null) evnt(this, new ConnectionEventArgs(user, ConnectionState.Connected));

			} else {
				var user = _users.GetUser(peer);
				var evnt = PacketReceived;
				if (evnt != null) evnt(this, new PacketEventArgs(packet, user));
			}
		}

		/// <summary>Underlaying service.</summary>
		readonly Listener _peer;

		/// <summary>Gets protocol.</summary>
		readonly RealmProtocol _protocol;

		/// <summary>Peer to user map.</summary>
		readonly UserPeerMap _users = new UserPeerMap();

		/// <summary>Stores entity and ids.</summary>
		readonly EntityIdMap _entities = new EntityIdMap();
	}
}
