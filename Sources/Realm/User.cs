
namespace Khrussk.Realm {
	using System;

	/// <summary>User.</summary>
	public class User {
		/// <summary>Initializes a new instance of the User class using the specified session.</summary>
		public User(Guid session) {
			Session = session;
		}

		/// <summary>Returns string representation of object.</summary>
		/// <returns>String</returns>
		public override string ToString() {
			return Session.ToString();
		}

		/// <summary>Gets session Id.</summary>
		public Guid Session { get; private set; }
	}
}
