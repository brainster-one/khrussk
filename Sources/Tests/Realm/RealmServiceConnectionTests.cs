
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>Connection establishing between RealmService and client tests.</summary>
	[TestClass] public sealed class RealmServiceConnectionTests : ConnectionTests {

		/// <summary>Disconnected event should be triggered on client side when connection closed on local side.</summary>
		[TestMethod] public void DisconnectedEventShouldBeTriggeredThenConnectionClosedTest2() {
			Context.Client.Disconnect();
			Assert.IsTrue(Context.WaitFor(() => !Context.IsClientConnected, Context.WaitingPeriod));
		}

		/// <summary>UserConnected event should be triggered when connection established.</summary>
		[TestMethod] public void UserConnectedEventShouldBeTriggeredThenConnectionEstablishedTest() {
			Assert.AreEqual(1, Context.ConnectedUsers.Count());
		}

		/// <summary>UserDisconnected event should be triggered when connection closed.</summary>
		[TestMethod] public void UserDisconnectedEventShouldBeTriggeredThenConnectionClosedTest() {
			Context.Client.Disconnect();
			Assert.IsTrue(Context.WaitFor(() => !Context.ConnectedUsers.Any(), Context.WaitingPeriod));
		}

		/// <summary>Realm server can handle  multiple connections.</summary>
		[TestMethod] public void RealmServerCanHandleMultipleConnectionsTest() {
			Context.NewRealmClient().Connect(Context.EndPoint);
			Context.NewRealmClient().Connect(Context.EndPoint);
			Assert.IsTrue(Context.WaitFor(() => Context.ConnectedUsers.Count() == 2 + 1, Context.WaitingPeriod));
		}

		/// <summary>Realm server can handle  multiple disconnections.</summary>
		[TestMethod] public void RealmServerCanHandleMultipleDisconnectionTest() {
			var client = Context.NewRealmClient();
			client.Connect(Context.EndPoint);
			Assert.IsTrue(Context.WaitFor(() => Context.ConnectedUsers.Count() == 1 + 1, Context.WaitingPeriod));

			client.Disconnect();
			Assert.IsTrue(Context.WaitFor(() => Context.ConnectedUsers.Count() == 1, Context.WaitingPeriod));
		}

		/// <summary>Related user should be passed via event then connection closed.</summary>
		[TestMethod] public void RelatedUserShouldBePassedViaEventThenConnectionClosedTest() {
			// Second client
			var client = Context.NewRealmClient();
			client.Connect(Context.EndPoint);
			Assert.IsTrue(Context.WaitFor(() => Context.ConnectedUsers.Count() == 1 + 1, Context.WaitingPeriod));
			var secondUser = Context.ConnectedUsers.Last();

			// Second client disconnected
			client.Disconnect();
			Assert.IsTrue(Context.WaitFor(() => !Context.ConnectedUsers.Contains(secondUser), Context.WaitingPeriod));
		}
	}
}
