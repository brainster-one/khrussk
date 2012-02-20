
namespace Khrussk.Peers {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	
	/// <summary>Protocol packet serializer info.</summary>
	internal sealed class ProtocolPacketSerializerInfo {
		/// <summary>Gets or sets packet type id.</summary>
		public int PacketTypeId { get; set; }

		/// <summary>Gets or sets packet type.</summary>
		public Type PacketType { get; set; }

		/// <summary>Gets or sets packet streamer.</summary>
		public IPacketSerializer Serializer { get; set; }
	}

	// TODO Может течь память на создании BinaryReader/BinaryWriter
	/// <summary>Common protocol.
	/// [PacketType:byte][PacketSize:word][PacketData:byte[]]
	/// </summary>
	public class Protocol : IProtocol {
		/// <summary>Registres packet type serializer.</summary>
		/// <param name="packetType">Type of packet.</param>
		/// <param name="packetSerializer">Serializer.</param>
		public void RegisterPacketType(Type packetType, IPacketSerializer packetSerializer) {
			_serializers.Add(new ProtocolPacketSerializerInfo {
				PacketType = packetType,
				PacketTypeId = _serializers.Count(),
				Serializer = packetSerializer
			});
		}

		/// <summary>Creates packet from stream.</summary>
		/// <param name="stream">Stream to read data from.</param>
		/// <returns>Created packet.</returns>
		public IPacket Read(Stream stream) {
			if (stream == null) throw new ArgumentNullException("stream");
			if (!stream.CanRead) throw new ArgumentException("Can't read from closed stream", "stream");

			// Read data from stream
			var reader = new BinaryReader(stream);
			var packetTypeId = reader.ReadByte();
			var packetSize = reader.ReadUInt16();
			var buffer = reader.ReadBytes(packetSize);
			var packetStream = new MemoryStream(buffer, false);
			if (packetSize != buffer.Length) throw new IOException("Partial data read");

			// Call serializer
			var sinfo = _serializers.FirstOrDefault(x => x.PacketTypeId == packetTypeId);
			if (sinfo == null) { throw new InvalidOperationException("No serializer registered for this packet type: " + packetTypeId); }
			var packet = sinfo.Serializer.Deserialize(new BinaryReader(packetStream));

			// Return packet
			return packet;
		}

		/// <summary>Writes packet to stream.</summary>
		/// <param name="stream">Stream to write data to.</param>
		/// <param name="packet">Packet to write.</param>
		public void Write(Stream stream, IPacket packet) {
			if (stream == null) throw new ArgumentNullException("stream");
			if (packet == null) throw new ArgumentNullException("packet");
			if (!stream.CanWrite) throw new ArgumentException("Can't write to stream", "stream");

			// Find handler
			var sinfo = _serializers.FirstOrDefault(x => x.PacketType.Equals(packet.GetType()));
			if (sinfo == null) { throw new InvalidOperationException("No serializer registered for this packet type: " + packet.GetType()); }

			// Write packet's data to temp stream
			var tempStream = new MemoryStream();
			sinfo.Serializer.Serialize(new BinaryWriter(tempStream), packet);

			// Write packet's type id, size and data to stream
			var writer = new BinaryWriter(stream);
			writer.Write((byte)sinfo.PacketTypeId);
			writer.Write((UInt16)tempStream.Length);
			writer.Write(tempStream.ToArray(), 0, (int)tempStream.Length);

			// Close temp stream
			tempStream.Close();
		}

		/// <summary>Packet handlers.</summary>
		readonly List<ProtocolPacketSerializerInfo> _serializers = new List<ProtocolPacketSerializerInfo>();
	}
}
