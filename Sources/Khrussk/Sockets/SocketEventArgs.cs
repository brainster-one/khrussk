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

		public SocketEventArgs(Socket socket) {
			ClientSocket = socket;
		}

		public SocketEventArgs(Socket clientSocket, byte[] buffer, int count) {
			ClientSocket = clientSocket;
			Buffer = new byte[count];
			System.Buffer.BlockCopy(buffer, 0, Buffer, 0, count);
			
		}
		public Socket ClientSocket { get; private set; }
		public byte[] Buffer { get; private set; }
	}
}
