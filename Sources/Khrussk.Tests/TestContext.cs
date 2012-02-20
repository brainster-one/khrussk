
namespace Khrussk.Tests {
	using System.Net;
	using System.Threading;

	/// <summary>Test context.</summary>
	class TestContext {
		/// <summary>Initializes a new instance of the TestContext class.</summary>
		public TestContext() {
			Wait = new ManualResetEvent(false);
			EndPoint = new IPEndPoint(IPAddress.Loopback, ++_port);
		}

		/// <summary>Gets wait event.</summary>
		public ManualResetEvent Wait { get; private set; }

		/// <summary>Gets EndPoint.</summary>
		public IPEndPoint EndPoint { get; private set; }

		/// <summary>Port.</summary>
		static int _port = 1025;
	}
}
