
namespace Khrussk.Tests.UnitTests.Peers.Protocol {
	using Khrussk.Peers;
	using System.IO;

	class Packet {
		public byte Data { get; set; }
	}

	class PacketSerializer : IPacketSerializer<Packet> {
		public Packet Deserialize(BinaryReader reader) {
			return new Packet {
				Data = reader.ReadByte()
			};
		}

		public void Serialize(BinaryWriter writer, Packet packet) {
			writer.Write(packet.Data);
		}
	}
}
