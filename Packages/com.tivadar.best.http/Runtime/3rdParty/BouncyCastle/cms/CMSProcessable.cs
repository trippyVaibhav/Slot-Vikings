#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

namespace Best.HTTP.SecureProtocol.Org.BouncyCastle.Cms
{
	public interface CmsProcessable
	{
		/// <summary>
		/// Generic routine to copy out the data we want processed.
		/// </summary>
		/// <remarks>
		/// This routine may be called multiple times.
		/// </remarks>
		void Write(Stream outStream);
	}
}
#pragma warning restore
#endif
