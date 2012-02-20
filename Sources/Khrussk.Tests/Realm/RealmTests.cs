
namespace Khrussk.Tests.Realm {
	using System;
	using Khrussk.Tests;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Khrussk.Realm;
	using Khrussk.Peers;
	using Khrussk.Realm.Protocol;
	using System.Threading;
	using System.Net;

	/// <summary>
	///This is a test class for ListenerSocketTests and is intended
	///to contain all ListenerSocketTests Unit Tests
	///</summary>
	[TestClass()] public class RealmTests {
		public ManualResetEvent evnt = new ManualResetEvent(false);
		private Context _context = new Context();

		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.Service.RegisterEntityType(typeof(Player), new PlayerSerializer());
			_context.Client.RegisterEntityType(typeof(Player), new PlayerSerializer());

			_context.Service.Start(_context.EndPoint);
			_context.WaitFor(() => _context.Client.Connect(_context.EndPoint));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
		}

		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod()] public void ConnectionShouldBeEstablishedTest() {
			RealmService service = new RealmService();
			service.Start(_context.EndPoint);

			RealmClient client = new RealmClient();
			client.Connected += new EventHandler<RealmServiceEventArgs>(client_Connected);
			client.Connect(_context.EndPoint);

			Assert.IsTrue(evnt.WaitOne(TimeSpan.FromSeconds(3)));
		}

		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod()] public void AddEntityTest() {
			_context.WaitFor(() => _context.Service.AddEntity(new Player { Name = "Olololo" }));
			Assert.IsNotNull(_context.RealmServiceEventArgs.iEntity);
			Assert.AreEqual("Olololo", ((Player)_context.RealmServiceEventArgs.iEntity).Name);
		}

		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod()] public void SyncEntityTest() {
			var player = new Player { Name = "test" };
			_context.WaitFor(() => _context.Service.AddEntity(player));

			player.Name = "test_after";
			_context.WaitFor(() => _context.Service.ModifyEntity(player));
			
			Assert.IsNotNull(_context.RealmServiceEventArgs.EntityDiffData);
			_context.RealmServiceEventArgs.EntityDiffData.ApplyChanges(player);
			
			Assert.AreEqual("test_after", player.Name);
		}

		void client_Connected(object sender, RealmServiceEventArgs e) {
			evnt.Set();
		}
	}
}
