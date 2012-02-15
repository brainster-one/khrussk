using Uberball.Engine.Network.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;

namespace Uberball.Tests.Engine.Network {


	/// <summary>
	///This is a test class for ListenerSocketTests and is intended
	///to contain all ListenerSocketTests Unit Tests
	///</summary>
	[TestClass()]
	public class SocketsCommunicationTests {
		IPEndPoint _endpoint = new IPEndPoint(IPAddress.Loopback, 9876);
		ListenerSocket _listener = new ListenerSocket();
		ClientSocket _client = new ClientSocket();
		ClientSocket _accepted;
		ManualResetEvent _connectionEvent = new ManualResetEvent(false);

		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_listener.ClientSocketAccepted += _listener_ClientSocketAccepted;
			_listener.Listen(_endpoint);
			_client.Connect(_endpoint);
			
			_connectionEvent.WaitOne(1000);
			Assert.IsTrue(_client.IsConnected);
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			if (_accepted != null) _accepted.Disconnect();
			_client.Disconnect();
			_listener.Disconnect();
		}

		/// <summary>
		///A test for Listen
		///</summary>
		[TestMethod()]
		public void ListenTest() {
			Assert.IsTrue(_client.IsConnected);
		}


		void client_Connected(object sender, SocketEventArgs e) {
			return;
		}
		
		void _listener_ClientSocketAccepted(object sender, SocketEventArgs e) {
			_accepted = e.ClientSocket;
			_connectionEvent.Set();
		}
	}
}
