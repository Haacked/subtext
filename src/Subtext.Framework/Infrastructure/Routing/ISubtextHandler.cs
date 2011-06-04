#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using Ninject;
using Subtext.Framework.Providers;

namespace Subtext.Framework.Routing
{
    public interface ISubtextDependencies
    {
        [Inject]
        ISubtextContext SubtextContext { get; }

        BlogUrlHelper Url { get; }

        ObjectRepository Repository { get; }

        AdminUrlHelper AdminUrl { get; }

        Blog Blog { get; }
    }
}