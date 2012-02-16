
namespace Khrussk.Network {
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Threading;
	using Khrussk.Sockets;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
using Khrussk.Tests;

	/// <summary>
	///This is a test class for ListenerSocketTests and is intended
	///to contain all ListenerSocketTests Unit Tests
	///</summary>
	[TestClass()] public class SocketsCommunicationTests {
		readonly Context _context = new Context();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.ListenerSocket.Listen(_context.EndPoint);
			_context.ClientSocket.Connect(_context.EndPoint);
			_context.ClientSocketAccepted.WaitOne(TimeSpan.FromSeconds(1));

			Assert.IsTrue(_context.ClientSocket.IsConnected);
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod()] public void ConnectionShouldBeEstablishedTest() {
			Assert.IsTrue(_context.ClientSocket.IsConnected);
		}

		/// <summary>Data from remote host should be read.</summary>
		[TestMethod()] public void IsConnectedEqualsFalseAfterDisconnect() {
			_context.ClientSocket.Disconnect();

			Assert.IsTrue(_context.Wait.WaitOne(TimeSpan.FromSeconds(1)));
			Assert.IsFalse(_context.ClientSocket.IsConnected);
		}

		/// <summary>Data from remote host should be read.</summary>
		[TestMethod()] public void DataFromRemoteHostShouldBeRead() {
			_context.Accepted.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
			_context.ClientSocketDataReceivedEvent.WaitOne(TimeSpan.FromSeconds(1));
			
			Assert.IsNotNull(_context.SocketEventArgs);
			Assert.AreEqual(5, _context.SocketEventArgs.Buffer.Length);
		}

		/// <summary>Data from remote host should be read.</summary>
		[TestMethod()] public void DataFromLocalHostShouldBeRead() {
			_context.ClientSocket.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);

			Assert.IsTrue(_context.ClientSocketDataReceivedEvent.WaitOne(TimeSpan.FromSeconds(1)));
			Assert.IsNotNull(_context.SocketEventArgs);
			Assert.AreEqual(5, _context.SocketEventArgs.Buffer.Length);
		}
	}
}
