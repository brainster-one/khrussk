using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khrussk.Extras.PolicyService;

namespace Khrussk.Extras.SilverlightPolicyService.Host {
	class Program {
		static void Main(string[] args) {
			Console.Write("Starting...\n"); 
			PolicyServer ps = new PolicyServer(@"clientaccesspolicy.xml");
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
		}
	}
}
