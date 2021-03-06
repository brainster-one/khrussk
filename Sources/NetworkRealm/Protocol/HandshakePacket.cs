﻿
namespace Khrussk.NetworkRealm.Protocol {
	using System;
	using System.IO;
	using Peers; 

	/// <summary>Handshake packet.</summary>
	public sealed class HandshakePacket {
		/// <summary>Initializes a new instance of the HandshakePacket class.</summary>
		/// <param name="session">Session.</param>
		public HandshakePacket(Guid session) {
			Session = session;
		}

		/// <summary>Gets or sets entity.</summary>
		public Guid Session { get; private set; }
	}

	/// <summary>HandshakePacket serializer.</summary>
	sealed class HandshakePacketSerializer : IPacketSerializer<HandshakePacket> {
		/// <summary>Deserializes packet from stream.</summary>
		/// <param name="reader">Reader to deserialize packet by.</param>
		/// <returns>Packet.</returns>
		public HandshakePacket Deserialize(BinaryReader reader) {
			return new HandshakePacket(Guid.Parse(reader.ReadString()));
		}

		/// <summary>Serializes packet to stream.</summary>
		/// <param name="writer">Writer to serialize packet.</param>
		/// <param name="packet">Packet to write.</param>
		/// <returns>Packet.</returns>
		public void Serialize(BinaryWriter writer, HandshakePacket packet) {
			writer.Write(packet.Session.ToString());
		}
	}
}
