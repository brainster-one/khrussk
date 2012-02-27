
namespace Khrussk.Tests.Realm {
	using System;
	using System.Linq;
	using System.Threading;
	using Khrussk.Realm;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass] public class RealmTests {
		private RealmTestContext _context = new RealmTestContext();

		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.Service.Start(_context.EndPoint);
			_context.Client.Connect(_context.EndPoint);

			Assert.IsTrue(_context.WaitFor(() => _context.ConnectedUsers.Count() == 1, 1000));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
		}
		
		/// <summary>UserConnected event should be triggered when connection established.</summary>
		[TestMethod] public void UserConnectedEventShouldBeTriggeredThenConnectionEstablishedTest() {
			Assert.AreEqual(1, _context.ConnectedUsers.Count());
		}
		
		/// <summary>EntityAdded event should be triggered when entity added.</summary>
		[TestMethod] public void EntityAddedEventShouldBeTriggeredWhenEntityAddedTest() {
			_context.Service.AddEntity(new Player { Name = "name" });
			
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, 1000));
			Assert.IsNotNull(_context.Entities.First());
			Assert.AreEqual("name", ((Player)_context.Entities.First()).Name);
		}

		/// <summary>EntityRemoved event should be triggered when entity removed.</summary>
		[TestMethod] public void EntityRemovedEventShouldBeTriggeredWhenEntityAddedTest() {
			_context.Service.AddEntity(new Player { Name = "name" });
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, 1000));

			_context.Service.RemoveEntity(_context.Entities.First());
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 0, 1000));
		}

		/// <summary>Entity should be synced properly.</summary>
		[TestMethod] public void EntityShouldBeSyncedProperlyTest() {
			_context.Service.AddEntity(new Player { Id = 99, Name = "player_name" });

			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, 1000));
			Assert.AreEqual(99, ((Player)_context.Entities.First()).Id);
			Assert.AreEqual("player_name", ((Player)_context.Entities.First()).Name);
		}
		
		/// <summary>Entity changed should be synced properly.</summary>
		[TestMethod] public void EntityChangesShouldBeSyncedProperlyTest() {
			// Add and modify entity
			var player = new Player { Id = 987, Name = "test" };
			_context.Service.AddEntity(player);
			player.Name = "changed";
			_context.Service.ModifyEntity(player);

			// Check changes
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, 1000));
			Assert.IsTrue(_context.WaitFor(() => ((Player)_context.Entities.First()).Name == "changed", 1000));
		}
	}
}
