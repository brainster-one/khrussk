
namespace Khrussk.Tests.Realm {
	using System;
	using System.Net;
	using System.Threading;
	using Khrussk.Realm;
	using System.Collections.Generic;
	using Khrussk.Realm.Protocol;
	using System.Linq;

	sealed class RealmTestContext : TestContext {
		/// <summary></summary>
		public RealmTestContext() {
			ConnectedUsers = new List<User>();
			Entities = new List<IEntity>();

			Service = new RealmService();
			Client = new RealmClient();

			Service.RegisterEntityType(typeof(Player), new PlayerSerializer());
			Client.RegisterEntityType(typeof(Player), new PlayerSerializer());

			Service.UserConnected += Service_UserConnected;
			Client.EntityAdded += EntityAdded;
			Client.EntityModified += EntityModified;
			Client.EntityRemoved += EntityRemoved;
			/*Service.PacketReceived += Service_UserConnected;
			
			Client.Connected += Service_UserConnected;*/

			EndPoint = new IPEndPoint(IPAddress.Loopback, ++_port);
			Wait = new ManualResetEvent(false);
		}

		void Service_UserConnected(object sender, RealmServiceEventArgs e) {
			ConnectedUsers.Add(e.user);
		}

		void EntityAdded(object sender, RealmServiceEventArgs e) {
			Entities.Add(e.iEntity);
		}

		void EntityRemoved(object sender, RealmServiceEventArgs e) {
			Entities.RemoveAll(x => x.Id == e.EntityId);
		}

		void EntityModified(object sender, RealmServiceEventArgs e) {
			e.EntityDiffData.ApplyChanges(Entities.First(x => x.Id == e.EntityId));
		}

		public void Cleanup() {
		}

		public IPEndPoint EndPoint { get; set; }
		public RealmService Service { get; set; }
		public RealmClient Client { get; set; }
		public RealmClient Accepted { get; set; }		
		private ManualResetEvent Wait { get; set; }
		public RealmServiceEventArgs RealmServiceEventArgs { get; set; }
		static int _port = 1025;
		static object _lock = new object();

		public List<User> ConnectedUsers { get; set; }
		public List<IEntity> Entities { get; set; }
	}
}
