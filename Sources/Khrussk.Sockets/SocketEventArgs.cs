using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khrussk.Sockets {
	public class SocketEventArgs : EventArgs {
		private byte[] p;
		private int p_2;

		public SocketEventArgs() {

		}

		public SocketEventArgs(ClientSocket socket) {
			ClientSocket = socket;
		}

		public SocketEventArgs(ClientSocket clientSocket, byte[] buffer, int count) {
			ClientSocket = clientSocket;
			Buffer = new byte[count];
			System.Buffer.BlockCopy(buffer, 0, Buffer, 0, count);
			
		}
		public ClientSocket ClientSocket { get; private set; }
		public byte[] Buffer { get; private set; }
	}
}
