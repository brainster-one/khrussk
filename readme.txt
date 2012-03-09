Khrussk is network engine for games based on .net technology

Realm layer
===========

1. Start service and client (TODO: pass protocol - this make step 2 unnecessary)
var service = new RealmService();
cvar client = new RealmClient();

2. Register entities
service.Protocol.RegisterEntityType(typeof(TestEntity), new TestEntitySerializer());
client.Protocol.RegisterEntityType(typeof(TestEntity), new TestEntitySerializer());

3. Subscribe on events
client.EntityAdded += OnEntityAddedHandler;

4. Establish connection
client.Connect(new IPEndPoint(IPAddress.Loopback, 9876));

5. Publish yours first entity
service.AddEntity(new TestEntity());

6. Gets it on another side in OnEntityAddedHandler