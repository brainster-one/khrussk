
namespace Khrussk.NetworkRealm.Helpers {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Peers;

	/// <summary>Stores peeruser/peer map.</summary>
	class UserPeerMap {
		/// <summary>Maps user to peer.</summary>
		/// <param name="user">User to map.</param>
		/// <param name="peer">Peer to map.</param>
		public void Map(User user, Peer peer) {
			if (user == null) throw new ArgumentNullException("user", "User is null");
			if (peer == null) throw new ArgumentNullException("peer", "Peer is null");
			if (_map.ContainsKey(user)) throw new InvalidOperationException(string.Format("User '{0}' already mapped", user));
			if (_map.ContainsValue(peer)) throw new InvalidOperationException(string.Format("Peer '{0}' already mapped", peer));

			_map.Add(user, peer);
		}

		/// <summary>Returns user's peer.</summary>
		/// <param name="user">User.</param>
		/// <returns>Peer.</returns>
		public Peer GetPeer(User user) {
			if (user == null) throw new ArgumentNullException("user");
			if (!_map.ContainsKey(user)) throw new InvalidOperationException(string.Format("User '{0}' is not stored.", user));

			return _map[user];
		}
		
		/// <summary>Returns user by peer.</summary>
		/// <param name="peer">Peer to find user.</param>
		/// <returns>User.</returns>
		public User GetUser(Peer peer) {
			if (peer == null) throw new ArgumentNullException("peer");
			if (!_map.ContainsValue(peer)) throw new InvalidOperationException(string.Format("Peer '{0}' is not stored.", peer));

			return _map.First(x => x.Value == peer).Key;
		}

		public IEnumerable<object> Peers {
			get { return _map.Values; }
		}

		/// <summary>Dictionary stores object-id map.</summary>
		readonly Dictionary<User, Peer> _map = new Dictionary<User, Peer>();
	}
}
