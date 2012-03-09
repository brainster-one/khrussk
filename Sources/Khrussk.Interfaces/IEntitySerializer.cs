
namespace Khrussk.NetworkRealm.Protocol {
	using System.IO;

	/// <summary>Entity serializer interface.</summary>
	public interface IEntitySerializer {
		/// <summary>Serializes entity to stream.</summary>
		/// <param name="writer">Writer to serialize entity by.</param>
		/// <param name="entity">Entity to serialize.</param>
		void Serialize(BinaryWriter writer, IEntity entity);

		/// <summary>Deserializes entity from stream.</summary>
		/// <param name="reader">Reader to deserialize entity by.</param>
		/// <param name="entity">Entity.</param>
		void Deserialize(BinaryReader reader, ref IEntity entity);
	}
}
