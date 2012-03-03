
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>Connection establishing between RealmService and client tests.</summary>
	[TestClass] public sealed class RealmServiceConnectionTests : ConnectionTests {

		/// <summary>Disconnected event should be triggered on client side when connection closed on local side.</summary>
		[TestMethod] public void DisconnectedEventShouldBeTriggeredThenConnectionClosedTest2() {
			_context.Client.Disconnect();
			Assert.IsTrue(_context.WaitFor(() => !_context.IsClientConnected, _context.WaitingPeriod));
		}

		/// <summary>UserConnected event should be triggered when connection established.</summary>
		[TestMethod] public void UserConnectedEventShouldBeTriggeredThenConnectionEstablishedTest() {
			Assert.AreEqual(1, _context.ConnectedUsers.Count());
		}

		/// <summary>UserDisconnected event should be triggered when connection closed.</summary>
		[TestMethod] public void UserDisconnectedEventShouldBeTriggeredThenConnectionClosedTest() {
			_context.Client.Disconnect();
			Assert.IsTrue(_context.WaitFor(() => _context.ConnectedUsers.Count() == 0, _context.WaitingPeriod));
		}

		/// <summary>Realm server can handle  multiple connections.</summary>
		[TestMethod] public void RealmServerCanHandleMultipleConnectionsTest() {
			_context.NewRealmClient().Connect(_context.EndPoint);
			_context.NewRealmClient().Connect(_context.EndPoint);
			Assert.IsTrue(_context.WaitFor(() => _context.ConnectedUsers.Count() == 2 + 1, _context.WaitingPeriod));
		}

		/// <summary>Realm server can handle  multiple disconnections.</summary>
		[TestMethod] public void RealmServerCanHandleMultipleDisconnectionTest() {
			var client = _context.NewRealmClient();
			client.Connect(_context.EndPoint);
			Assert.IsTrue(_context.WaitFor(() => _context.ConnectedUsers.Count() == 1 + 1, _context.WaitingPeriod));

			client.Disconnect();
			Assert.IsTrue(_context.WaitFor(() => _context.ConnectedUsers.Count() == 1, _context.WaitingPeriod));
		}

		/// <summary>Related user should be passed via event then connection closed.</summary>
		[TestMethod] public void RelatedUserShouldBePassedViaEventThenConnectionClosedTest() {
			var user = _context.ConnectedUsers.First();
			
			// Second client
			var client = _context.NewRealmClient();
			client.Connect(_context.EndPoint);
			Assert.IsTrue(_context.WaitFor(() => _context.ConnectedUsers.Count() == 1 + 1, _context.WaitingPeriod));
			var secondUser = _context.ConnectedUsers.Last();

			// Second client disconnected
			client.Disconnect();
			Assert.IsTrue(_context.WaitFor(() => !_context.ConnectedUsers.Contains(secondUser), _context.WaitingPeriod));
		}
	}
}
