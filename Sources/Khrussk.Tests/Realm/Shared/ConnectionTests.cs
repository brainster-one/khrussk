
namespace Khrussk.Tests.Realm {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass] public abstract class ConnectionTests {
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			Context = new TestContext();
			Context.Service.Start(Context.EndPoint);
			Context.Client.Connect(Context.EndPoint);
			Assert.IsTrue(Context.WaitFor(() => Context.ConnectedUsers.Count() == 1, Context.WaitingPeriod));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			Context.Cleanup();
		}

		/// <summary>Gets context.</summary>
		protected TestContext Context { get; private set; }
	}
}
