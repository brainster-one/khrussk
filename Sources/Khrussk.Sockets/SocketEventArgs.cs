using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khrussk.Sockets {
	public class SocketEventArgs : EventArgs {
		public SocketEventArgs() {

		}

		public SocketEventArgs(ClientSocket socket) {
			ClientSocket = socket;
		}

		public ClientSocket ClientSocket { get; private set; }
	}
}
