

namespace Khrussk.Tests.UnitTests.Realm.Shared {
	using System.Collections.Generic;
	using NetworkRealm;

	/// <summary>Test context.</summary>
	public sealed class TestContext : BasicTestContext {
		/// <summary>Initializes new instance of TestContext.</summary>
		public TestContext() {
			ConnectedUsers = new List<User>();
			Entities = new Dictionary<int, object>();
			Service = new RealmService(new TestProtocol());
			Client = NewRealmClient();

			Service.UserConnected += OnUserConnected;
			Service.UserDisconnected += OnUserDisconnected;
		}

		/// <summary>Cleanup environment.</summary>
		public void Cleanup() {
			Service.Stop();
		}

		/// <summary>Creates new realm client.</summary>
		/// <returns>RealmClient.</returns>
		public RealmClient NewRealmClient() {
			var client = new RealmClient(new TestProtocol());
			client.EntityStateChanged += OnEntityStateChanged;
			client.ConnectionStateChanged += OnConnectionStateChanged;
			return client;
		}

		/// <summary>Client connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnConnectionStateChanged(object sender, RealmClientEventArgs e) {
			IsClientConnected = e.ConnectionState == ConnectionState.Connected;
		}

		/// <summary>User connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnUserConnected(object sender, RealmServiceEventArgs e) {
			ConnectedUsers.Add(e.User);
		}

		/// <summary>User disconnected from service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnUserDisconnected(object sender, RealmServiceEventArgs e) {
			ConnectedUsers.Remove(e.User);
		}

		/// <summary>On entity added event triggered</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityStateChanged(object sender, RealmClientEventArgs e) {
			if (e.EntityInfo.Action == EntityNetworkAction.Added)
				Entities.Add(e.EntityInfo.Id, e.EntityInfo.Entity);
			else if (e.EntityInfo.Action == EntityNetworkAction.Removed)
				Entities.Remove(e.EntityInfo.Id);
			/*else if (e.EntityInfo.Action == EntityNetworkAction.Modified)
				e.EntityInfo.Diff.ApplyChanges(Entities.First(x => x.Key == e.EntityInfo.Id).Value);*/
		}

		/// <summary>Gets service.</summary>
		public RealmService Service { get; private set; }

		/// <summary>Gets client.</summary>
		public RealmClient Client { get; private set; }

		/// <summary>Gets list of connected users.</summary>
		public List<User> ConnectedUsers { get; private set; }

		/// <summary>Gets list of entities.</summary>
		public Dictionary<int, object> Entities { get; set; }

		/// <summary>Gets client connection flag.</summary>
		public bool IsClientConnected { get; set; }
	}
}
