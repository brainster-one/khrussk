
namespace Khrussk.Realm {
	using System;
	using System.Net;
	using Khrussk.Realm.Protocol;
	using Khrussk.Services;

	public sealed class RealmService {
		public RealmService() {
			_serializer = new EntitySerializer();
			_protocol = new RealmProtocol(_serializer);
			_service = new Service(new RealmProtocol(_serializer));
			_service.PacketReceived += new System.EventHandler<Peers.PeerEventArgs>(_service_PacketReceived);
		}

		public void Start() {
			_service.Start(new IPEndPoint(IPAddress.Any, 9876));
		}

		public void AddEntity(IEntity entity) {
			_service.SendAll(new AddEntityPacket(entity));
			//_service.SendAll();
		}

		public void RemoveEntity(IEntity entity) {

		}

		public void ModifyEntity(IEntity entity) {

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

		private IEntitySerializer _serializer;
		private RealmProtocol _protocol;
		private Service _service;
	}
}
