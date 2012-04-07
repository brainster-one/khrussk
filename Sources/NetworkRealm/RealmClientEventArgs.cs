
namespace Khrussk.NetworkRealm {
	using System;

	public enum EntityNetworkAction {
		Added,
		Removed,
		Modified
	}

	public class PacketEventArgs : EventArgs {
		internal PacketEventArgs(object packet) {
			Packet = packet;
		}

		internal PacketEventArgs(object packet, User user) {
			Packet = packet;
			User = user;
		}

		/// <summary>Gets or sets packet.</summary>
		public object Packet { get; private set; }

		public User User { get; private set; }
	}

	public class ConnectionEventArgs : EventArgs {
		internal ConnectionEventArgs(User user, ConnectionState connectionState) {
			User = user;
			ConnectionState = connectionState;
		}

		public ConnectionState ConnectionState { get; private set; }
		public User User { get; private set; }
	}

	public class EntityEventArgs : EventArgs {
		internal EntityEventArgs(object entity, EntityNetworkAction state) {
			Entity = entity;
			Action = state;
		}
		public object Entity { get; private set; }
		public EntityNetworkAction Action { get; private set; }
	}

	/*public class RealmClientEventArgs : EventArgs {
		public Guid Session { get; set; }
		public ConnectionState ConnectionState { get; set; }
		public object Packet { get; set; }
	}*/


}
