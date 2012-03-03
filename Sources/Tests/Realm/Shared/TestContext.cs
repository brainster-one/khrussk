
namespace Khrussk.Tests.Realm {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Khrussk.Realm;
	using Khrussk.Realm.Protocol;

	/// <summary>Test context.</summary>
	public sealed class TestContext : BasicTestContext {
		/// <summary>Initializes new instance of TestContext.</summary>
		public TestContext() {
			ConnectedUsers = new List<User>();
			Entities = new List<IEntity>();
			Service = new RealmService();
			Client = NewRealmClient();

			Service.RegisterEntityType(typeof(TestEntity), new TestEntitySerializer());
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
			client.Connected += new EventHandler<Khrussk.Realm.RealmServiceEventArgs>(OnClientConnected);
			client.Disconnected += new EventHandler<Khrussk.Realm.RealmServiceEventArgs>(OnClientDisconnected);
			client.RegisterEntityType(typeof(TestEntity), new TestEntitySerializer());
			return client;
		}

		/// <summary>Client connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientConnected(object sender, RealmServiceEventArgs e) {
			IsClientConnected = true;
		}

		/// <summary>Client disconnected form service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientDisconnected(object sender, RealmServiceEventArgs e) {
			IsClientConnected = false;
		}

		/// <summary>User connected to service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnUserConnected(object sender, RealmServiceEventArgs e) {
			ConnectedUsers.Add(e.user);
		}

		/// <summary>User disconnected from service.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnUserDisconnected(object sender, RealmServiceEventArgs e) {
			ConnectedUsers.Remove(e.user);
		}

		/// <summary>On entity added event triggered</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityAdded(object sender, RealmServiceEventArgs e) {
			Entities.Add(e.iEntity);
		}

		/// <summary>On entity removed event triggered.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityRemoved(object sender, RealmServiceEventArgs e) {
			Entities.RemoveAll(x => x.Id == e.EntityId);
		}

		/// <summary>On entity modified event triggered.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnEntityModified(object sender, RealmServiceEventArgs e) {
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
