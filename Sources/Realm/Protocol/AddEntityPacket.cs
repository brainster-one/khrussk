
namespace Khrussk.Realm.Protocol {
	using System.IO;
	using Khrussk.Peers;

	/// <summary>Add entity packet.</summary>
	sealed class AddEntityPacket : IPacket {
		/// <summary>Initializes a new instance of the AddEntityPacket class.</summary>
		/// <param name="entity">Entity.</param>
		public AddEntityPacket(IEntity entity) {
			Entity = entity;
		}

		/// <summary>Gets or sets entity.</summary>
		public IEntity Entity { get; private set; }
	}

	/// <summary>AddEntityPacket serializer.</summary>
	sealed class AddEntityPacketSerializer : IPacketSerializer {
		/// <summary>AddEntity packet serializer.</summary>
		/// <param name="serializer">Entity serializer to serialize entity into stream.</param>
		public AddEntityPacketSerializer(IEntitySerializer serializer) {
			_serializer = serializer;
		}

		/// <summary>Deserializes packet from stream.</summary>
		/// <param name="reader">Reader to deserialize packet by.</param>
		/// <returns>Packet.</returns>
		public IPacket Deserialize(BinaryReader reader) {
			IEntity entity = null;
			_serializer.Deserialize(reader, ref entity);
			return new AddEntityPacket(entity);

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
