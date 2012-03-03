
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass] public class EntitySyncTests : ConnectionTests {
		
		/// <summary>EntityAdded event should be triggered when entity added.</summary>
		[TestMethod] public void EntityAddedEventShouldBeTriggeredWhenEntityAddedTest() {
			_context.Service.AddEntity(new Player { Name = "name" });
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, _context.WaitingPeriod));
		}

		/// <summary>EntityRemoved event should be triggered when entity removed.</summary>
		[TestMethod] public void EntityRemovedEventShouldBeTriggeredWhenEntityAddedTest() {
			_context.Service.AddEntity(new Player { Name = "name" });
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, _context.WaitingPeriod));
			_context.Service.RemoveEntity(_context.Entities.First());
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 0, _context.WaitingPeriod));
		}

		/// <summary>Entity should be synced properly.</summary>
		[TestMethod] public void EntityShouldBeSyncedTest() {
			_context.Service.AddEntity(new Player { Id = 99, Name = "player_name" });

			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, _context.WaitingPeriod));
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
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, _context.WaitingPeriod));
			Assert.IsTrue(_context.WaitFor(() => ((Player)_context.Entities.First()).Name == "changed", _context.WaitingPeriod));
		}
	}
}
