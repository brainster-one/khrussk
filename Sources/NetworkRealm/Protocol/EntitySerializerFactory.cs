
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.Collections.Generic;
	using Khrussk.NetworkRealm.Protocol;

	sealed class EntitySerializerFactory {
		public void Register<T>(IEntitySerializer<T> serializer) {
			_serializers.Add(typeof(T), (object)serializer);
		}

		public IEntitySerializer<T> Get<T>() {
			return _serializers[typeof(T)] as IEntitySerializer<T>;
		}

		Dictionary<Type, object> _serializers = new Dictionary<Type, object>();
	}
}
