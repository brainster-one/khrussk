
namespace Khrussk.Realm.Protocol {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	/// <summary>Protocol packet handlers info.</summary>
	sealed class EntitySerializerInfo {
		/// <summary>Gets or sets packet type id.</summary>
		public int EntityTypeId { get; set; }

		/// <summary>Gets or sets packet type.</summary>
		public Type EntityType { get; set; }

		/// <summary>Gets or sets packet streamer.</summary>
		public IEntitySerializer Serializer { get; set; }
	}

	/// <summary>Basic entity serializer.</summary>
	public sealed class EntitySerializer : IEntitySerializer {
		/// <summary>Registers entity serializer.</summary>
		/// <param name="entityType">Entity type.</param>
		/// <param name="serializer">Entity serializer.</param>
		public void RegisterEntityType(Type entityType, IEntitySerializer serializer) {
			_serializers.Add(new EntitySerializerInfo {
				EntityTypeId = _serializers.Count(),
				EntityType = entityType,
				Serializer = serializer
			});
		}

		/// <summary>Serializes entity to stream.</summary>
		/// <param name="writer">Writer to serialize entity by.</param>
		/// <param name="entity">Entity to serialize.</param>
		public void Serialize(BinaryWriter writer, IEntity entity) {
			var handler = _serializers.FirstOrDefault(x => x.EntityType == entity.GetType());
			if (handler == null) throw new InvalidOperationException("No serializer registered for entity type: " + entity.GetType());

			// Serializes entity to stream
			writer.Write((byte)handler.EntityTypeId);
			writer.Write((short)entity.Id);
			handler.Serializer.Serialize(writer, entity);
		}

		/// <summary>Deserializes entity from stream.</summary>
		/// <param name="reader">Reader to deserialize entity by.</param>
		/// <param name="entity">Entity.</param>
		public void Deserialize(BinaryReader reader, ref IEntity entity) {
			// Looking for serializer
			var entityTypeId = reader.ReadByte();
			var handler = _serializers.FirstOrDefault(x => x.EntityTypeId == entityTypeId);
			if (handler == null)
				throw new InvalidOperationException("No serializer registered for entity type id: " + entityTypeId);

			// Deserializes entity from stream
			var entityId = reader.ReadUInt16();
			handler.Serializer.Deserialize(reader, ref entity);
			entity.Id = entityId;
		}

		/// <summary>Serializers.</summary>
		readonly List<EntitySerializerInfo> _serializers = new List<EntitySerializerInfo>();
	}
}
