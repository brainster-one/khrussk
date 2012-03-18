
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using System.Collections.Generic;
	using Khrussk.NetworkRealm;
	using Khrussk.Tests.Realm.Shared;

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
			client.EntityAdded += OnEntityAdded;
			client.EntityModified += OnEntityModified;
			client.EntityRemoved += OnEntityRemoved;
			client.Connected += OnClientConnected;
			client.Disconnected += OnClientDisconnected;
			return client;
		}

		/// <summary>Client connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientConnected(object sender, RealmClientEventArgs e) {
			IsClientConnected = true;
		}

		/// <summary>Client disconnected form service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientDisconnected(object sender, RealmClientEventArgs e) {
			IsClientConnected = false;
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
		void OnEntityAdded(object sender, RealmClientEventArgs e) {
			Entities.Add(e.EntityInfo.Id, e.EntityInfo.Entity);
		}

		/// <summary>On entity removed event triggered.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityRemoved(object sender, RealmClientEventArgs e) {
			Entities.Remove(e.EntityInfo.Id);
		}

		/// <summary>On entity modified event triggered.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityModified(object sender, RealmClientEventArgs e) {
			e.EntityInfo.Diff.ApplyChanges(Entities.First(x => x.Key == e.EntityInfo.Id).Value);
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
