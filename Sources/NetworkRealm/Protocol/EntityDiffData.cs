
namespace Khrussk.NetworkRealm.Protocol {
	using System.IO;

	class EntityDiffData {
		internal EntityDiffData(EntitySerializer serializer, BinaryReader reader) {
			_serializer = serializer;
			_reader = reader;
		}

		/// <summary>Apply changes to entity.</summary>
		/// <param name="entity">Entity to apply changes to.</param>
		public void ApplyChanges(object entity) {
			_serializer.Deserialize(_reader, ref entity);
		}

		readonly EntitySerializer _serializer;
		readonly BinaryReader _reader;
	}
}
