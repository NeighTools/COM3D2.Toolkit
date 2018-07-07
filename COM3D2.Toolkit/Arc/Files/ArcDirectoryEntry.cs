using System.Collections.Generic;

namespace COM3D2.Toolkit.Arc.Files
{
	public class ArcDirectoryEntry : ArcEntry
	{
		internal static ArcDirectoryEntry Read(WarcArc arc, ArcEntry parent = null, IEnumerable<ArcEntry> children = null)
		{
			ArcDirectoryEntry dir = new ArcDirectoryEntry
			{
				Archive = arc,
				Parent = parent,
				Children = children
			};

			return dir;
		}
	}
}
