
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;

namespace Khrussk.NetworkRealm.Protocol {
	public sealed class SimpleRealmProtocol : RealmProtocol {
		public SimpleRealmProtocol(IEnumerable<Type> entityTypes) {
			foreach (var type in entityTypes) {
				var method = typeof(SimpleRealmProtocol)
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
