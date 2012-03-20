
namespace Khrussk.NetworkRealm {
	using System;
	using System.Collections.Generic;

	/// <summary>User.</summary>
	public class User {
		/// <summary>Initializes a new instance of the User class using the specified session.</summary>
		public User(Guid session) {
			Session = session;
		}

		/// <summary>Gets or sets related to user objects.</summary>
		/// <param name="index">Index.</param>
		/// <returns>Object.</returns>
		public object this[string index] {
			get { return _objects[index]; }
			set { _objects[index] = value; }
		}

		/// <summary>Returns string representation of object.</summary>
		/// <returns>String</returns>
		public override string ToString() {
			return Session.ToString();
		}

		/// <summary>Gets session Id.</summary>
		public Guid Session { get; private set; }

		/// <summary>Related to user objects.</summary>
		private Dictionary<string, object> _objects = new Dictionary<string, object>();
	}
}
