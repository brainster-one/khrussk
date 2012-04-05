
namespace Khrussk.Tests.UnitTests.Realm.Shared {
	using System;
	using System.IO;
	using NetworkRealm.Protocol;

	sealed class TestEntity2 {
		public TestEntity2()
		{
			Int64 = Int64.MaxValue;
			Int32 = Int32.MaxValue;
			Int16 = Int16.MaxValue;
			Double = Double.MaxValue;
			Float = float.MaxValue;
			String = "TEST";
		}

		public Int64 Int64 { get; set; }
		public Int32 Int32 { get; set; }
		public Int16 Int16 { get; set; }
		public Double Double { get; set; }
		public Single Float { get; set; }
		public string String { get; set; }

	};

	/// <summary>TestEntity</summary>
	sealed class TestEntity {
		/// <summary>Initializes new instance of TestEntity.</summary>
		public TestEntity() {
			Name = String.Empty;
		}

		/// <summary>Gets or sets name.</summary>
		public string Name { get; set; }
	}

	/// <summary>TestEntity serializer.</summary>
	sealed class TestEntitySerializer : IEntitySerializer<TestEntity> {
		/// <summary>Serializes entity to stream.</summary>
		/// <param name="writer">Writer to serialize entity by.</param>
		/// <param name="entity">Entity to serialize.</param>
		public void Serialize(BinaryWriter writer, TestEntity entity) {
			writer.Write(entity.Name);
		}

		/// <summary>Deserializes entity from stream.</summary>
		/// <param name="reader">Reader to deserialize entity by.</param>
		/// <param name="entity">Entity.</param>
		public void Deserialize(BinaryReader reader, ref TestEntity entity) {
			var ent = (entity ?? new TestEntity());
			ent.Name = reader.ReadString();
			entity = ent;
		}
	}
}
