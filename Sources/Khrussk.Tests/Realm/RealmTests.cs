
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
		
		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod] public void ConnectionShouldBeEstablishedTest() {
			Assert.AreEqual(1, _context.ConnectedUsers.Count());
		}
		
		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod] public void AddEntityTest() {
			_context.Service.AddEntity(new Player { Name = "name" });
			
			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, 1000));
			Assert.IsNotNull(_context.Entities.First());
			Assert.AreEqual("name", ((Player)_context.Entities.First()).Name);
		}
		
		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod] public void SyncEntityTest() {
			var player = new Player { Name = "test" };
			_context.Service.AddEntity(player);
			player.Name = "changed";
			_context.Service.ModifyEntity(player);

			Assert.IsTrue(_context.WaitFor(() => _context.Entities.Count() == 1, 10000));
			Assert.IsTrue(_context.WaitFor(() => ((Player)_context.Entities.First()).Name == "changed", 10000));
		}
	}
}
