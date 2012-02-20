using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Khrussk.Realm.Protocol {
	public class EntityDiffData {
		internal EntityDiffData(IEntitySerializer serializer, BinaryReader reader) {
			_serializer = serializer;
			_reader = reader;
		}

		/// <summary>Apply changes to entity.</summary>
		/// <param name="entity">Entity to apply changes to.</param>
		public void ApplyChanges(IEntity entity) {
			_serializer.Deserialize(_reader, ref entity);
		}

		private IEntitySerializer _serializer;
		private BinaryReader _reader;
	}
}
