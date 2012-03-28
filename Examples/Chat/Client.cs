

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

		void OnConnectionStateChanged(object sender, RealmClientEventArgs e) {
			Console.WriteLine("Connection state is '{0}'", e.ConnectionState);
		}

		void OnPacketReceived(object sender, RealmClientEventArgs e) {
			var packet = e.Packet as MessagePacket;
			if (packet != null) Console.WriteLine(packet.Content);
		}

		void OnEntityStateChanged(object sender, RealmClientEventArgs e) {
			var entity = (Player)e.EntityInfo.Entity;
			Console.WriteLine("'{0}' state is {1}", entity.Name, e.EntityInfo.Action);
		}

		readonly RealmClient _client = new RealmClient(new ChatProtocol());
	}
}
