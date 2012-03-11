
namespace Khrussk.NetworkRealm.Protocol {
	using System.IO;

	public class EntityDiffData {
		internal EntityDiffData(IEntitySerializer serializer, BinaryReader reader) {
			_serializer = serializer;
			_reader = reader;
		}

		/// <summary>Apply changes to entity.</summary>
		/// <param name="entity">Entity to apply changes to.</param>
		public void ApplyChanges(object entity) {
			_serializer.Deserialize(_reader, ref entity);
		}

		private IEntitySerializer _serializer;
		private BinaryReader _reader;
	}
}
