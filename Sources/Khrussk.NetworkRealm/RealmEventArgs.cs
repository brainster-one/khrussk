
namespace Khrussk.NetworkRealm {
	using System;
	using Khrussk.Peers;
	using Khrussk.NetworkRealm.Protocol;

	/// <summary>Event args for realm events.</summary>
	public class RealmEventArgs : EventArgs {
/*		public RealmEventArgs(int entityId) {
			EntityId = entityId;
		}

		public RealmEventArgs(User user) {
			this.User = user;
		}

		public RealmEventArgs(Guid session) {
			this.Session = session;
		}

		public RealmEventArgs(IEntity iEntity) {
			this.Entity = iEntity;
		}

		public RealmEventArgs(int entityId, EntityDiffData entityDiffData) {
			this.EntityId = entityId;
			this.EntityDiffData = entityDiffData;
		}

		public RealmEventArgs() {
			// TODO: Complete member initialization
		}

		public RealmEventArgs(Peers.IPacket packet) {
			Packet = packet;
		}
*/
		/// <summary>Gets user.</summary>
		public User User { get; set; }

		/// <summary>Gets session.</summary>
		public Guid Session { get; set; }

		/// <summary>Gets entity.</summary>
		public object Entity { get; set; }

		/// <summary>Gets entity Id.</summary>
		public int EntityId { get; set; }

		/// <summary>Gets entity diff data.</summary>
		public EntityDiffData EntityDiffData { get; set; }

		public IPacket Packet { get; set; }
	}
}
