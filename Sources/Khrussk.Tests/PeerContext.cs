using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Khrussk.Sockets;
using System.Threading;
using Khrussk.Peers;

namespace Khrussk.Tests {
	class PeerContext {
		public PeerContext() {
			ListenerPeer = new ListenerPeer(new Protocol());
			ClientPeer = new ClientPeer(new Protocol());

			ClientPeer.Connected += ClientSocket_Connected;
			ClientPeer.PacketReceived += ClientSocket_DataReceived;
			ClientPeer.Disconnected += ClientSocket_Disconnected;
			ListenerPeer.ClientPeerConnected += ListenerSocket_ClientSocketAccepted;

			ClientSocketConnectedEvent = new ManualResetEvent(false);
			ClientSocketDataReceivedEvent = new ManualResetEvent(false);
			ClientSocketAccepted = new ManualResetEvent(false);
			Wait = new ManualResetEvent(false);

			EndPoint = new IPEndPoint(IPAddress.Loopback, new Random().Next(1025, 65535));
		}

		void ClientSocket_Disconnected(object sender, PeerEventArgs e) {
			Wait.Set();
		}

		public void Cleanup() {
			if (Accepted != null) Accepted.Disconnect();
			ClientPeer.Disconnect();
			ListenerPeer.Disconnect();
		}

		void ListenerSocket_ClientSocketAccepted(object sender, PeerEventArgs e) {
			Accepted = e.Client;
			Accepted.PacketReceived += ClientSocket_DataReceived;
			SocketEventArgs = e;
			ClientSocketAccepted.Set();
		}

		void ClientSocket_DataReceived(object sender, PeerEventArgs e) {
			SocketEventArgs = e;
			ClientSocketDataReceivedEvent.Set();
		}

		void ClientSocket_Connected(object sender, PeerEventArgs e) {
			SocketEventArgs = e;
			ClientSocketConnectedEvent.Reset();
		}

		public IPEndPoint EndPoint { get; set; }

		public ListenerPeer ListenerPeer { get; set; }
		public ClientPeer ClientPeer { get; set; }
		public ClientPeer Accepted { get; set; }
		
		public ManualResetEvent ClientSocketConnectedEvent { get; set; }
		public ManualResetEvent ClientSocketDataReceivedEvent { get; set; }
		public ManualResetEvent ClientSocketAccepted { get; set; }
		public ManualResetEvent Wait { get; set; }
		public PeerEventArgs SocketEventArgs { get; set; }
	}
}
