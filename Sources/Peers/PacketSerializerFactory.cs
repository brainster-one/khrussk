﻿
namespace Khrussk.Peers {
	using System;
	using System.Collections.Generic;

	sealed class PacketSerializerFactory {
		public void Register<T>(IPacketSerializer<T> serializer) {
			_serializers.Add(typeof(T), (object)serializer);
		}

		public IPacketSerializer<T> Get<T>() {
			return _serializers[typeof(T)] as IPacketSerializer<T>;
		}

		Dictionary<Type, object> _serializers = new Dictionary<Type, object>();
	}
}
