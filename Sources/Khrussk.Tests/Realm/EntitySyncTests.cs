
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass] public class EntitySyncTests : ConnectionTests {
		
		/// <summary>EntityAdded event should be triggered when entity added.</summary>
		[TestMethod] public void EntityAddedEventShouldBeTriggeredWhenEntityAddedTest() {
			Context.Service.AddEntity(new TestEntity { Name = "name" });
			Assert.IsTrue(Context.WaitFor(() => Context.Entities.Count() == 1, Context.WaitingPeriod));
		}

		/// <summary>EntityRemoved event should be triggered when entity removed.</summary>
		[TestMethod] public void EntityRemovedEventShouldBeTriggeredWhenEntityAddedTest() {
			var serviceEntity = new TestEntity { Name = "name" };
			Context.Service.AddEntity(serviceEntity);
			Assert.IsTrue(Context.WaitFor(() => Context.Entities.Count() == 1, Context.WaitingPeriod));
			Context.Service.RemoveEntity(serviceEntity);
			Assert.IsTrue(Context.WaitFor(() => Context.Entities.Count() == 0, Context.WaitingPeriod));
		}

		/// <summary>Entity should be synced properly.</summary>
		[TestMethod] public void EntityShouldBeSyncedTest() {
			Context.Service.AddEntity(new TestEntity { Name = "player_name" });

			Assert.IsTrue(Context.WaitFor(() => Context.Entities.Count() == 1, Context.WaitingPeriod));
			Assert.AreEqual("player_name", ((TestEntity)Context.Entities.First().Value).Name);
		}
		
		/// <summary>Entity changed should be synced properly.</summary>
		[TestMethod] public void EntityChangesShouldBeSyncedProperlyTest() {
			// Add and modify entity
			var player = new TestEntity { Name = "test" };
			Context.Service.AddEntity(player);
			player.Name = "changed";
			Context.Service.ModifyEntity(player);

			// Check changes
			Assert.IsTrue(Context.WaitFor(() => Context.Entities.Count() == 1, Context.WaitingPeriod));
			Assert.IsTrue(Context.WaitFor(() => ((TestEntity)Context.Entities.First().Value).Name == "changed", Context.WaitingPeriod));
		}

		/// <summary>EntityAdded event should be triggered when entity added.</summary>
		[TestMethod] public void RealmServiceCanHandleMultipleEntities() {
			foreach (var name in new[] { "p1", "p2", "p3", "p4", "p5", "p6" }) {
				Context.Service.AddEntity(new TestEntity { Name = name });
			}
			Assert.IsTrue(Context.WaitFor(() => Context.Entities.Count() == 6, Context.WaitingPeriod * 10));
		}
	}
}
