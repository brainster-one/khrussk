

namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	public sealed class SimpleEntitySerializer<T> : IEntitySerializer<T> {
		public void Serialize(BinaryWriter writer, T entity) {
			foreach (var prop in GetProperties(entity)) {
				dynamic value = prop.GetValue(entity, null);
				writer.Write(value);
			}
		}

		public void Deserialize(BinaryReader reader, ref T entity) {
			//entity = entity ?? CreateEntity();
			if (entity == null) entity = CreateEntity();
			foreach (var prop in GetProperties(entity)) {
				dynamic value = null;
				if (prop.PropertyType == typeof(Int64)) value = reader.ReadInt64();
				if (prop.PropertyType == typeof(Int32)) value = reader.ReadInt32();
				if (prop.PropertyType == typeof(Int16)) value = reader.ReadInt16();
				if (prop.PropertyType == typeof(Double)) value = reader.ReadDouble();
				if (prop.PropertyType == typeof(Single)) value = reader.ReadSingle();
				if (prop.PropertyType == typeof(String)) value = reader.ReadString();
				
				prop.SetValue(entity, value,  null);
			}

		}

		private static IEnumerable<PropertyInfo> GetProperties(T entity) {
			return entity.GetType().GetProperties()
				.Where(x => x.CanRead)
				.Where(x => x.PropertyType.IsPrimitive);
		}

		private static T CreateEntity() {
			return (T)typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null);
			// return default(T);
		}
	}
}
