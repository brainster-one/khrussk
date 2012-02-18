
namespace Khrussk.Network {
	using System;
	using Khrussk.Tests;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>
	///This is a test class for ListenerSocketTests and is intended
	///to contain all ListenerSocketTests Unit Tests
	///</summary>
	[TestClass()] public class PeerTests {
		readonly PeerContext _context = new PeerContext();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.ListenerPeer.Listen(_context.EndPoint);
			_context.ClientPeer.Connect(_context.EndPoint);
			_context.ClientSocketAccepted.WaitOne(TimeSpan.FromSeconds(1));

			Assert.IsTrue(_context.ClientPeer.IsConnected);
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod()] public void ConnectionShouldBeEstablishedTest() {
			Assert.IsTrue(_context.ClientPeer.IsConnected);
		}
		/*
		/// <summary>Data from remote host should be read.</summary>
		[TestMethod()] public void IsConnectedEqualsFalseAfterDisconnect() {
			_context.ClientPeer.Disconnect();

			Assert.IsTrue(_context.Wait.WaitOne(TimeSpan.FromSeconds(1)));
			Assert.IsFalse(_context.ClientPeer.IsConnected);
		}
		 */
		
		/// <summary>Data from remote host should be read.</summary>
		[TestMethod()] public void DataFromRemoteHostShouldBeRead() {
			_context.Accepted.Send(new Packet { Data = 127 });
			_context.ClientSocketDataReceivedEvent.WaitOne(TimeSpan.FromSeconds(10));
			
			Assert.IsNotNull(_context.SocketEventArgs);
			Assert.IsNotNull(_context.SocketEventArgs.Packet);
			Assert.AreEqual(127, ((Packet)_context.SocketEventArgs.Packet).Data);
		}
		
		/// <summary>Data from remote host should be read.</summary>
		[TestMethod()] public void DataFromLocalHostShouldBeRead() {
			_context.ClientPeer.Send(new Packet { Data = 127 });
			_context.ClientSocketDataReceivedEvent.WaitOne(TimeSpan.FromSeconds(10));

			Assert.IsNotNull(_context.SocketEventArgs);
			Assert.IsNotNull(_context.SocketEventArgs.Packet);
			Assert.AreEqual(127, ((Packet)_context.SocketEventArgs.Packet).Data);
		}
	}
}
