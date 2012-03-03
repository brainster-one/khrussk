
namespace Khrussk.Tests.Peers.Protocol {
	using Khrussk.Peers;

	class TestProtocol : IProtocol {
		public IPacket Read(System.IO.Stream stream) {
			return (IPacket)new Packet {
				Data = (byte)stream.ReadByte()
			};
		}

		public void Write(System.IO.Stream stream, IPacket packet) {
			var p = packet as Packet;
			stream.WriteByte((byte)p.Data);
		}
	}
}
