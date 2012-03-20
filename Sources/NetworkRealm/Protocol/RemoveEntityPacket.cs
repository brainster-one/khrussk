
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.IO;
	using Khrussk.Peers;

	/// <summary>Add entity packet.</summary>
	sealed class RemoveEntityPacket {
		/// <summary>Initializes a new instance of the AddEntityPacket class.</summary>
		/// <param name="entity">Entity.</param>
		public RemoveEntityPacket(int entityId) {
			EntityId = entityId;
		}

		/// <summary>Gets or sets entity.</summary>
		public int EntityId { get; private set; }
	}

	/// <summary>AddEntityPacket serializer.</summary>
	sealed class RemoveEntityPacketSerializer : IPacketSerializer<RemoveEntityPacket> {
		/// <summary>Deserializes packet from stream.</summary>
		/// <param name="reader">Reader to deserialize packet by.</param>
		/// <returns>Packet.</returns>
		public RemoveEntityPacket Deserialize(BinaryReader reader) {
			var id = reader.ReadInt16();
			return new RemoveEntityPacket(id);
		}

		/// <summary>Serializes packet to stream.</summary>
		/// <param name="writer">Writer to serialize packet.</param>
		/// <param name="packet">Packet to write.</param>
		/// <returns>Packet.</returns>
		public void Serialize(BinaryWriter writer, RemoveEntityPacket packet) {
			writer.Write((Int16)packet.EntityId);
		}
	}
}
