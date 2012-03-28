
namespace Khrussk.Peers {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	// TODO Может течь память на создании BinaryReader/BinaryWriter
	/// <summary>Common protocol.
	/// [PacketSize:word][PacketType:byte][PacketData:byte[]]
	/// </summary>
	public class Protocol : IProtocol {
		/// <summary>Registres packet type serializer.</summary>
		/// <param name="packetSerializer">Serializer.</param>
		public void Register<T>(IPacketSerializer<T> packetSerializer) {
			_factory.Register(packetSerializer);
			_idToType[++_curPacketId] = typeof(T);
		}

		/// <summary>Creates packet from stream.</summary>
		/// <param name="stream">Stream to read data from.</param>
		/// <returns>Created packet.</returns>
		public object Read(Stream stream) {
			if (stream == null) throw new ArgumentNullException("stream");
			if (!stream.CanRead) throw new ArgumentException("Can't read from closed stream", "stream");

			// No data to read.
			if (stream.Length - stream.Position == 0) return null;

			// Creates and stores binary reader for specified stream
			var reader = _readers.ContainsKey(stream) ? _readers[stream] : new BinaryReader(stream);
			_readers[stream] = reader;

			// Reads packet size
			var packetSize = reader.ReadUInt16();
			var isFullPacketPresent = (stream.Length - stream.Position - packetSize) >= 0;
			if (!isFullPacketPresent) {
				stream.Position -= sizeof(UInt16);
				return null;
			}

			// Reads packet type and data
			var packetTypeId = reader.ReadByte();
			var buffer = reader.ReadBytes(packetSize);
			var packetStream = new MemoryStream(buffer, false);
			if (packetSize != buffer.Length)
				throw new IOException(String.Format("Partial data read. Expected {0}, read {1}", packetSize, buffer.Length));

			// Call serializer
			return typeof(Protocol)
				.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod(_idToType[packetTypeId])
				.Invoke(this, new object[] { new BinaryReader(packetStream), _idToType[packetTypeId] });
		}

		/// <summary>Writes packet to stream.</summary>
		/// <param name="stream">Stream to write data to.</param>
		/// <param name="packet">Packet to write.</param>
		public void Write(Stream stream, object packet) {
			if (stream == null) throw new ArgumentNullException("stream");
			if (packet == null) throw new ArgumentNullException("packet");
			if (!stream.CanWrite) throw new ArgumentException("Can't write to stream", "stream");

			// Write packet's data to temp stream
			var packetTypeId = _idToType.First(x => x.Value == packet.GetType()).Key;
			var tempStream = new MemoryStream();
			var method = typeof(Protocol)
				.GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod(packet.GetType())
				.Invoke(this, new[] { new BinaryWriter(tempStream), packet });
			
			// Write packet's type id, size and data to stream
			var writer = new BinaryWriter(stream);
			writer.Write((UInt16)tempStream.Length);
			writer.Write((byte)packetTypeId);
			writer.Write(tempStream.ToArray(), 0, (int)tempStream.Length);

			// Close temp stream
			tempStream.Close();
		}

		private void Serialize<T>(BinaryWriter writer, object packet) {
			var serializer = _factory.Get<T>();
			if (serializer == null) throw new InvalidOperationException("No serializer registered for " + packet.GetType());
			serializer.Serialize(writer, (T)packet);
		}

		private T Deserialize<T>(BinaryReader reader, Type type) {
			var serializer = _factory.Get<T>();
			if (serializer == null) throw new InvalidOperationException("No serializer registered for " + type);
			return serializer.Deserialize(reader);
		}

		/// <summary>Packet handlers.</summary>
		readonly PacketSerializerFactory _factory = new PacketSerializerFactory();
		readonly Dictionary<int, Type> _idToType = new Dictionary<int, Type>();
		readonly Dictionary<Stream, BinaryReader> _readers = new Dictionary<Stream, BinaryReader>();
		private int _curPacketId;
	}
}
