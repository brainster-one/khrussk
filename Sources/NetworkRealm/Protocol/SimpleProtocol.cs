
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public sealed class SimpleProtocol : RealmProtocol {
		public SimpleProtocol(IEnumerable<Type> entityTypes) {
			foreach (var type in entityTypes) {
				var method = typeof(SimpleProtocol)
					.GetMethod("RegisterEntityType", BindingFlags.NonPublic | BindingFlags.Instance)
					.MakeGenericMethod(type)
					.Invoke(this, null);
			}
		}

		private void RegisterEntityType<T>() {
			var ser = new SimpleEntitySerializer<T>();
			Register(ser);
		}
	}


}
