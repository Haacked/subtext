using System;
using System.Web;
using Subtext.Framework;
using Subtext.Framework.Configuration;

namespace UnitTests.Subtext
{
	/// <summary>
	/// Summary description for UnitTestConfigProvider.
	/// </summary>
	public class UnitTestConfigProvider : UrlBasedBlogInfoProvider
	{
		static BlogInfo _config = new BlogInfo();

		/// <summary>
		/// Gets a dummy config object for the purpose of unit testing.
		/// </summary>
		/// <remarks>
		/// Will look for the configuration in the cache first using the 
		/// key "BlogConfig-".
		/// </remarks>
		/// <returns></returns>
		public override BlogInfo GetBlogInfo()
		{
			return _config;	
		}

	}
}
