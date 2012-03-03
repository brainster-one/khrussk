
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>Connection establishing between RealmService and client tests.</summary>
	[TestClass] public class ConnectionTests {
		protected TestContext _context = new TestContext();

		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.Service.Start(_context.EndPoint);
			_context.Client.Connect(_context.EndPoint);
			Assert.IsTrue(_context.WaitFor(() => _context.ConnectedUsers.Count() == 1, 1000));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}
	}
}
