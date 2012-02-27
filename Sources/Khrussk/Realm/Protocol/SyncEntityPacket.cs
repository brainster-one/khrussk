
namespace Khrussk.Realm.Protocol {
	using System.IO;
	using Khrussk.Peers;
	using System.Collections.Generic;
	using System.Linq;
	using System;

	/// <summary>Add entity packet.</summary>
	sealed class SyncEntityPacket : IPacket {
		/// <summary>Initializes a new instance of the AddEntityPacket class.</summary>
		/// <param name="entity">Entity.</param>
		public SyncEntityPacket(IEntity entity) {
			Entity = entity;
		}

		internal SyncEntityPacket(int entityId, EntityDiffData data) {
			EntityId = entityId;
			Diff = data;
		}

		/// <summary>Gets or sets entity.</summary>
		public IEntity Entity { get; private set; }
		public int EntityId { get; private set; }
		public EntityDiffData Diff { get; private set; }
	}

	/// <summary>AddEntityPacket serializer.</summary>
	sealed class SyncEntityPacketSerializer : IPacketSerializer {
		/// <summary>AddEntity packet serializer.</summary>
		/// <param name="serializer">Entity serializer to serialize entity into stream.</param>
		public SyncEntityPacketSerializer(IEntitySerializer serializer) {
			_serializer = serializer;
		}

		/// <summary>Deserializes packet from stream.</summary>
		/// <param name="reader">Reader to deserialize packet by.</param>
		/// <returns>Packet.</returns>
		public IPacket Deserialize(BinaryReader reader) {
			// TODO Make copy of reader. Потомучтод  данные могут быть прочитаны позже
			var entityId = reader.ReadInt16();
			var entityDiffData = new EntityDiffData(_serializer, reader);
			return new SyncEntityPacket(entityId, entityDiffData);
		}

		/// <summary>Serializes packet to stream.</summary>
		/// <param name="writer">Writer to serialize packet.</param>
		/// <param name="packet">Packet to write.</param>
		/// <returns>Packet.</returns>
		public void Serialize(BinaryWriter writer, IPacket packet) {
			writer.Write((Int16)((SyncEntityPacket)packet).Entity.Id);
			_serializer.Serialize(writer, ((SyncEntityPacket)packet).Entity);
		}

		/// <summary>Entity serializer.</summary>
		readonly IEntitySerializer _serializer;
	}
}
