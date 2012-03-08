
namespace Khrussk.Realm {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Khrussk.Peers;
	using Khrussk.Realm.Protocol;
	using Khrussk.Services;

	/// <summary>Realm service.</summary>
	public sealed class RealmService {
		/// <summary>Initializes a new instance of the RealmService class.</summary>
		public RealmService() {
			Protocol = new RealmProtocol();
			_service = new Service(Protocol);
			_service.PacketReceived += _service_PacketReceived;
			_service.ClientDisconnected += _service_ClientDisconnected;
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

		public void Disconnect(User user) {
			var peer = _peerUserMap.FirstOrDefault(x => x.Value == user);
			if (peer.Key != null) peer.Key.Disconnect();
			// TODO throw exception - user is not connected
		}

		public void AddEntity(IEntity entity) {
			_service.SendAll(new AddEntityPacket(entity));
			//_service.SendAll();
		}

		public void RemoveEntity(IEntity entity) {
			_service.SendAll(new RemoveEntityPacket(entity));
		}

		public void ModifyEntity(IEntity entity) {
			_service.SendAll(new SyncEntityPacket(entity));
		}

		/*public event EventHandler<RealmServiceEventArgs> ClientConnected;
		public event EventHandler<RealmServiceEventArgs> ClientDisconnected;*/

		public event EventHandler<RealmEventArgs> UserConnected;
		public event EventHandler<RealmEventArgs> UserDisconnected;
		public event EventHandler<RealmEventArgs> PacketReceived;

		/*void _service_ClientConnected(object sender, Peers.PeerEventArgs e) {
			throw new System.NotImplementedException();
		}
		*/
		
		void _service_ClientDisconnected(object sender, Peers.PeerEventArgs e) {
			var evnt = UserDisconnected;
			if (evnt != null) {
				var user = _peerUserMap.FirstOrDefault(x => x.Key == e.Peer);
				evnt(this, new RealmEventArgs(user.Value));
			}
		}

		void _service_PacketReceived(object sender, Peers.PeerEventArgs e) {
			if (e.Packet is HandshakePacket) {
				e.Peer.Send(new HandshakePacket(Guid.NewGuid()));
				var session = (e.Packet as HandshakePacket).Session;
				var user = new User(session);
				_peerUserMap[e.Peer] = user;

				var evnt = UserConnected;
				if (evnt != null) evnt(this, new RealmEventArgs(user));
			}
		}

		public RealmProtocol Protocol { get; private set; }
		private Service _service;
		private Dictionary<Peer, User> _peerUserMap = new Dictionary<Peer,User>();
	}
}
