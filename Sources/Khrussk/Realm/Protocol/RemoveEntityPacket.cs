
namespace Khrussk.Realm.Protocol {
	using System.IO;
	using Khrussk.Peers;

	/// <summary>Add entity packet.</summary>
	sealed class RemoveEntityPacket : IPacket {
		/// <summary>Initializes a new instance of the AddEntityPacket class.</summary>
		/// <param name="entity">Entity.</param>
		public RemoveEntityPacket(IEntity entity) {
			EntityId = entity.Id;
		}

		/// <summary>Initializes a new instance of the AddEntityPacket class.</summary>
		/// <param name="entity">Entity.</param>
		public RemoveEntityPacket(int id) {
			EntityId = id;
		}

		/// <summary>Gets or sets entity.</summary>
		public int EntityId { get; private set; }
	}

	/// <summary>AddEntityPacket serializer.</summary>
	sealed class RemoveEntityPacketSerializer : IPacketSerializer {
		/// <summary>AddEntity packet serializer.</summary>
		/// <param name="serializer">Entity serializer to serialize entity into stream.</param>
		public RemoveEntityPacketSerializer(IEntitySerializer serializer) {
			_serializer = serializer;
		}

		/// <summary>Deserializes packet from stream.</summary>
		/// <param name="reader">Reader to deserialize packet by.</param>
		/// <returns>Packet.</returns>
		public IPacket Deserialize(BinaryReader reader) {
			var id = reader.ReadInt32();
			return new RemoveEntityPacket(id);
		}

		/// <summary>Serializes packet to stream.</summary>
		/// <param name="writer">Writer to serialize packet.</param>
		/// <param name="packet">Packet to write.</param>
		/// <returns>Packet.</returns>
		public void Serialize(BinaryWriter writer, IPacket packet) {
			_serializer.Serialize(writer, ((AddEntityPacket)packet).Entity);
		}

		/// <summary>Entity serializer.</summary>
		readonly IEntitySerializer _serializer;
	}
}
