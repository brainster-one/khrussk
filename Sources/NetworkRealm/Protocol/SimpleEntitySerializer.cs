
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	/// <summary>Simple entity serializer</summary>
	/// <typeparam name="T">Type of entity.</typeparam>
	public sealed class SimpleEntitySerializer<T> : IEntitySerializer<T> {
		/// <summary>Serializes entity to stream.</summary>
		/// <param name="writer">Writer to serialize entity by.</param>
		/// <param name="entity">Entity to serialize.</param>
		public void Serialize(BinaryWriter writer, T entity) {
			foreach (var prop in GetProperties(entity.GetType())) {
				SerializeProperty(writer, entity, prop);
			}
		}

		/// <summary>Deserializes entity from stream.</summary>
		/// <param name="reader">Reader to deserialize entity by.</param>
		/// <param name="entity">Entity.</param>
		public void Deserialize(BinaryReader reader, ref T entity) {
			//entity = entity ?? CreateEntity();
			if (entity == null) entity = CreateEntity();

			foreach (var prop in GetProperties(entity.GetType())) {
				DeserializeProperty(reader, entity, prop);
			}
		}

		/// <summary>Serializes property to stream.</summary>
		/// <param name="writer">Writer.</param>
		/// <param name="entity">Entity instance.</param>
		/// <param name="property">Property to serialize.</param>
		static void SerializeProperty(BinaryWriter writer, T entity, PropertyInfo property) {
			try {
				dynamic value = property.GetValue(entity, null);
				writer.Write(value);
			} catch (Exception ex) {
				throw new InvalidOperationException(string.Format("Unable to serialize property {0}", property.Name), ex);
			}
		}

		/// <summary>Serializes property to stream.</summary>
		/// <param name="reader">Writer.</param>
		/// <param name="entity">Entity instance.</param>
		/// <param name="property">Property to serialize.</param>
		static void DeserializeProperty(BinaryReader reader, T entity, PropertyInfo property) {
			try {
				object value = null;
				if (property.PropertyType == typeof(Int64)) value = reader.ReadInt64();
				if (property.PropertyType == typeof(Int32)) value = reader.ReadInt32();
				if (property.PropertyType == typeof(Int16)) value = reader.ReadInt16();
				if (property.PropertyType == typeof(Double)) value = reader.ReadDouble();
				if (property.PropertyType == typeof(Single)) value = reader.ReadSingle();
				if (property.PropertyType == typeof(String)) value = reader.ReadString();
				if (value == null) throw new InvalidOperationException(String.Format("{0} type is not supported", property.PropertyType.Name));

				property.SetValue(entity, value, null);
			} catch (Exception ex) {
				throw new InvalidOperationException(string.Format("Unable to deserialize property {0}", property.Name), ex);
			}
		}

		/// <summary>Gets list of properties to serialize/deserialize.</summary>
		/// <param name="type">Entity type.</param>
		/// <returns>List of PropertyInfo.</returns>
		static IEnumerable<PropertyInfo> GetProperties(Type type) {
			return type.GetProperties()
				.Where(x => x.CanRead)
				.Where(x => x.PropertyType.IsPrimitive);
		}

		/// <summary>Creates entity of specified type.</summary>
		/// <returns>Entity.</returns>
		static T CreateEntity() {
			var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
			if (constructor == null) throw new InvalidOperationException(String.Format("{0} does not contain parameterless constructor", typeof(T).Name));
			return (T)constructor.Invoke(null);
		}
	}
}
