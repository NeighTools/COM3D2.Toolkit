using System;

namespace COM3D2.Toolkit
{
	public class ArcException : Exception
	{
		public ArcException() : base()
		{

		}

		public ArcException(string message) : base(message)
		{

		}
		
		public ArcException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
