
namespace Khrussk.Realm {
	using System;
	using System.Net;
	using Khrussk.Realm.Protocol;
	using Khrussk.Services;
	using Khrussk.Peers;
	using System.Linq;
	using System.Collections.Generic;

	public sealed class RealmService {
		public RealmService() {
			_protocol = new RealmProtocol();
			_service = new Service(_protocol);
			_service.PacketReceived += new System.EventHandler<Peers.PeerEventArgs>(_service_PacketReceived);
			_service.ClientDisconnected += new EventHandler<Peers.PeerEventArgs>(_service_ClientDisconnected);
		}

		public void RegisterEntityType(Type type, IEntitySerializer serializer) {
			_protocol.RegisterEntityType(type, serializer);
		}

		public void Start(IPEndPoint endpoint) {
			_service.Start(endpoint);
		}

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

		private RealmProtocol _protocol;
		private Service _service;
		private Dictionary<Peer, User> _peerUserMap = new Dictionary<Peer,User>();
	}
}
