
namespace Khrussk.Tests.Sockets {
	using System;
	using Khrussk.Tests;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary></summary>
	[TestClass] public class SocketsCommunicationTests {
		readonly Context _context = new Context();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.ListenerSocket.Listen(_context.EndPoint);
			
			_context.WaitFor(() => _context.ClientSocket.Connect(_context.EndPoint));
			Assert.IsTrue(_context.ClientSocket.IsConnected);
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Connection between sockets should be established.</summary>
		[TestMethod()] public void ConnectionShouldBeEstablishedTest() {
			Assert.IsTrue(_context.ListenerSocket.IsConnected);
			Assert.IsTrue(_context.ClientSocket.IsConnected);
		}

		/// <summary>Connection should be closed after disconnection on client side (IsConnected == false on both sides).</summary>
		[TestMethod] public void ConnectionShouldBeClosedAfterDisconnect() {
			Assert.IsTrue(_context.WaitFor(() => _context.ClientSocket.Disconnect()));
			Assert.IsFalse(_context.ClientSocket.IsConnected);
			Assert.IsFalse(_context.Accepted.IsConnected);
		}

		/// <summary>Connection should be closed after disconnection on remote side (IsConnected == false on both sides).</summary>
		[TestMethod] public void ConnectionShouldBeClosedAfterDisconnectOnRemoteSideTest() {
			Assert.IsTrue(_context.WaitFor(() => _context.Accepted.Disconnect()));
			Assert.IsFalse(_context.Accepted.IsConnected);
			Assert.IsFalse(_context.ClientSocket.IsConnected); // todo: ClientSocket.IsConnected == true
		}

		/// <summary>Data should be transfered to server.</summary>
		[TestMethod] public void DataShouldBeTransferedToRemoteSideTest() {
			Assert.IsTrue(_context.WaitFor(() => {
				_context.ClientSocket.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
			}));
			Assert.AreEqual(5, _context.SocketEventArgs.Buffer.Length);
		}

		/// <summary>Data should be transfered to client side.</summary>
		[TestMethod] public void DataShouldBeTransferedToClientSideTest() {
			Assert.IsTrue(_context.WaitFor(() => {
				_context.Accepted.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
			}));
			Assert.AreEqual(5, _context.SocketEventArgs.Buffer.Length);
		}
	}
}
