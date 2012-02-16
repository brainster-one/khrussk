
namespace Khrussk.Peers {
	using System.IO;

	/// <summary>Reads and writes packet data to stream.</summary>
	public interface IPacketSerializer {
		/// <summary>Writes packet to stream.</summary>
		/// <param name="writer">BinaryWriter to write data.</param>
		/// <param name="packet">Packet to write.</param>
		void Serialize(BinaryWriter writer, IPacket packet);

		/// <summary>Creates packet from stream.</summary>
		/// <param name="reader">BinaryReader to read data.</param>
		/// <returns>Created packet.</returns>
		IPacket Deserialize(BinaryReader reader);
	}
}
