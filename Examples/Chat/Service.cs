

namespace Khrussk.Examples.Chat {
	using System;
	using System.Globalization;
	using System.Net;
	using NetworkRealm;
	using Protocol;

	class Service {
		public Service() {
			_service.UserConnected += OnUserConnected;
			_service.UserDisconnected += OnUserDisconnected;
			_service.PacketReceived += OnPacketReceived;
		}

		public void Start(IPEndPoint endpoint) {
			_service.Start(endpoint);
		}

		void OnUserConnected(object sender, RealmServiceEventArgs e) {
			var player = new Player { Name = DateTime.Now.Millisecond.ToString(CultureInfo.InvariantCulture) };
			e.User["player"] = player;
			_service.AddEntity(player);
			Console.WriteLine("Service: User '{0}' connected. Player '{1}' created.", e.User.Session, player.Name);
		}

		void OnUserDisconnected(object sender, RealmServiceEventArgs e) {
			var player = (Player)e.User["player"];
			Console.WriteLine("Service: User '{0}' disconnected. Player '{1}' removed.", e.User.Session, player.Name);
		}

		void OnPacketReceived(object sender, RealmServiceEventArgs e) {
			var packet = (MessagePacket)e.Packet;
			Console.WriteLine("Service: Message '{0}' received. Sending to all connected clients.", packet.Content);
			_service.SendAll(e.Packet);
		}

		readonly RealmService _service = new RealmService(new ChatProtocol());
	}
}
