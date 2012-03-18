
namespace Khrussk.Tests.Realm.Shared {
	using Khrussk.NetworkRealm.Protocol;

	class TestProtocol : RealmProtocol {
		public TestProtocol() {
			RegisterEntityType(typeof(TestEntity), new TestEntitySerializer());
		}
	}
}
