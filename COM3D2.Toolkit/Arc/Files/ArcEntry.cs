using System.Collections.Generic;
using COM3D2.Toolkit.Native;

namespace COM3D2.Toolkit.Arc.Files
{
	public abstract class ArcEntry
	{
		public WarcArc Archive { get; protected set; }

		public ulong UTF16Hash { get; protected set; }

		public ulong UTF8Hash { get; protected set; }
		
		protected string _name;

		public virtual string Name
		{
			get => _name;
			set
			{
				_name = value;
				string preName = _name.ToLower();
				UTF8Hash = DataHasher.BaseHasher.GetHashUTF8(preName);
				UTF16Hash = DataHasher.BaseHasher.GetHashUTF16(preName);
			}
		}

		public virtual string FullName => $"{Parent?.FullName}\\{Name}";

		public ArcEntry Parent { get; protected set; }

		public IEnumerable<ArcEntry> Children { get; set; }
	}
}
