
namespace Khrussk.Tests.Peers.Protocol {
	using Khrussk.Peers;

	class TestProtocol : Protocol {
		public TestProtocol() {
			Register(new PacketSerializer());
		}
	}


}
