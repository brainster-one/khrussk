﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khrussk.Realm.Protocol;

namespace Khrussk.Tests.Realm {
	class Player : IEntity {
		public Player() {
			Name = String.Empty;
		}
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class PlayerSerializer : IEntitySerializer {

		public void Serialize(System.IO.BinaryWriter writer, IEntity entity) {
			writer.Write((entity as Player).Name);
		}

		public void Deserialize(System.IO.BinaryReader reader, ref IEntity entity) {
			var player = (entity == null ? new Player() : (Player)entity); 
			player.Name = reader.ReadString();
			entity = player;
		}
	}
}