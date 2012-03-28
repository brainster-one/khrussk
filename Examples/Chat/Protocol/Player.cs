
namespace Khrussk.Examples.Chat {
	using System.IO;
	using NetworkRealm.Protocol;

	class Player {
		public string Name { get; set; }
	}

	class PlayerSerializer : IEntitySerializer<Player> {
		public void Deserialize(BinaryReader reader, ref Player entity) {
			entity = entity ?? new Player();
			entity.Name = reader.ReadString();
		}

		public void Serialize(BinaryWriter writer, Player entity) {
			writer.Write(entity.Name);
		}
	}
}
