using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khrussk.Realm {
	public class RealmServiceEventArgs : EventArgs {
		public User user;
		public Guid session;
		public Protocol.IEntity iEntity;
		public Protocol.EntityDiffData EntityDiffData { get; private set; } 

		public RealmServiceEventArgs(User user) {
			// TODO: Complete member initialization
			this.user = user;
		}

		public RealmServiceEventArgs(Guid session) {
			// TODO: Complete member initialization
			this.session = session;
		}

		public RealmServiceEventArgs(Protocol.IEntity iEntity) {
			// TODO: Complete member initialization
			this.iEntity = iEntity;
		}

		public RealmServiceEventArgs(Protocol.EntityDiffData entityDiffData) {
			// TODO: Complete member initialization
			EntityDiffData = entityDiffData;
		}
	}
}
