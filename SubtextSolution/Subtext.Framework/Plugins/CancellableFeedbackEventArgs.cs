#region Disclaimer/Info
///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at SourceForge at http://sourceforge.net/projects/subtext
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using Subtext.Framework.Components;

namespace Subtext.Extensibility.Plugins
{
	public class CancellableFeedbackEventArgs: FeedbackEventArgs
	{

		private bool _cancel;

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}

		public CancellableFeedbackEventArgs(FeedbackItem feedback, ObjectState state) : base(feedback, state) { }

		public CancellableFeedbackEventArgs(FeedbackItem feedback) : this(feedback, ObjectState.None) { }

	}
}
