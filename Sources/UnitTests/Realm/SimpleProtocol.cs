
using System.Net;

namespace Khrussk.Tests.UnitTests.Realm {
	using System;
	using System.IO;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using NetworkRealm.Protocol;
	using Shared;
	using Khrussk.NetworkRealm;

	[TestClass]
	public class SimpleProtocolTests {
		[TestMethod]
		public void SimpleSerializerTest() {
			var stream = new MemoryStream();
			var reader = new BinaryReader(stream);
			var writer = new BinaryWriter(stream);

			var serializer = new SimpleEntitySerializer<TestEntity2>();
			serializer.Serialize(writer, new TestEntity2(), new SerializationInfo());

			var ent = new TestEntity2(); // TODO: CREATE EMPTY ENTITY
			stream.Position = 0;
			serializer.Deserialize(reader, ref ent, new SerializationInfo());

			Assert.AreEqual(Int64.MaxValue, ent.Int64);
			Assert.AreEqual(Int32.MaxValue, ent.Int32);
			Assert.AreEqual(Int16.MaxValue, ent.Int16);
			Assert.AreEqual(Double.MaxValue, ent.Double);
			Assert.AreEqual(Single.MaxValue, ent.Float);
			Assert.AreEqual("TEST", ent.String);
		}

		[TestMethod]
		public void SimpleProtocolTest() {/*
			var strm = new MemoryStream();
			var ent = new TestEntity2();

			var prot = new SimpleProtocol(new[] { typeof(TestEntity2) });
			prot.Write(strm, new AddEntityPacket(1, ent));

			strm.Position = 0;
			var packet = prot.Read(strm) as AddEntityPacket;
			var nent = packet.Entity as TestEntity2;


			Assert.AreEqual(Int64.MaxValue, nent.Int64);
			Assert.AreEqual(Int32.MaxValue, nent.Int32);
			Assert.AreEqual(Int16.MaxValue, nent.Int16);
			Assert.AreEqual(Double.MaxValue, nent.Double);
			Assert.AreEqual(Single.MaxValue, nent.Float);
			Assert.AreEqual("TEST", nent.String);
		*/}
	}
}
