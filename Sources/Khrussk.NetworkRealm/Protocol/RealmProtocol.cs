
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using Khrussk.Peers;

	/// <summary>Realm protocol.</summary>
	public class RealmProtocol : Protocol {
		/// <summary>Initializes a new instance of the RealmProtocol class.</summary>
		public RealmProtocol() {
			RegisterPacketType(typeof(HandshakePacket), new HandshakePacketSerializer());
			RegisterPacketType(typeof(AddEntityPacket), new AddEntityPacketSerializer(_serializer));
			RegisterPacketType(typeof(RemoveEntityPacket), new RemoveEntityPacketSerializer(_serializer));
			RegisterPacketType(typeof(SyncEntityPacket), new SyncEntityPacketSerializer(_serializer));
		}

		/// <summary>Registeres entity type and serializer.</summary>
		/// <param name="type">Entity type.</param>
		/// <param name="serializer">Entity serializer.</param>
		public void RegisterEntityType(Type type, IEntitySerializer serializer) {
			_serializer.RegisterEntityType(type, serializer);
		}

		/// <summary>Entity serializer.</summary>
		private EntitySerializer _serializer = new EntitySerializer();
	}
}
