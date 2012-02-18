
namespace Khrussk.Tests.Sockets {
	using System;
	using Khrussk.Tests;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary></summary>
	[TestClass] public class SocketsInterfaceTests {
		readonly Context _context = new Context();
		
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
	}
}
