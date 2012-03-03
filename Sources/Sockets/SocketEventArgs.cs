
namespace Khrussk.Sockets {
	using System;

	/// <summary>Socket event arguments.</summary>
	public class SocketEventArgs : EventArgs {
		/// <summary>Initializes a new instance of the SocketEventArgs class using the specified socket.</summary>
		/// <param name="socket">Socket.</param>
		public SocketEventArgs(Socket socket) {
			Socket = socket;
		}

		/// <summary>Initializes a new instance of the SocketEventArgs class using the specified socket and buffer.</summary>
		/// <param name="socket">Socket.</param>
		/// <param name="buffer">Buffer.</param>
		public SocketEventArgs(Socket socket, byte[] buffer) {
			Socket = socket;
			Buffer = buffer;
		}
		
		/// <summary>Gets socket.</summary>
		public Socket Socket { get; private set; }

		/// <summary>Gets buffer.</summary>
		public byte[] Buffer { get; private set; }
	}
}
