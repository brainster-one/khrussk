

namespace Khrussk.Examples.Chat {
	using System;
	using System.Globalization;
	using System.Net;
	using NetworkRealm;
	using Protocol;

	class Service {
		public Service() {
			_service.UserConnectionStateChanged += OnUserConnectionStateChanged;
			_service.PacketReceived += OnPacketReceived;
		}

		public void Start(IPEndPoint endpoint) {
			_service.Start(endpoint);
		}

		void OnUserConnectionStateChanged(object sender, ConnectionEventArgs e) {
			if (e.ConnectionState == ConnectionState.Connected) {
				var player = new Player { Name = "plr_" + DateTime.Now.Millisecond.ToString(CultureInfo.InvariantCulture) };
				e.User["player"] = player;
				_service.AddEntity(player);
				Console.WriteLine("'{0}' connected", player.Name);
			} else if (e.ConnectionState == ConnectionState.Disconnected) {
				var player = (Player)e.User["player"];
				_service.RemoveEntity(player);
				Console.WriteLine("'{0}' disconnected", player.Name);
			}
		}

		void OnPacketReceived(object sender, PacketEventArgs e) {
			var packet = (MessagePacket)e.Packet;
			var player = (Player)e.User["player"];
			var text = string.Format("{0}: {1}", player.Name, packet.Content);
			Console.WriteLine(text);
			_service.SendAll(new MessagePacket { Content = text });
		}

		readonly RealmService _service = new RealmService(new ChatProtocol());
	}
}
