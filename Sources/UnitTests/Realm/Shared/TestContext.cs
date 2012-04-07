
namespace Khrussk.Tests.UnitTests.Realm.Shared {
	using System.Collections.Generic;
	using NetworkRealm;

	/// <summary>Test context.</summary>
	public sealed class TestContext : BasicTestContext {
		/// <summary>Initializes new instance of TestContext.</summary>
		public TestContext() {
			ConnectedUsers = new List<User>();
			Entities = new List<object>();
			Service = new RealmService(new TestProtocol());
			Client = NewRealmClient();

			Service.UserConnectionStateChanged += OnUserConnectionStateChanged;
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
		void OnConnectionStateChanged(object sender, ConnectionEventArgs e) {
			IsClientConnected = e.State == ConnectionState.Connected;
		}

		/// <summary>User connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnUserConnectionStateChanged(object sender, ConnectionEventArgs e) {
			if (e.State == ConnectionState.Connected)
				ConnectedUsers.Add(e.User);
			else if (e.State == ConnectionState.Disconnected)
				ConnectedUsers.Remove(e.User);
		}

		/// <summary>On entity added event triggered</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityStateChanged(object sender, EntityEventArgs e) {
			if (e.State == EntityState.Added)
				Entities.Add(e.Entity);
			else if (e.State == EntityState.Removed)
				Entities.Remove(e.Entity);
		}

		/// <summary>Gets service.</summary>
		public RealmService Service { get; private set; }

		/// <summary>Gets client.</summary>
		public RealmClient Client { get; private set; }

		/// <summary>Gets list of connected users.</summary>
		public List<User> ConnectedUsers { get; private set; }

		/// <summary>Gets list of entities.</summary>
		public List<object> Entities { get; set; }

		/// <summary>Gets client connection flag.</summary>
		public bool IsClientConnected { get; set; }
	}
}
