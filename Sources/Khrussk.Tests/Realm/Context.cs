
namespace Khrussk.Tests.Realm {
	using System.Net;
	using System.Threading;
	using Khrussk.Sockets;
	using System;
	using Khrussk.Realm;

	/// <summary>Thread handler.</summary>
	/// <param name="exception">Exception.</param>
	public delegate void ThreadHandler();

	sealed class Context {
		/// <summary></summary>
		public Context() {
			Service = new RealmService();
			Client = new RealmClient();

			//Service.UserConnected += new EventHandler<RealmServiceEventArgs>(Service_UserConnected);
			Service.PacketReceived += Service_UserConnected;
			Client.EntityAdded += Service_UserConnected;
			Client.Connected += Service_UserConnected;

			EndPoint = new IPEndPoint(IPAddress.Loopback, ++_port);
			Wait = new ManualResetEvent(false);
		}

		void Service_UserConnected(object sender, RealmServiceEventArgs e) {
			/*if (e. != null) {
				Accepted = e.Socket;
				Accepted.DataReceived += callback;
				Accepted.Disconnected += callback;
			}*/
			RealmServiceEventArgs = e;
			Wait.Set();
		}

		public void Cleanup() {


		}

		public bool WaitFor(ThreadHandler h) {
			lock (_lock) {
				Wait.Reset();
				h();
				var res = Wait.WaitOne(TimeSpan.FromSeconds(1));
				return res;
			}
		}

		public IPEndPoint EndPoint { get; set; }
		public RealmService Service { get; set; }
		public RealmClient Client { get; set; }
		public RealmClient Accepted { get; set; }		
		private ManualResetEvent Wait { get; set; }
		public RealmServiceEventArgs RealmServiceEventArgs { get; set; }
		static int _port = 1025;
		static object _lock = new object();
	}
}
