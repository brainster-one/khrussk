
namespace Khrussk.NetworkRealm {
	using System;
	using Khrussk.NetworkRealm.Protocol;

	public class EntityInfo {
		public int Id { get; set; }
		public object Entity { get; set; }
		public EntityDiffData Diff { get; set; }
	}

	public class RealmClientEventArgs : EventArgs {
		public Guid Session { get; set; }
		public EntityInfo EntityInfo { get; set; }
	}
}
