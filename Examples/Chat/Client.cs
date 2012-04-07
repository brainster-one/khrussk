

namespace Khrussk.Examples.Chat {
	using System;
	using System.Net;
	using NetworkRealm;
	using Protocol;

	class Client {
		public Client() {
			_client.ConnectionStateChanged += OnConnectionStateChanged;
			_client.PacketReceived += OnPacketReceived;
			_client.EntityStateChanged += OnEntityStateChanged;
		}

		public void Connect(IPEndPoint endpoint) {
			_client.Connect(endpoint);
		}

		public void Say(string msg) {
			_client.Send(new MessagePacket { Content = msg });
		}

		void OnConnectionStateChanged(object sender, ConnectionEventArgs e) {
			Console.WriteLine("Connection state is '{0}'", e.State);
		}

		void OnPacketReceived(object sender, PacketEventArgs e) {
			var packet = e.Packet as MessagePacket;
			if (packet != null) Console.WriteLine(packet.Content);
		}

		void OnEntityStateChanged(object sender, EntityEventArgs e) {
			var entity = (Player)e.Entity;
			Console.WriteLine("'{0}' state is {1}", entity.Name, e.State);
		}

		readonly RealmClient _client = new RealmClient(new ChatProtocol());
	}
}
