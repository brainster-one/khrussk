
namespace Khrussk.Tests.Peers.Protocol {
	using Khrussk.Peers;

	class TestProtocol : IProtocol {
		public object Read(System.IO.Stream stream) {
			if (stream.Position >= stream.Length) return null;

			return (object)new Packet {
				Data = (byte)stream.ReadByte()
			};
		}

		public void Write(System.IO.Stream stream, object packet) {
			var p = packet as Packet;
			stream.WriteByte((byte)p.Data);
		}
	}
}
