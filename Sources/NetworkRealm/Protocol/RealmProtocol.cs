
namespace Khrussk.NetworkRealm.Protocol {
	using Peers;

	/// <summary>Realm protocol.</summary>
	public class RealmProtocol : Protocol {
		/// <summary>Initializes a new instance of the RealmProtocol class.</summary>
		public RealmProtocol() {
			Register(new HandshakePacketSerializer());
			Register(new AddEntityPacketSerializer(_serializer));
			Register(new RemoveEntityPacketSerializer());
			Register(new SyncEntityPacketSerializer(_serializer));
		}

		/// <summary>Registeres entity type and serializer.</summary>
		/// <param name="serializer">Entity serializer.</param>
		public void Register<T>(IEntitySerializer<T> serializer) {
			_serializer.Register(serializer);
		}

		/// <summary>Entity serializer.</summary>
		readonly EntitySerializer _serializer = new EntitySerializer();
	}
}
