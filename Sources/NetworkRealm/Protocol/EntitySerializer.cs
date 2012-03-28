
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;


	/// <summary>Basic entity serializer.</summary>
	public sealed class EntitySerializer {
		/// <summary>Registers entity serializer.</summary>
		/// <param name="serializer">Entity serializer.</param>
		public void Register<T>(IEntitySerializer<T> serializer) {
			_factory.Register(serializer);
			_idToType[++_currId] = typeof(T);
		}

		/// <summary>Serializes entity to stream.</summary>
		/// <param name="writer">Writer to serialize entity by.</param>
		/// <param name="entity">Entity to serialize.</param>
		public void Serialize(BinaryWriter writer, object entity) {
			writer.Write((byte)_idToType.First(x => x.Value == entity.GetType()).Key);
			var method = typeof(EntitySerializer)
				.GetMethod("write", BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod(entity.GetType())
				.Invoke(this, new[] { writer, entity });
		}

		/// <summary>Deserializes entity from stream.</summary>
		/// <param name="reader">Reader to deserialize entity by.</param>
		/// <param name="entity">Entity.</param>
		public void Deserialize(BinaryReader reader, ref object entity) {
			// Looking for serializer
			var entityTypeId = reader.ReadByte();
			entity = typeof(EntitySerializer)
				.GetMethod("read", BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod(_idToType[entityTypeId])
				.Invoke(this, new[] { reader, entity });
		}

		private void Write<T>(BinaryWriter writer, object entity) {
			var serializer = _factory.Get<T>();
			if (serializer == null) throw new InvalidOperationException("No serializer registered for " + entity.GetType());
			serializer.Serialize(writer, (T)entity);
		}

		private T Read<T>(BinaryReader reader, object entity) {
			var serializer = _factory.Get<T>();
			var res = (T)entity;
			if (serializer == null) throw new InvalidOperationException("No serializer registered for ");
			serializer.Deserialize(reader, ref res);
			return res;
		}

		/// <summary>Serializers.</summary>
		readonly Dictionary<int, Type> _idToType = new Dictionary<int, Type>();
		readonly EntitySerializerFactory _factory = new EntitySerializerFactory();
		int _currId;
	}
}
