
namespace Khrussk.Tests.UnitTests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Shared;

	/// <summary>Connection establishing between RealmService and client tests.</summary>
	[TestClass] public sealed class RealmClientConnectionTests : ConnectionTests {

		/// <summary>Connected event should be triggered on client side when connection established.</summary>
		[TestMethod] public void ConnectedEventShouldBeTriggeredThenConnectionEstablishedTest() {
			Assert.IsTrue(Context.IsClientConnected);
		}

		/// <summary>Disconnected event should be triggered on client side when connection closed on remote side.</summary>
		[TestMethod] public void DisconnectedEventShouldBeTriggeredThenConnectionClosedTest() {
			Context.Service.Disconnect(Context.ConnectedUsers.First());
			Assert.IsTrue(Context.WaitFor(() => !Context.IsClientConnected, Context.WaitingPeriod));
		}

	}
}
