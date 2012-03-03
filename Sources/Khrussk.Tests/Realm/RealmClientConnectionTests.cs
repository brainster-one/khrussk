
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>Connection establishing between RealmService and client tests.</summary>
	[TestClass] public sealed class RealmClientConnectionTests : ConnectionTests {

		/// <summary>Connected event should be triggered on client side when connection established.</summary>
		[TestMethod] public void ConnectedEventShouldBeTriggeredThenConnectionEstablishedTest() {
			Assert.IsTrue(_context.IsClientConnected);
		}

		/// <summary>Disconnected event should be triggered on client side when connection closed on remote side.</summary>
		[TestMethod] public void DisconnectedEventShouldBeTriggeredThenConnectionClosedTest() {
			_context.Service.Disconnect(_context.ConnectedUsers.First());
			Assert.IsTrue(_context.WaitFor(() => !_context.IsClientConnected, _context.WaitingPeriod));
		}

	}
}
