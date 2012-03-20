
namespace Khrussk.NetworkRealm.Helpers {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>Entity to Id map.</summary>
	class EntityIdMap {
		/// <summary>Initializes a new instance of the EntityIdMap class.</summary>
		public EntityIdMap() {
			for (int i = 1; i < Int16.MaxValue; ++i) {
				_ids.Enqueue(i);
			}
		}

		/// <summary>Adds entity to map.</summary>
		/// <param name="entity">Entity to add.</param>
		/// <returns>Added entity id.</returns>
		public int Add(object entity) {
			if (entity == null) throw new ArgumentNullException("entity", "null can not be stored in map");
			if (_map.ContainsKey(entity)) throw new InvalidOperationException(string.Format("Entity '{0}' already stored in map with key '{1}'", entity, _map[entity]));
			if (!_ids.Any()) throw new InvalidOperationException("Maximum number of objects reached");

			var id = _ids.Dequeue();
			_map.Add(entity, id);

			return id;
		}

		/// <summary>Removes entity from map.</summary>
		/// <param name="entity">Entity to remove.</param>
		/// <returns>Removed entity id.</returns>
		public int Remove(object entity) {
			if (entity == null) throw new ArgumentNullException("entity", "null can not be removed from map");
			if (!_map.ContainsKey(entity)) throw new InvalidOperationException(string.Format("Entity '{0}' is not stored in map", entity));

			var map = _map.First(x => x.Key == entity);
			_map.Remove(map.Key);
			_ids.Enqueue(map.Value);

			return map.Value;
		}

		/// <summary>Returns entity Id.</summary>
		/// <param name="entity">Entity to get id for.</param>
		/// <returns>Id.</returns>
		public int GetId(object entity) {
			if (entity == null) throw new ArgumentNullException("entity", "null is not stored in map");
			if (!_map.ContainsKey(entity)) throw new InvalidOperationException(string.Format("Entity '{0}' is not stored in map", entity));

			return _map[entity];
		}

		/// <summary>Gets list of stored entities.</summary>
		public IEnumerable<object> Entities {
			get { return _map.Keys.ToList().AsReadOnly(); }
		}

		/// <summary>Dictionary stores object-id map.</summary>
		private Dictionary<object, int> _map = new Dictionary<object, int>();

		/// <summary>Entity ids pool.</summary>
		private Queue<int> _ids = new Queue<int>();
	}
}
