
namespace Khrussk.Examples.Chat {
	using System.IO;
	using Peers;

	class MessagePacket {
		public string Content { get; set; }
	}

	class MessagePacketSerializer : IPacketSerializer<MessagePacket> {
		public MessagePacket Deserialize(BinaryReader reader) {
			return new MessagePacket {
				Content = reader.ReadString()
			};
		}

		public void Serialize(BinaryWriter writer, MessagePacket packet) {
			writer.Write(packet.Content);
		}
	}
}
