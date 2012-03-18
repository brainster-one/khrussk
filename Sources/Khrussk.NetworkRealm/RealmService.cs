
namespace Khrussk.NetworkRealm {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Khrussk.NetworkRealm.Protocol;
	using Khrussk.Peers;
	using Khrussk.Services;
	using Khrussk.NetworkRealm.Helpers;

	/// <summary>Realm service.</summary>
	public sealed class RealmService {
		/// <summary>Initializes a new instance of the RealmService class.</summary>
		public RealmService() {
			Protocol = new RealmProtocol();
			_service = new Service(Protocol);
			_service.PacketReceived += OnPacketReceived;
			_service.ClientDisconnected += OnClientDisconnected;
		}

		/// <summary>Starts service.</summary>
		/// <param name="endpoint">Endpoint to listen on.</param>
		public void Start(IPEndPoint endpoint) {
			_service.Start(endpoint);
		}

		/// <summary>Stops the service.</summary>
		public void Stop() {
			_service.Stop();
		}

		/// <summary>Disconnects user from service.</summary>
		/// <param name="user">User to disconnect.</param>
		public void Disconnect(User user) {
			_users.GetPeer(user).Disconnect();
		}

		/// <summary>Adds antity to realm.</summary>
		/// <param name="entity">Entity to add.</param>
		public void AddEntity(object entity) {
			var id = _entities.Add(entity);
			_service.SendAll(new AddEntityPacket(id, entity));
		}

		/// <summary>Removes entity from realm.</summary>
		/// <param name="entity">Entity to remove.</param>
		public void RemoveEntity(object entity) {
			var id = _entities.Remove(entity);
			_service.SendAll(new RemoveEntityPacket(id));
		}

		/// <summary>Syncs entities for all users.</summary>
		/// <param name="entity">Entity to sync.</param>
		public void ModifyEntity(object entity) {
			var id = _entities.GetId(entity);
			_service.SendAll(new SyncEntityPacket(id, entity));
		}

		/// <summary>New user connected to realm.</summary>
		public event EventHandler<RealmServiceEventArgs> UserConnected;

		/// <summary>User disconnected.</summary>
		public event EventHandler<RealmServiceEventArgs> UserDisconnected;

		/// <summary>Custom packet received.</summary>
		public event EventHandler<RealmServiceEventArgs> PacketReceived;

		/// <summary>Client disconnected from service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientDisconnected(object sender, PeerEventArgs e) {
			var evnt = UserDisconnected;
			if (evnt != null) evnt(this, new RealmServiceEventArgs { User = _users.GetUser(e.Peer) });
		}
		
		/// <summary>Packet received from client.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			var peer = e.Peer;
			var packet = e.Packet;

			if (e.Packet is HandshakePacket) {
				peer.Send(new HandshakePacket(Guid.NewGuid()));
				var session = (packet as HandshakePacket).Session;
				var user = new User(session);
				_users.Map(user, peer);
				

				// TODO Move it to another place
				foreach (var entity in _entities.Entities) {
					var id = _entities.GetId(entity);
					e.Peer.Send(new AddEntityPacket(id, entity));
				}
				//

				var evnt = UserConnected;
				if (evnt != null) evnt(this, new RealmServiceEventArgs { User = user });
			} else {
				var user = _users.GetUser(peer);
				var evnt = PacketReceived;
				if (evnt != null) evnt(this, new RealmServiceEventArgs { User = user, Packet = packet });
			}
		}

		/// <summary>Gets protocol.</summary>
		public RealmProtocol Protocol { get; private set; }
		
		/// <summary>Underlaying service.</summary>
		private Service _service;

		/// <summary>Peer to user map.</summary>
		UserPeerMap _users = new UserPeerMap();

		/// <summary>Stores entity and ids.</summary>
		EntityIdMap _entities = new EntityIdMap();
	}
}
