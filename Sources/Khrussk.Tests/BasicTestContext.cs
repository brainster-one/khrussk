
namespace Khrussk.Tests {
	using System.Net;
	using System.Threading;

	/// <summary>Waiting for event thread handler.</summary>
	/// <returns>False - timeout.</returns>
	public delegate bool WaitForThreadHandler();

	/// <summary>Test context.</summary>
	public class BasicTestContext {
		/// <summary>Initializes a new instance of the TestContext class.</summary>
		public BasicTestContext() {
			Wait = new ManualResetEvent(false);
			EndPoint = new IPEndPoint(IPAddress.Loopback, ++_port);
		}

		/// <summary>Waits for event.</summary>
		/// <param name="handler">Event wait function.</param>
		/// <param name="timeout">Timeout.</param>
		/// <returns>true - Event happens/ otherwice - false.</returns>
		public bool WaitFor(WaitForThreadHandler handler, int timeout) {
			var evt = new ManualResetEvent(false);

			new System.Threading.Thread(x => {
				while (handler() == false) { }
				evt.Set();
			}).Start();

			return evt.WaitOne(timeout);
		}


		/// <summary>Gets wait event.</summary>
		public ManualResetEvent Wait { get; private set; }

		/// <summary>Gets EndPoint.</summary>
		public IPEndPoint EndPoint { get; private set; }

		/// <summary>Gets waiting period in millisecnds.</summary>
		public int WaitingPeriod { get { return 500; } }

		/// <summary>Port.</summary>
		static int _port = 1025;
	}
}
