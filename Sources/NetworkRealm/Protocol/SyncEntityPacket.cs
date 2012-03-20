
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.IO;
	using Khrussk.Peers;

	/// <summary>Add entity packet.</summary>
	sealed class SyncEntityPacket {
		/// <summary>Initializes a new instance of the AddEntityPacket class.</summary>
		/// <param name="entity">Entity.</param>
		public SyncEntityPacket(int entityId, object entity) {
			EntityId = entityId;
			Entity = entity;
		}

		internal SyncEntityPacket(int entityId, EntityDiffData diff) {
			EntityId = entityId;
			Diff = diff;
		}

		/// <summary>Gets or sets entity.</summary>
		public int EntityId { get; private set; }
		public object Entity { get; private set; }	
		public EntityDiffData Diff { get; private set; }
	}

	/// <summary>AddEntityPacket serializer.</summary>
	sealed class SyncEntityPacketSerializer : IPacketSerializer<SyncEntityPacket> {
		/// <summary>AddEntity packet serializer.</summary>
		/// <param name="serializer">Entity serializer to serialize entity into stream.</param>
		public SyncEntityPacketSerializer(EntitySerializer serializer) {
			_serializer = serializer;
		}

		/// <summary>Deserializes packet from stream.</summary>
		/// <param name="reader">Reader to deserialize packet by.</param>
		/// <returns>Packet.</returns>
		public SyncEntityPacket Deserialize(BinaryReader reader) {
			// TODO Make copy of reader. Потому что  данные могут быть прочитаны позже
			var entityId = reader.ReadInt16();
			var entityDiffData = new EntityDiffData(_serializer, reader);
			return new SyncEntityPacket(entityId, entityDiffData);
		}

		/// <summary>Serializes packet to stream.</summary>
		/// <param name="writer">Writer to serialize packet.</param>
		/// <param name="packet">Packet to write.</param>
		/// <returns>Packet.</returns>
		public void Serialize(BinaryWriter writer, SyncEntityPacket packet) {
			writer.Write((Int16)packet.EntityId);
			_serializer.Serialize(writer, packet.Entity);
		}

		/// <summary>Entity serializer.</summary>
		readonly EntitySerializer _serializer;
	}
}
