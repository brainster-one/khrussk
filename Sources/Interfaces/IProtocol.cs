
namespace Khrussk.Peers {
	using System.IO;

	/// <summary>Protocol interface.</summary>
	public interface IProtocol {
		/// <summary>Creates packet from stream.</summary>
		/// <param name="stream">Stream to read data from.</param>
		/// <returns>Created packet.</returns>
		object Read(Stream stream);

		/// <summary>Writes packet to stream.</summary>
		/// <param name="stream">Stream to write data to.</param>
		/// <param name="packet">Packet to write.</param>
		void Write(Stream stream, object packet);
	}
}
