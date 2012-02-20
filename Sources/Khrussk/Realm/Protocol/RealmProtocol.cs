
namespace Khrussk.Realm.Protocol {
	using Khrussk.Peers;
	using System.Collections.Generic;
	using System;

	public class RealmProtocol : Protocol {
		public RealmProtocol() {	
			RegisterPacketType(typeof(HandshakePacket), new HandshakePacketSerializer());
			RegisterPacketType(typeof(AddEntityPacket), new AddEntityPacketSerializer(_serializer));
			RegisterPacketType(typeof(RemoveEntityPacket), new RemoveEntityPacketSerializer(_serializer));
			RegisterPacketType(typeof(SyncEntityPacket), new SyncEntityPacketSerializer(_serializer));
		}

		public void RegisterEntityType(Type type, IEntitySerializer serializer) {
			_serializer.RegisterEntityType(type, serializer);
		}

		private EntitySerializer _serializer = new EntitySerializer();
	}
}
