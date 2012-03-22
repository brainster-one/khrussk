
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

			Assert.IsTrue(_context.WaitFor(() => _context.AcceptedSockets.Count() == 1, _context.WaitingPeriod));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Data should be transfered to server.</summary>
		[TestMethod] public void DataShouldBeTransferedToRemoteSideTest() {
			_context.ClientSocket.Send(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
			Assert.IsTrue(_context.WaitFor(() => _context.DataReceived.Count() == 1, _context.WaitingPeriod));
			Assert.AreEqual(5, _context.DataReceived.First().Length);
		}

		/// <summary>Data should be transfered to client side.</summary>
		[TestMethod] public void DataShouldBeTransferedToClientSideTest() {
			_context.AcceptedSockets.First().Send(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
			Assert.IsTrue(_context.WaitFor(() => _context.DataReceived.Count() == 1, _context.WaitingPeriod));
			Assert.AreEqual(5, _context.DataReceived.First().Length);
		}

		[TestMethod] public void BigDataChunkTest() {
			byte[] buffer = new byte[1024 * 1024];
			_context.ClientSocket.Send(buffer, 0, buffer.Length);

			_context.WaitFor(() => false, 1000);
			var l = _context.DataReceived.ToArray().Sum(x => x.Length);
			Assert.AreEqual(1024 * 1024, l);
		}

		[TestMethod] public void LotOfSmallDataChunksTest() {
			for (int i = 0; i < 2048; ++i) {
				byte[] buffer = new byte[3];
				_context.ClientSocket.Send(buffer, 0, buffer.Length);
			}

			_context.WaitFor(() => false, 1000);
			var l = _context.DataReceived.ToArray().Sum(x => x.Length);
			Assert.AreEqual(2048 * 3, l);
		}
	}
}
