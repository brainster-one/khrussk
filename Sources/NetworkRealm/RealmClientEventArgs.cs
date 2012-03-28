
namespace Khrussk.NetworkRealm {
	using System;
	using Protocol;

	public enum EntityNetworkAction {
		Added,
		Removed,
		Modified
	}

	public class EntityInfo {
		public int Id { get; set; }
		public object Entity { get; set; }
		public EntityDiffData Diff { get; set; }
		public EntityNetworkAction Action { get; set; }
	}

	public class RealmClientEventArgs : EventArgs {
		public Guid Session { get; set; }
		public EntityInfo EntityInfo { get; set; }
		public ConnectionState ConnectionState { get; set; }
		public object Packet { get; set; }
	}
}
