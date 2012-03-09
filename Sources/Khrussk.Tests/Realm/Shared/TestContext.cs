
namespace Khrussk.Tests.Realm {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Khrussk.NetworkRealm.Protocol;
	using Khrussk.NetworkRealm;

	/// <summary>Test context.</summary>
	public sealed class TestContext : BasicTestContext {
		/// <summary>Initializes new instance of TestContext.</summary>
		public TestContext() {
			ConnectedUsers = new List<User>();
			Entities = new List<IEntity>();
			Service = new RealmService();
			Client = NewRealmClient();

			Service.Protocol.RegisterEntityType(typeof(TestEntity), new TestEntitySerializer());
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
			var client = new RealmClient();
			client.EntityAdded += OnEntityAdded;
			client.EntityModified += OnEntityModified;
			client.EntityRemoved += OnEntityRemoved;
			client.Connected += OnClientConnected;
			client.Disconnected += OnClientDisconnected;
			client.Protocol.RegisterEntityType(typeof(TestEntity), new TestEntitySerializer());
			return client;
		}

		/// <summary>Client connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientConnected(object sender, RealmEventArgs e) {
			IsClientConnected = true;
		}

		/// <summary>Client disconnected form service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientDisconnected(object sender, RealmEventArgs e) {
			IsClientConnected = false;
		}

		/// <summary>User connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnUserConnected(object sender, RealmEventArgs e) {
			ConnectedUsers.Add(e.User);
		}

		/// <summary>User disconnected from service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnUserDisconnected(object sender, RealmEventArgs e) {
			ConnectedUsers.Remove(e.User);
		}

		/// <summary>On entity added event triggered</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityAdded(object sender, RealmEventArgs e) {
			Entities.Add(e.Entity);
		}

		/// <summary>On entity removed event triggered.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityRemoved(object sender, RealmEventArgs e) {
			Entities.RemoveAll(x => x.Id == e.EntityId);
		}

		/// <summary>On entity modified event triggered.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityModified(object sender, RealmEventArgs e) {
			e.EntityDiffData.ApplyChanges(Entities.First(x => x.Id == e.EntityId));
		}

		/// <summary>Gets service.</summary>
		public RealmService Service { get; private set; }

		/// <summary>Gets client.</summary>
		public RealmClient Client { get; private set; }

		/// <summary>Gets list of connected users.</summary>
		public List<User> ConnectedUsers { get; private set; }

		/// <summary>Gets list of entities.</summary>
		public List<IEntity> Entities { get; set; }

		/// <summary>Gets client connection flag.</summary>
		public bool IsClientConnected { get; set; }
	}
}
