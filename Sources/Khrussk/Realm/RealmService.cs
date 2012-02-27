
namespace Khrussk.Realm {
	using System;
	using System.Net;
	using Khrussk.Realm.Protocol;
	using Khrussk.Services;

	public sealed class RealmService {
		public RealmService() {
			_protocol = new RealmProtocol();
			_service = new Service(_protocol);
			_service.PacketReceived += new System.EventHandler<Peers.PeerEventArgs>(_service_PacketReceived);
		}

		public void RegisterEntityType(Type type, IEntitySerializer serializer) {
			_protocol.RegisterEntityType(type, serializer);
		}

		public void Start(IPEndPoint endpoint) {
			_service.Start(endpoint);
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

		public event EventHandler<RealmServiceEventArgs> UserConnected;
		public event EventHandler<RealmServiceEventArgs> PacketReceived;

		/*void _service_ClientConnected(object sender, Peers.PeerEventArgs e) {
			throw new System.NotImplementedException();
		}

		void _service_ClientDisconnected(object sender, Peers.PeerEventArgs e) {
			throw new System.NotImplementedException();
		}*/

		void _service_PacketReceived(object sender, Peers.PeerEventArgs e) {
			if (e.Packet is HandshakePacket) {
				e.Peer.Send(new HandshakePacket(Guid.NewGuid()));
				var session = (e.Packet as HandshakePacket).Session;
				var evnt = UserConnected;
				if (evnt != null) evnt(this, new RealmServiceEventArgs(new User(session)));
			}
		}

		private RealmProtocol _protocol;
		private Service _service;
	}
}
