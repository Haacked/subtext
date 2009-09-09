#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Subtext.Framework.Exceptions
{
    [Serializable]
    public class DeprecatedPhysicalPathsException : Exception
    {
        readonly string message;

        public DeprecatedPhysicalPathsException(ReadOnlyCollection<string> physicalPaths)
        {
            InvalidPhysicalPaths = physicalPaths;
            message = "In order to complete the upgrade, please delete the following directories/files." +
                      Environment.NewLine;
            foreach(string path in physicalPaths)
            {
                message += " " + path + Environment.NewLine;
            }
        }

        public DeprecatedPhysicalPathsException(IEnumerable<string> physicalPaths)
            : this(new ReadOnlyCollection<string>(physicalPaths.ToList()))
        {
        }

        public override string Message
        {
            get { return message; }
        }

        public ReadOnlyCollection<string> InvalidPhysicalPaths { get; private set; }
    }
}