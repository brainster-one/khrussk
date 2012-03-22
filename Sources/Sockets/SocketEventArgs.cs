
namespace Khrussk.Sockets {
	using System;

	/// <summary>Socket event arguments.</summary>
	public class SocketEventArgs : EventArgs {
		/// <summary>Initializes a new instance of the SocketEventArgs class using the specified socket and connection state.</summary>
		/// <param name="socket">Socket.</param>
		/// <param name="connectionState">Socket connection state.</param>
		internal SocketEventArgs(Socket socket, ConnectionState connectionState) {
			Socket = socket;
			ConnectionState = connectionState;
		}

		/// <summary>Initializes a new instance of the SocketEventArgs class using the specified socket and buffer.</summary>
		/// <param name="socket">Socket.</param>
		/// <param name="buffer">Buffer.</param>
		internal SocketEventArgs(Socket socket, byte[] buffer) {
			Socket = socket;
			Buffer = buffer;
		}
		
		/// <summary>Gets socket.</summary>
		public Socket Socket { get; private set; }

		/// <summary>Gets connection state.</summary>
		public ConnectionState ConnectionState { get; private set; }

		/// <summary>Gets buffer.</summary>
		public byte[] Buffer { get; private set; }
	}
}
