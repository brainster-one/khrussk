
namespace Khrussk.Tests {
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Threading;

	/// <summary>Waiting for event thread handler.</summary>
	/// <returns>False - timeout.</returns>
	public delegate bool WaitForThreadHandler();

	/// <summary>Test context.</summary>
	public class BasicTestContext {
		/// <summary>Initializes a new instance of the BasicTestContext class.</summary>
		public BasicTestContext() {
			EndPoint = new IPEndPoint(IPAddress.Loopback, ++_port);
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
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

		/// <summary>Gets EndPoint.</summary>
		public IPEndPoint EndPoint { get; private set; }

		/// <summary>Gets waiting period in millisecnds.</summary>
		public int WaitingPeriod { get { return 750; } }

		/// <summary>On unhandled exception.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Evet args.</param>
		void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Debug.Print("Unhandled exception: " + e.ExceptionObject);
		}

		/// <summary>Port.</summary>
		static int _port = 1025;
	}
}
