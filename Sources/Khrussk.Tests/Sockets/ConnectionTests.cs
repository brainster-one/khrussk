
namespace Khrussk.Tests.Sockets {
	using System;
	using Khrussk.Tests;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary></summary>
	[TestClass] public class ConnectionsTests {
		readonly SocketTestContext _context = new SocketTestContext();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.ListenerSocket.Listen(_context.EndPoint);
			
			_context.WaitFor(() => _context.ClientSocket.Connect(_context.EndPoint));
			Assert.IsNotNull(_context.Accepted);
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Connection between sockets should be established.</summary>
		[TestMethod()] public void ConnectionShouldBeEstablishedTest() {
			Assert.IsNotNull(_context.Accepted);
		}

		/// <summary>Connection should be closed after disconnection on client side.</summary>
		[TestMethod] public void ConnectionShouldBeClosedAfterDisconnectionOnClientSideTest() {
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSocket.Disconnect()));
			Assert.IsNotNull(_context.Accepted);
		}

		/// <summary>Connection should be closed after disconnection on remote side (IsConnected == false on both sides).</summary>
		[TestMethod] public void ConnectionShouldBeClosedAfterDisconnectionOnRemoteSideTest() {
			Assert.IsTrue(_context.WaitFor(() => _context.Accepted.Disconnect()));
		}

		/// <summary>Socket can be disconnected serveral times.</summary>
		[TestMethod] public void ConnectionCanBeClosedSeveralTimesTest() {
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSocket.Disconnect()));
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSocket.Disconnect()));
		}

		/// <summary>Can not connect twice.</summary>
		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void ConnectionShouldNotBeEstablishedSeveralTimesTest() {
			_context.ClientSocket.Connect(_context.EndPoint);
		}

		/// <summary>Recconnection works properly.</summary>
		[TestMethod] public void ConnectionCanBeReusedTest() {
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSocket.Disconnect()));
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSocket.Connect(_context.EndPoint)));
		}

		/// <summary>Can not listen twice.</summary>
		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void CanNotListenSeveralTimes() {
			_context.ListenerSocket.Listen(_context.EndPoint);
		}
	}
}
