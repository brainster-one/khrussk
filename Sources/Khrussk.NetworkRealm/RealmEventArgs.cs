
namespace Khrussk.NetworkRealm {
	using System;
	using Khrussk.Peers;
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

	public class RealmServiceEventArgs : EventArgs {
		/// <summary>Gets or sets user.</summary>
		public User User { get; set; }

		/// <summary>Gets or sets packet.</summary>
		public object Packet { get; set; }
	}
}
