
namespace Khrussk.Tests.Sockets {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>Data transfer tests.</summary>
	[TestClass] public class DataTransferTests {
		readonly SocketTestContext _context = new SocketTestContext();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.ListenerSocket.Listen(_context.EndPoint);
			_context.ClientSocket.Connect(_context.EndPoint);

			Assert.IsTrue(_context.WaitFor(() => _context.AcceptedSockets.Count() == 1, 1000));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Data should be transfered to server.</summary>
		[TestMethod] public void DataShouldBeTransferedToRemoteSideTest() {
			_context.ClientSocket.Send(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
			Assert.IsTrue(_context.WaitFor(() => _context.DataReceived.Count() == 1, 1000));
			Assert.AreEqual(5, _context.DataReceived.First().Length);
		}

		/// <summary>Data should be transfered to client side.</summary>
		[TestMethod] public void DataShouldBeTransferedToClientSideTest() {
			_context.AcceptedSockets.First().Send(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
			Assert.IsTrue(_context.WaitFor(() => _context.DataReceived.Count() == 1, 1000));
			Assert.AreEqual(5, _context.DataReceived.First().Length);
		}
	}
}
