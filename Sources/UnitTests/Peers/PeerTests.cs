﻿

namespace Khrussk.Tests.UnitTests.Peers {
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Protocol;

	[TestClass] public class PeerTests {
		readonly PeerTestContext _context = new PeerTestContext();
		
		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
			_context.Listener.Listen(_context.EndPoint);
			_context.Peer.Connect(_context.EndPoint);

			Assert.IsTrue(_context.WaitFor(() => _context.AcceptedPeers.Count() == 1, _context.WaitingPeriod));
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
			_context.Cleanup();
		}

		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod] public void ConnectionShouldBeEstablishedTest() {
			Assert.IsTrue(_context.AcceptedPeers.Count() == 1);
		}
		
		/// <summary>Data from remote host should be read.</summary>
		[TestMethod] public void DataFromRemoteHostShouldBeRead() {
			_context.AcceptedPeers.First().Send(new Packet { Data = 127 });

			Assert.IsTrue(_context.WaitFor(() => _context.Packets.Count() == 1, _context.WaitingPeriod));
			Assert.AreEqual(127, ((Packet)_context.Packets.First()).Data);
		}
		
		/// <summary>Data from remote host should be read.</summary>
		[TestMethod] public void DataFromLocalHostShouldBeRead() {
			_context.Peer.Send(new Packet { Data = 127 });

			Assert.IsTrue(_context.WaitFor(() => _context.Packets.Count() == 1, 10000));
			Assert.AreEqual(127, ((Packet)_context.Packets.First()).Data);
		}
	}
}
