
namespace Khrussk.Examples.Chat.Protocol {
	using NetworkRealm.Protocol;

	class ChatProtocol : RealmProtocol {
		public ChatProtocol() {
			Register(new PlayerSerializer());
			Register(new MessagePacketSerializer());
		}
	}
}
