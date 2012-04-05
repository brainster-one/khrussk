Khrussk is network engine for games based on .net technology

Installation
============
To install Khrussk, run the following command in the Package Manager Console
```
PM> Install-Package Khrussk
```

Usage
=====
Let's define our first entity.
```c#
class Player {
  public string X { get; set; }
  public string Y { get; set; }
}
```

Server side
```c#
var protocol = new SimpleRealmProtocol(new[] { typeof(Player) });
var service = new RealmService(protocol);
service.UserStateChanged += (s, a) => {
  var player = a.User["player"] ?? new Player();
  if (a.State == UserState.Connected) service.AddEntity(player);
  if (a.State == UserState.Disconnected) service.RemoveEntity(player);
};
service.Start(new IPEndPoint(IPAddress.Any, 9876));
```

Client side
```c#
var client = new RealmClient(protocol);
client.EntityStateChanged += (s, a) => {
  if (a.EntityInfo.Action == EntityNetworkAction.Added) _realm.AddEntity(a.EntityInfo.Entity);
  if (a.EntityInfo.Action == EntityNetworkAction.Removed) _realm.RemoveEntity(a.EntityInfo.Entity);
};
client.Connect(new IPEndPoint(IPAddress.Loopback, 9876));
```

Entity synchronization will be done automatically.