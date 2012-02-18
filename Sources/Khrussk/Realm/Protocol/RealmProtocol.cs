
namespace Khrussk.Realm.Protocol {
	using Khrussk.Peers;

	public class RealmProtocol : Protocol {
		public RealmProtocol() {
			RegisterPacketType(typeof(HandshakePacket), new HandshakePacketSerializer());
		}
	}
}
