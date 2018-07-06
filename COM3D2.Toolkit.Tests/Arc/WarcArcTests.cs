using Microsoft.VisualStudio.TestTools.UnitTesting;
using COM3D2.Toolkit.Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM3D2.Toolkit.Arc.Tests
{
	[TestClass()]
	public class WarcArcTests
	{
		[TestMethod()]
		public void WarcArcTest()
		{
			WarcArc arc = new WarcArc(@"M:\COM3D2\GameData\csv.arc");
		}
	}
}