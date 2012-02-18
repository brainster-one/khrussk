
namespace Khrussk.Realm.Protocol {
	using Khrussk.Peers;

	public class RealmProtocol : Protocol {
		public RealmProtocol(IEntitySerializer _serializer) {
			RegisterPacketType(typeof(HandshakePacket), new HandshakePacketSerializer());
			RegisterPacketType(typeof(AddEntityPacket), new AddEntityPacketSerializer(_serializer));
		}
	}
}
