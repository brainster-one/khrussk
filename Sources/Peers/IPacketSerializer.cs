
namespace Khrussk.Peers {
	using System.IO;

	/// <summary>Reads and writes packet data to stream.</summary>
	public interface IPacketSerializer<T> {
		/// <summary>Writes packet to stream.</summary>
		/// <param name="writer">BinaryWriter to write data.</param>
		/// <param name="packet">Packet to write.</param>
		void Serialize(BinaryWriter writer, T packet);

		/// <summary>Creates packet from stream.</summary>
		/// <param name="reader">BinaryReader to read data.</param>
		/// <returns>Created packet.</returns>
		T Deserialize(BinaryReader reader);
	}
}
