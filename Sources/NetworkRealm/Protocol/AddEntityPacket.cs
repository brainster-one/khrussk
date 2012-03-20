
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.IO;
	using Khrussk.Peers;

	/// <summary>Add entity packet.</summary>
	sealed class AddEntityPacket {
		/// <summary>Initializes a new instance of the AddEntityPacket class.</summary>
		/// <param name="entity">Entity.</param>
		public AddEntityPacket(int entityId, object entity) {
			EntityId = entityId;
			Entity = entity;
		}

		/// <summary>Gets or sets entity.</summary>
		public object Entity { get; private set; }
		public int EntityId { get; private set; }
	}

	/// <summary>AddEntityPacket serializer.</summary>
	sealed class AddEntityPacketSerializer : IPacketSerializer<AddEntityPacket> {
		/// <summary>AddEntity packet serializer.</summary>
		/// <param name="serializer">Entity serializer to serialize entity into stream.</param>
		public AddEntityPacketSerializer(EntitySerializer serializer) {
			_serializer = serializer;
		}

		/// <summary>Deserializes packet from stream.</summary>
		/// <param name="reader">Reader to deserialize packet by.</param>
		/// <returns>Packet.</returns>
		public AddEntityPacket Deserialize(BinaryReader reader) {
			object entity = null;
			var entityId = reader.ReadInt16();
			_serializer.Deserialize(reader, ref entity);
			return new AddEntityPacket(entityId, entity);
		}

		/// <summary>Serializes packet to stream.</summary>
		/// <param name="writer">Writer to serialize packet.</param>
		/// <param name="packet">Packet to write.</param>
		/// <returns>Packet.</returns>
		public void Serialize(BinaryWriter writer, AddEntityPacket packet) {
			writer.Write((Int16)packet.EntityId);
			_serializer.Serialize(writer, packet.Entity);
		}

		/// <summary>Entity serializer.</summary>
		readonly EntitySerializer _serializer;
	}
}
