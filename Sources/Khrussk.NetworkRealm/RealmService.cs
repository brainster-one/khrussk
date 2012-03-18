
namespace Khrussk.NetworkRealm {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Khrussk.NetworkRealm.Protocol;
	using Khrussk.Peers;
	using Khrussk.Services;

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
			var peer = _peerUserMap.FirstOrDefault(x => x.Value == user);
			if (peer.Key == null)
				throw new InvalidOperationException(String.Format("User '{0}' is not connected", user));
			else
				peer.Key.Disconnect();
		}

		/// <summary>Adds antity to realm.</summary>
		/// <param name="entity">Entity to add.</param>
		public void AddEntity(object entity) {
			var id = _store.Add(entity);
			_service.SendAll(new AddEntityPacket(id, entity));
		}

		/// <summary>Removes entity from realm.</summary>
		/// <param name="entity">Entity to remove.</param>
		public void RemoveEntity(object entity) {
			var id = _store.Remove(entity);
			_service.SendAll(new RemoveEntityPacket(id));
		}

		/// <summary>Syncs entities for all users.</summary>
		/// <param name="entity">Entity to sync.</param>
		public void ModifyEntity(object entity) {
			var id = _store.GetId(entity);
			_service.SendAll(new SyncEntityPacket(id, entity));
		}

		/// <summary>New user connected to realm.</summary>
		public event EventHandler<RealmEventArgs> UserConnected;

		/// <summary>User disconnected.</summary>
		public event EventHandler<RealmEventArgs> UserDisconnected;

		/// <summary>Custom packet received.</summary>
		public event EventHandler<RealmEventArgs> PacketReceived;

		/// <summary>Client disconnected from service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientDisconnected(object sender, PeerEventArgs e) {
			var evnt = UserDisconnected;
			if (evnt != null) {
				var user = _peerUserMap.FirstOrDefault(x => x.Key == e.Peer);
				evnt(this, new RealmEventArgs { User = user.Value });
			}
		}
		
		/// <summary>Packet received from client.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnPacketReceived(object sender, PeerEventArgs e) {
			if (e.Packet is HandshakePacket) {
				e.Peer.Send(new HandshakePacket(Guid.NewGuid()));
				var session = (e.Packet as HandshakePacket).Session;
				var user = new User(session);
				_peerUserMap[e.Peer] = user;

				// TODO Move it to another place
				foreach (var entity in _store.Entities) {
					var id = _store.GetId(entity);
					e.Peer.Send(new AddEntityPacket(id, entity));
				}
				//

				var evnt = UserConnected;
				if (evnt != null) evnt(this, new RealmEventArgs { User = user });
			} else {
				var user = _peerUserMap[e.Peer];
				var evnt = PacketReceived;
				if (evnt != null) evnt(this, new RealmEventArgs { Packet = e.Packet, User = user });
			}
		}

		/// <summary>Gets protocol.</summary>
		public RealmProtocol Protocol { get; private set; }
		
		/// <summary>Underlaying service.</summary>
		private Service _service;

		/// <summary>Peer to user map.</summary>
		private Dictionary<Peer, User> _peerUserMap = new Dictionary<Peer,User>();

		/// <summary>Stores entity and ids.</summary>
		EntityIdMap _store = new EntityIdMap();
	}
}
