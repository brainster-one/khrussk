
using System.Globalization;

namespace Khrussk.Tests.Sockets {
	using System.Collections.Generic;
	using System.Linq;
	using Khrussk.Sockets;
	using System.Diagnostics;

	/// <summary>Context for tests.</summary>
	sealed class SocketTestContext : BasicTestContext {
		/// <summary>Initializes a new instance of the SocketTestContext class.</summary>
		public SocketTestContext() {
			ClientSockets = new List<Socket>();
			AcceptedSockets = new List<Socket>();
			DataReceived = new List<byte[]>();

			ClientSocket = new Socket();
			ClientSocket.ConnectionStateChanged += OnConnectionStateChanged;
			ClientSocket.DataReceived += OnDataReceived;
			
			ListenerSocket = new Socket();
			ListenerSocket.ConnectionAccepted += OnClientSocketAccepted;
		}

		/// <summary>Cleanup environment.</summary>
		public void Cleanup() {
			ClientSockets.ToList().ForEach(x => x.Disconnect());
			//AcceptedSockets.ToList().ForEach(x => x.Disconnect());
			foreach (var socket in AcceptedSockets.ToList()) {
				socket.Disconnect();
			}
			ListenerSocket.Disconnect();
		}

		/// <summary>Creates new socket.</summary>
		/// <returns>New socket.</returns>
		public Socket NewSocket() {
			var ns = new Socket();
			ClientSockets.Add(ns);
			return ns;
		}

		/// <summary>Gets client socket.</summary>
		public Socket ClientSocket { get; private set; }

		/// <summary>Gets listener socket.</summary>
		public Socket ListenerSocket { get; private set; }

		/// <summary>Gets list of client's sockets.</summary>
		public List<Socket> ClientSockets { get; private set; }

		/// <summary>Gets list of accepted sockets.</summary>
		public List<Socket> AcceptedSockets { get; private set; }

		/// <summary>Gets list of received data chunks.</summary>
		public List<byte[]> DataReceived { get; private set; }

		/// <summary>On client socket connected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnConnectionStateChanged(object sender, SocketEventArgs e) {
			if (e.ConnectionState == ConnectionState.Disconnected)
				if (ClientSockets.Contains(e.Socket)) ClientSockets.Remove(e.Socket);
		}

		/// <summary>On connection accepted</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnClientSocketAccepted(object sender, SocketEventArgs e) {
			lock (AcceptedSockets) {
				AcceptedSockets.Add(e.Socket);
				e.Socket.DataReceived += OnDataReceived;
				e.Socket.ConnectionStateChanged += OnAcceptedSocketDisconnected;
				Debug.Print("ACCEPTED! " + AcceptedSockets.Count);
			}
		}

		/// <summary>On socket disconnected.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnAcceptedSocketDisconnected(object sender, SocketEventArgs e) {
			lock (AcceptedSockets) {
				if (e.ConnectionState == ConnectionState.Disconnected) {
					if (AcceptedSockets.Contains(e.Socket)) {
						Debug.Print("DISCONNECTED! " + AcceptedSockets.Count);
						AcceptedSockets.Remove(e.Socket);
					}
				}
			}
		}

		/// <summary>On data received.</summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		void OnDataReceived(object sender, SocketEventArgs e) {
			DataReceived.Add(e.Buffer);
			Debug.Print(e.Buffer.Length.ToString(CultureInfo.InvariantCulture));
		}
	}
}
