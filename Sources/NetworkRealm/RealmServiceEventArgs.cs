
namespace Khrussk.NetworkRealm {
	using System;

	/// <summary>Realm service event args.</summary>
	public class RealmServiceEventArgs : EventArgs {
		/// <summary>Gets or sets user.</summary>
		public User User { get; set; }

		/// <summary>Gets or sets packet.</summary>
		public object Packet { get; set; }
	}
}
