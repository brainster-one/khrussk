
namespace Khrussk.Tests.UnitTests.Realm.Shared {
	using NetworkRealm.Protocol;

	class TestProtocol : RealmProtocol {
		public TestProtocol() {
			Register(new TestEntitySerializer());
		}
	}
}
