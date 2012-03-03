
namespace Khrussk.Tests.Realm {
	using System;
	using System.IO;
	using Khrussk.Realm.Protocol;

	/// <summary>TestEntity</summary>
	sealed class TestEntity : IEntity {
		/// <summary>Initializes new instance of TestEntity.</summary>
		public TestEntity() {
			Name = String.Empty;
		}
		
		/// <summary>Gets or sets Id.</summary>
		public int Id { get; set; }

		/// <summary>Gets or sets name.</summary>
		public string Name { get; set; }
	}

	/// <summary>TestEntity serializer.</summary>
	sealed class TestEntitySerializer : IEntitySerializer {
		/// <summary>Serializes entity to stream.</summary>
		/// <param name="writer">Writer to serialize entity by.</param>
		/// <param name="entity">Entity to serialize.</param>
		public void Serialize(BinaryWriter writer, IEntity entity) {
			writer.Write((entity as TestEntity).Name);
		}

		/// <summary>Deserializes entity from stream.</summary>
		/// <param name="reader">Reader to deserialize entity by.</param>
		/// <param name="entity">Entity.</param>
		public void Deserialize(BinaryReader reader, ref IEntity entity) {
			var ent = (entity == null ? new TestEntity() : (TestEntity)entity); 
			ent.Name = reader.ReadString();
			entity = ent;
		}
	}
}
