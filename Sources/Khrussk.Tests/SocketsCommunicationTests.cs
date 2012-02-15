using Khrussk.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace Khrussk.Network {


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
		ManualResetEvent _receivedEvent = new ManualResetEvent(false);
		SocketEventArgs _args;

		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_listener.ClientSocketAccepted += _listener_ClientSocketAccepted;
			_client.DataReceived += new EventHandler<SocketEventArgs>(_client_DataReceived);
			_client.Connected += new EventHandler<SocketEventArgs>(_client_Connected);
			_listener.Listen(_endpoint);
			_client.Connect(_endpoint);
			
			_connectionEvent.WaitOne(1000);
			Debug.Print("1:" + _client.IsConnected.ToString());
			//Assert.IsTrue(_client.IsConnected);
			Debug.Print("2:" + _client.IsConnected.ToString());
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
			Debug.Print("3:" + _client.IsConnected.ToString());
			//Assert.IsTrue(_client.IsConnected);
			Debug.Print("4:" + _client.IsConnected.ToString());
		}

		[TestMethod()]
		public void SendTest() {
			_accepted.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
			_receivedEvent.WaitOne(5000);
			Assert.IsNotNull(_args);
			Assert.AreEqual(5, _args.Buffer.Length);
		}


		void _client_Connected(object sender, SocketEventArgs e) {
			Debug.Print(_client.IsConnected.ToString());
			_connectionEvent.Set();
		}
		
		void _listener_ClientSocketAccepted(object sender, SocketEventArgs e) {
			_accepted = e.ClientSocket;
			
		}

		void _client_DataReceived(object sender, SocketEventArgs e) {
			_receivedEvent.Set();
			_args = e;
		}

	}
}
