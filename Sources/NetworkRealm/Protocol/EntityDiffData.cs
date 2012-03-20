
namespace Khrussk.NetworkRealm.Protocol {
	using System.IO;

	public class EntityDiffData {
		internal EntityDiffData(EntitySerializer serializer, BinaryReader reader) {
			_serializer = serializer;
			_reader = reader;
		}

		/// <summary>Apply changes to entity.</summary>
		/// <param name="entity">Entity to apply changes to.</param>
		public void ApplyChanges(object entity) {
			_serializer.Deserialize(_reader, ref entity);
		}

		private EntitySerializer _serializer;
		private BinaryReader _reader;
	}
}
