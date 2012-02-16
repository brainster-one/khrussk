using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Khrussk.Sockets;
using System.Threading;

namespace Khrussk.Tests {
	class Context {
		public Context() {
			ListenerSocket = new ListenerSocket();
			ClientSocket = new ClientSocket();

			ClientSocket.Connected += new EventHandler<Sockets.SocketEventArgs>(ClientSocket_Connected);
			ClientSocket.DataReceived += new EventHandler<Sockets.SocketEventArgs>(ClientSocket_DataReceived);
			ClientSocket.Disconnected += new EventHandler<Sockets.SocketEventArgs>(ClientSocket_Disconnected);
			ListenerSocket.ClientSocketAccepted += new EventHandler<Sockets.SocketEventArgs>(ListenerSocket_ClientSocketAccepted);

			ClientSocketConnectedEvent = new ManualResetEvent(false);
			ClientSocketDataReceivedEvent = new ManualResetEvent(false);
			ClientSocketAccepted = new ManualResetEvent(false);
			Wait = new ManualResetEvent(false);

			EndPoint = new IPEndPoint(IPAddress.Loopback, new Random().Next(1025, 65535));
		}

		void ClientSocket_Disconnected(object sender, SocketEventArgs e) {
			Wait.Set();
		}

		public void Cleanup() {
			if (Accepted != null) Accepted.Disconnect();
			ClientSocket.Disconnect();
			ListenerSocket.Disconnect();
		}

		void ListenerSocket_ClientSocketAccepted(object sender, SocketEventArgs e) {
			Accepted = e.ClientSocket;
			Accepted.DataReceived += new EventHandler<Sockets.SocketEventArgs>(ClientSocket_DataReceived);
			SocketEventArgs = e;
			ClientSocketAccepted.Set();
		}

		void ClientSocket_DataReceived(object sender, SocketEventArgs e) {
			SocketEventArgs = e;
			ClientSocketDataReceivedEvent.Set();
		}

		void ClientSocket_Connected(object sender, SocketEventArgs e) {
			SocketEventArgs = e;
			ClientSocketConnectedEvent.Reset();
		}

		public IPEndPoint EndPoint { get; set; }

		public ListenerSocket ListenerSocket { get; set; }
		public ClientSocket ClientSocket { get; set; }
		public ClientSocket Accepted { get; set; }
		
		public ManualResetEvent ClientSocketConnectedEvent { get; set; }
		public ManualResetEvent ClientSocketDataReceivedEvent { get; set; }
		public ManualResetEvent ClientSocketAccepted { get; set; }
		public ManualResetEvent Wait { get; set; }
		public SocketEventArgs SocketEventArgs { get; set; }
	}
}
