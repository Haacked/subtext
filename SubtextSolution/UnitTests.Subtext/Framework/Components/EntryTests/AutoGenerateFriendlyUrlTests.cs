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
using MbUnit.Framework;
using Subtext.Extensibility;
using Subtext.Framework;
using Subtext.Framework.Components;
using Subtext.Framework.Configuration;

namespace UnitTests.Subtext.Framework.Components.EntryTests
{
	/// <summary>
	/// Tests the feature to auto generate the EntryName property 
	/// of an entry. This serves as a friendly url.
	/// </summary>
	[TestFixture]
	[Author("Robb Allen", "robb.allen@gmail.com", "http://blog.robballen.com")]
	public class AutoGenerateFriendlyUrlTests
	{
		string _hostName = string.Empty;

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FriendlyUrlThrowsArgumentException()
		{
			Entries.AutoGenerateFriendlyUrl(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FriendlyUrlWithSeparatorAndNullTitleThrowsArgumentException()
		{
			Entries.AutoGenerateFriendlyUrl(null, '_');
		}
	
		/// <summary>
		/// Make sure words are separated and limited correctly
		/// using 5 word limit and underscore
		/// </summary>
		
		[RowTest]
		[Row("Single", "Single")]
		[Row("Single ", "Single")]
		[Row("Two words", "Two_words")]
		[Row("Holymolythisisalongwordthatnormallywouldn'tbeused.", "Holymolythisisalongwordthatnormallywouldntbeused")]
		[Row("This is a very long title that will be truncated.", "This_is_a_very_long")]
		[RollBack]
		public void FriendlyUrlLimitedDelimited(string title, string expected)
		{
			Assert.IsTrue(Config.CreateBlog("", "username", "password", _hostName, string.Empty));
			Assert.AreEqual(expected, Entries.AutoGenerateFriendlyUrl(title), "The auto generated entry name is not what we expected.");
		}

		/// <summary>
		/// Makes sure we are generating nice friendly URLs.
		/// </summary>
		[RowTest]
		[Row("Title", "Title")]
		[Row("Title.", "Title")]
		[Row("A Very Good Book", "AVeryGoodBook")]
		[Row("a very good book", "AVeryGoodBook")]
		[Row("A Very :Good Book", "AVeryGoodBook")]
		[Row("A Very ;Good Book", "AVeryGoodBook")]
		[Row("A Very Good Book.", "AVeryGoodBook")]
		[Row("A Very Good Book..", "AVeryGoodBook")]
		[Row("A Very Good..Book", "AVeryGood.Book")]
		[Row("A Very Good...Book", "AVeryGood.Book")]
		[Row("� V�ry G��d B��k..", "%c3%85V%c3%aaryGdBk")]
		[Row("\u0130\u0069Turkish Character Test", "%c4%b0iTurkishCharacterTest")]
		[Row("Trouble With VS.NET", "TroubleWithVS.NET")]
		[Row("Bar�a is a nice town", "Bar%c3%a7aIsANiceTown")]
		[Row("Perch� Pi� felicit� pu� ed �?", "Perch%c3%a8Pi%c3%b9Felicit%c3%a0Pu%c3%b2Ed%c3%89")]
		[Row(@"[!""'`;:~@#$%^&*(){\[}\]?+/=\\|<> X", "X")]
		[RollBack]
		public void FriendlyUrlGeneratesNiceUrl(string title, string expected)
		{
			Assert.IsTrue(Config.CreateBlog("", "username", "password", _hostName, string.Empty));
			Assert.AreEqual(expected, Entries.AutoGenerateFriendlyUrl(title, char.MinValue), "The auto generated entry name is not what we expected.");
		}

		/// <summary>
		/// Makes sure we are generating nice friendly URLs Using Underscores.
		/// </summary>
		[RowTest]
		[Row("Title", "Title")]
		[Row("Title.", "Title")]
		[Row("A Very Good Book Yo", "A_Very_Good_Book_Yo")]
		[Row("a very good book yo", "a_very_good_book_yo")]
		[Row("A Very ::Good Book", "A_Very_Good_Book")]
		[Row("A Very ;;Good Book", "A_Very_Good_Book")]
		[Row("A Very Good Book yo.", "A_Very_Good_Book_yo")]
		[Row("A Very Good Book yo..", "A_Very_Good_Book_yo")]
		[Row("� V�ry Good Book yo..", "%c3%85_V%c3%aary_Good_Book_yo")]
		[Row("Trouble With VS.NET Yo", "Trouble_With_VS.NET_Yo")]
		[Row("Bar�a is a nice town", "Bar%c3%a7a_is_a_nice_town")]
		[Row("Perch� Pi� felicit� pu� ed �?", "Perch%c3%a8_Pi%c3%b9_felicit%c3%a0_pu%c3%b2_ed_%c3%a9")]
		[Row(@"[!""'`;:~@#$%^&*(){\[}\]?+/=\\|<> Y", "Y")]
		[Row(@"[!""'`;:~@#$%^&*(){\[}\]?+/=\\|<>YY", "YY")]
		[RollBack]
		public void FriendlyUrlGeneratesNiceUrlWithUnderscores(string title, string expected)
		{
			Assert.IsTrue(Config.CreateBlog("", "username", "password", _hostName, string.Empty));
			Assert.AreEqual(expected, Entries.AutoGenerateFriendlyUrl(title, '_'), "THe auto generated entry name is not what we expected.");
		}
		
		/// <summary>
		/// Makes sure we are generating nice friendly URLs Using Periods.
		/// </summary>
		[RowTest]
		[Row("Title.", "Title")]
		[Row("Contains.PeriodAlready", "Contains.PeriodAlready")]
		[Row("A Very Good Book yo..", "A.Very.Good.Book.yo")]
		[RollBack]
		public void FriendlyUrlGeneratesNiceUrlWithPeriods(string title, string expected)
		{
			Assert.IsTrue(Config.CreateBlog("", "username", "password", _hostName, string.Empty));
			Assert.AreEqual(expected, Entries.AutoGenerateFriendlyUrl(title, '.'), "THe auto generated entry name is not what we expected.");
		}

		[RowTest]
		[Row("_", "One_Two")]
		[Row(" ", "OneTwo")]
		[Row("X", "OneXTwo")]
		[Row("$", "OneTwo")]
		[RollBack]
		public void FriendlyUrlHandlesBadSeparators(string wordSeparatorString, string expected)
		{
			Assert.IsTrue(Config.CreateBlog("", "username", "password", _hostName, string.Empty));

			string title = "One Two";
			char wordSeparator = wordSeparatorString[0];
			Assert.AreEqual(expected, Entries.AutoGenerateFriendlyUrl(title, wordSeparator), "THe auto generated entry name is not what we expected.");
		}

		/// <summary>
		/// Make sure that generated friendly urls are unique.
		/// </summary>
		[Test]
		[RollBack]
		public void FriendlyUrlIsUnique()
		{
			Assert.IsTrue(Config.CreateBlog("", "username", "password", _hostName, string.Empty));

			Config.CurrentBlog.AutoFriendlyUrlEnabled = true;
			Entry entry = new Entry(PostType.BlogPost);
			entry.DateCreated = DateTime.Now;
			entry.SourceUrl = "http://localhost/ThisUrl/";
			entry.Title = "Some Title";
			entry.Body = "Some Body";
			int id = Entries.Create(entry);

            Entry savedEntry = Entries.GetEntry(id, PostConfig.None, false);
			Assert.AreEqual("Some_Title", savedEntry.EntryName, "The EntryName should have been auto-friendlied.");
			Assert.AreEqual(savedEntry.Url, savedEntry.TitleUrl, "The title url should link to the entry.");

			Entry duplicate = new Entry(PostType.BlogPost);
			duplicate.DateCreated = DateTime.Now;
			duplicate.SourceUrl = "http://localhost/ThisUrl/";
			duplicate.Title = "Some Title";
			duplicate.Body = "Some Body";
			int dupeId = Entries.Create(duplicate);
            Entry savedDupe = Entries.GetEntry(dupeId, PostConfig.None, false);
			
			Assert.AreEqual("Some_TitleAgain", savedDupe.EntryName, "Should have appended 'Again'");
			UnitTestHelper.AssertAreNotEqual(savedEntry.EntryName, savedDupe.EntryName, "No duplicate entry names are allowed.");

			Entry yetAnotherDuplicate = new Entry(PostType.BlogPost);
			yetAnotherDuplicate.DateCreated = DateTime.Now;
			yetAnotherDuplicate.SourceUrl = "http://localhost/ThisUrl/";
			yetAnotherDuplicate.Title = "Some Title";
			yetAnotherDuplicate.Body = "Some Body";
			dupeId = Entries.Create(yetAnotherDuplicate);
            savedDupe = Entries.GetEntry(dupeId, PostConfig.None, false);
			
			Assert.AreEqual("Some_TitleYetAgain", savedDupe.EntryName, "Should have appended 'YetAgain'");

			yetAnotherDuplicate = new Entry(PostType.BlogPost);
			yetAnotherDuplicate.DateCreated = DateTime.Now;
			yetAnotherDuplicate.SourceUrl = "http://localhost/ThisUrl/";
			yetAnotherDuplicate.Title = "Some Title";
			yetAnotherDuplicate.Body = "Some Body";
			dupeId = Entries.Create(yetAnotherDuplicate);
            savedDupe = Entries.GetEntry(dupeId, PostConfig.None, false);
			
			Assert.AreEqual("Some_TitleAndAgain", savedDupe.EntryName, "Should have appended 'AndAgain'");

			yetAnotherDuplicate = new Entry(PostType.BlogPost);
			yetAnotherDuplicate.DateCreated = DateTime.Now;
			yetAnotherDuplicate.SourceUrl = "http://localhost/ThisUrl/";
			yetAnotherDuplicate.Title = "Some Title";
			yetAnotherDuplicate.Body = "Some Body";
			dupeId = Entries.Create(yetAnotherDuplicate);
            savedDupe = Entries.GetEntry(dupeId, PostConfig.None, false);
			
			Assert.AreEqual("Some_TitleOnceMore", savedDupe.EntryName, "Should have appended 'OnceMore'");

			yetAnotherDuplicate = new Entry(PostType.BlogPost);
			yetAnotherDuplicate.DateCreated = DateTime.Now;
			yetAnotherDuplicate.SourceUrl = "http://localhost/ThisUrl/";
			yetAnotherDuplicate.Title = "Some Title";
			yetAnotherDuplicate.Body = "Some Body";
			dupeId = Entries.Create(yetAnotherDuplicate);
            savedDupe = Entries.GetEntry(dupeId, PostConfig.None, false);
			
			Assert.AreEqual("Some_TitleToBeatADeadHorse", savedDupe.EntryName, "Should have appended 'ToBeatADeadHorse'");
		}

		/// <summary>
		/// Sets the up test fixture.  This is called once for 
		/// this test fixture before all the tests run.  It 
		/// essentially copies the App.config file to the 
		/// run directory.
		/// </summary>
		[TestFixtureSetUp]
		public void SetUpTestFixture()
		{
			//Confirm app settings
            UnitTestHelper.AssertAppSettings();
		}

		[SetUp]
		public void SetUp()
		{
			_hostName = Guid.NewGuid().ToString().Replace("-", "") + ".com";
			UnitTestHelper.SetHttpContextWithBlogRequest(_hostName, "");
		}

		[TearDown]
		public void TearDown()
		{
			Config.ConfigurationProvider = null;
		}
	}
}
