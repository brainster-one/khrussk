﻿
namespace Khrussk.Network {
	using System;
	using Khrussk.Tests;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Khrussk.Realm;
	using Khrussk.Peers;
	using Khrussk.Realm.Protocol;
using System.Threading;
	using System.Net;

	/// <summary>
	///This is a test class for ListenerSocketTests and is intended
	///to contain all ListenerSocketTests Unit Tests
	///</summary>
	[TestClass()] public class RealmTests {
		public ManualResetEvent evnt = new ManualResetEvent(false);

		/// <summary>Initialize.</summary>
		[TestInitialize] public void Initialize() {
		}

		/// <summary>Cleanup.</summary>
		[TestCleanup] public void Cleanup() {
		}

		/// <summary>Connection between ListenerSocket and ClientSocket should be established.</summary>
		[TestMethod()] public void ConnectionShouldBeEstablishedTest() {
			RealmService service = new RealmService();
			service.Start();

			RealmClient client = new RealmClient();
			client.Connected += new EventHandler<RealmServiceEventArgs>(client_Connected);
			client.Connect(new IPEndPoint(IPAddress.Loopback, 9876));

			Assert.IsTrue(evnt.WaitOne(TimeSpan.FromSeconds(3)));
		}

		void client_Connected(object sender, RealmServiceEventArgs e) {
			evnt.Set();
		}
	}
}
