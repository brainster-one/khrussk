using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khrussk.Realm {
	public class RealmServiceEventArgs : EventArgs {
		private User user;
		private Guid session;

		public RealmServiceEventArgs(User user) {
			// TODO: Complete member initialization
			this.user = user;
		}

		public RealmServiceEventArgs(Guid session) {
			// TODO: Complete member initialization
			this.session = session;
		}
	}
}
