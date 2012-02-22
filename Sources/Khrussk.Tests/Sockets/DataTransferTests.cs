
namespace Khrussk.Tests.Sockets {
	using System;
	using Khrussk.Tests;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using System.Linq;

	/// <summary></summary>
	[TestClass] public class DataTransferTests {
		readonly SocketTestContext _context = new SocketTestContext();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.ListenerSocket.Listen(_context.EndPoint);

			_context.ClientSockets.First().Connect(_context.EndPoint);
			Assert.AreEqual(1, _context.AcceptedSockets.Count());
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Data should be transfered to server.</summary>
		[TestMethod] public void DataShouldBeTransferedToRemoteSideTest() {
			_context.ClientSocket.Send(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
			Assert.AreEqual(5, _context.SocketEventArgs.Buffer.Length);
		}

		/// <summary>Data should be transfered to client side.</summary>
		[TestMethod] public void DataShouldBeTransferedToClientSideTest() {
			_context.AcceptedSockets.First().Send(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
			Assert.AreEqual(5, _context.SocketEventArgs.Buffer.Length);
		}
	}
}
