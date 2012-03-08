
namespace Khrussk.Realm {
	using System;

	/// <summary>User.</summary>
	public class User {
		/// <summary>Initializes a new instance of the User class using the specified session.</summary>
		public User(Guid session) {
			Session = session;
		}

		/// <summary>Gets session Id.</summary>
		public Guid Session { get; private set; }
	}
}
