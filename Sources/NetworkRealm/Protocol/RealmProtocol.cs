
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using Khrussk.Peers;

	/// <summary>Realm protocol.</summary>
	public class RealmProtocol : Protocol {
		/// <summary>Initializes a new instance of the RealmProtocol class.</summary>
		public RealmProtocol() {
			Register<HandshakePacket>(new HandshakePacketSerializer());
			Register<AddEntityPacket>(new AddEntityPacketSerializer(_serializer));
			Register<RemoveEntityPacket>(new RemoveEntityPacketSerializer());
			Register<SyncEntityPacket>(new SyncEntityPacketSerializer(_serializer));
		}

		/// <summary>Registeres entity type and serializer.</summary>
		/// <param name="type">Entity type.</param>
		/// <param name="serializer">Entity serializer.</param>
		public void Register<T>(IEntitySerializer<T> serializer) {
			_serializer.Register<T>(serializer);
		}

		/// <summary>Entity serializer.</summary>
		private EntitySerializer _serializer = new EntitySerializer();
	}
}
