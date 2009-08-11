using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework;
using Subtext.Framework.Util;

namespace UnitTests.Subtext.Framework.Util
{
	[TestFixture]
	public class TimeZonesTest
	{
        public const int PacificTimeZoneId = -667686747;
        const int NewZealandZoneId = 742740495;
        const int CentralEuropeZoneId = -460310079;
        public const int HawaiiTimeZoneId = 2091460006;

		[Test]
		public void GenerateUpdateScript()
		{
			string sql = string.Empty;
			string sqlFormat = "UPDATE [<dbUser,varchar,dbo>].[subtext_Config] SET TimeZone = {0} WHERE TimeZone = {1}" + Environment.NewLine + "GO" + Environment.NewLine;
			foreach(WindowsTimeZone timezone in WindowsTimeZone.TimeZones)
			{
				sql += String.Format(sqlFormat, timezone.Id, timezone.BaseBias.TotalHours);
			}
			Console.Write(sql);
		}
		
		[Test, Ignore("Only run this when we need to regen this file. Better to make this a build step.")]
		public void WriteTimeZonesToFile()
		{
			Type tzcType = typeof(WindowsTimeZoneCollection);
			XmlSerializer ser = new XmlSerializer(tzcType);
			using (StreamWriter writer = new StreamWriter("c:\\WindowsTimeZoneCollection.xml", false, Encoding.UTF8)) 
				ser.Serialize(writer, WindowsTimeZone.TimeZones);
		}
		
		[RowTest]
		[Row(PacificTimeZoneId, "Pacific Standard Time", "Pacific Standard Time", "Pacific Daylight Time", "(GMT-08:00) Pacific Time (US & Canada)", -480)]
		[Row(0, "", "", "", "", 0, ExpectedException = typeof(NullReferenceException))]
		public void CanGetById(int id, string standardName, string standardZoneName, string daylightZoneName, string displayName, int bias)
		{
			WindowsTimeZone timeZone = WindowsTimeZone.GetById(id);
			Assert.AreEqual(standardZoneName, timeZone.StandardZoneName);
			Assert.AreEqual(standardName, timeZone.StandardName);
			Assert.AreEqual(daylightZoneName, timeZone.DaylightZoneName);
			Assert.AreEqual(displayName, timeZone.DisplayName);
			Assert.AreEqual(bias, timeZone.BaseBias.TotalMinutes);
		}
		
		[Test]
		public void CanEnumerateTimezones()
		{
			Assert.IsTrue(WindowsTimeZone.TimeZones.Count > 10, "Expected more than ten.");
			
			foreach(WindowsTimeZone timeZone in WindowsTimeZone.TimeZones)
			{
                Console.WriteLine(timeZone.Id + " " + timeZone.ZoneIndex + "\t" + timeZone.DisplayName + "\t" +timeZone.StandardName + "\t" + timeZone.GetUtcOffset(DateTime.Now));
			}

			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId);
			
			DaylightTime daylightTime = pst.GetDaylightChanges(2006);
			
			daylightTime = TimeZone.CurrentTimeZone.GetDaylightChanges(2006);
		}
		
		[Test]
		public void SimplePstConversionTest()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST
			Console.WriteLine(pst.Now);

			Console.WriteLine(TimeZone.CurrentTimeZone.ToLocalTime(DateTime.UtcNow));
		}
		
		[Test]
		public void ParseUsingAssumingUniversalReturnsDateTimeKindUtc()
		{
		  IFormatProvider culture = new CultureInfo("en-US", true);
		  DateTime utcDate = DateTime.Parse("10/01/2006 19:30", culture, DateTimeStyles.AssumeUniversal);
		  Assert.AreEqual("10/01/2006 19:30", utcDate.ToUniversalTime().ToString("MM/dd/yyyy HH:mm", culture));
		}

        [Test]
        public void CanConvertBeetweenTimeZones()
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime nzdtDate = DateTime.Parse("12/30/2006 19:30", culture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal);

            WindowsTimeZone nzdt = WindowsTimeZone.TimeZones.GetById(NewZealandZoneId); //NZ
            WindowsTimeZone cet = WindowsTimeZone.TimeZones.GetById(CentralEuropeZoneId); //CET

            DateTime cetDate = cet.ToLocalTime(nzdt.ToUniversalTime(nzdtDate));

            string formattedPstDate = cetDate.ToString("MM/dd/yyyy HH:mm", culture);
            Assert.AreEqual("12/30/2006 07:30", formattedPstDate);
        }

		[Test]
		public void ToLocalTimeReturnsProperTime()
		{
			IFormatProvider culture = new CultureInfo("en-US", true);
			DateTime utcDate = DateTime.Parse("12/30/2006 19:30", culture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);
			utcDate = utcDate.ToLocalTime().ToUniversalTime();
			Assert.AreEqual(DateTimeKind.Utc, utcDate.Kind);
			Assert.AreEqual("12/30/2006 19:30", utcDate.ToString("MM/dd/yyyy HH:mm", culture), "An assumption about round tripping the UTC date was wrong.");

			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST

			DateTime pstDate = pst.ToLocalTime(utcDate);
			Assert.AreEqual(DateTimeKind.Local, pstDate.Kind, "Expected to be local now.");

			string formattedPstDate = pstDate.ToString("MM/dd/yyyy HH:mm", culture);
			Assert.AreEqual("12/30/2006 11:30", formattedPstDate);
		}
		
		[Test]
		public void ToLocalTimeReturnsProperTimeDuringDaylightSavings()
		{
			IFormatProvider culture = new CultureInfo("en-US", true);
			DateTime utcDate = DateTime.Parse("10/01/2006 19:30", culture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal);
			utcDate = utcDate.ToLocalTime().ToUniversalTime();
			Assert.AreEqual(DateTimeKind.Utc, utcDate.Kind);
			Assert.AreEqual("10/01/2006 19:30", utcDate.ToString("MM/dd/yyyy HH:mm", culture), "An assumption about round tripping the UTC date was wrong.");

			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST

			DateTime pstDate = pst.ToLocalTime(utcDate);
			Assert.AreEqual(DateTimeKind.Local, pstDate.Kind, "Expected to be local now.");
			
			string formattedPstDate = pstDate.ToString("MM/dd/yyyy HH:mm", culture);
			Assert.AreEqual("10/01/2006 12:30", formattedPstDate);
		}
		
		[Test]
		public void ToUniversalTimeReturnsProperTime()
		{
			IFormatProvider culture = new CultureInfo("en-US", true);
			DateTime localDate = DateTime.Parse("10/01/2006 12:30", culture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal);

			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST

			DateTime utcDate = pst.ToUniversalTime(localDate);
			
			string formattedPstDate = utcDate.ToString("MM/dd/yyyy HH:mm", culture);
			Assert.AreEqual("10/01/2006 19:30", formattedPstDate);
		}

		/// <summary>
		/// Make sure that ToUniversalTime returns a datetime that is an utc time and 
		/// not a local time.
		/// </summary>
		[Test]
		public void ToUniversalTimeReturnsDateTimeKindUtc()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST
			DateTime now = DateTime.Now;
			Assert.AreEqual(DateTimeKind.Local, now.Kind);
			DateTime utcDate = pst.ToUniversalTime(now);
			Assert.AreEqual(DateTimeKind.Utc, utcDate.Kind, "Expected a UTC time.");
		}

		[Test]
		public void GetDaylightChangesReturnsProperDaylightSavingsInfo()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST
			DaylightTime daylightChanges = pst.GetDaylightChanges(2007);
            DateTime start = DateTime.ParseExact("3/11/2007 2:00:00 AM", "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact("11/4/2007 2:00:00 AM", "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
			Assert.AreEqual(start, daylightChanges.Start);
			Assert.AreEqual(end, daylightChanges.End);
		}
		
		[Test]
		public void GetZoneByIndexReturnsProperPSTInfo()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST
			Assert.AreEqual("Pacific Daylight Time", pst.DaylightName);
			Assert.AreEqual("Pacific Daylight Time", pst.DaylightZoneName);
			Assert.AreEqual(new TimeSpan(1, 0, 0), pst.DaylightBias, "Expected a one hour bias");

		}
		
		/// <summary>
		/// Make sure that ToLocalTime returns a datetime that is a local time and 
		/// not a UTC time.
		/// </summary>
		[Test]
		public void ToLocalTimeReturnsDateTimeKindLocal()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetById(PacificTimeZoneId); //PST
			DateTime utcNow = DateTime.UtcNow;
			Assert.AreEqual(DateTimeKind.Utc, utcNow.Kind);
			DateTime local = pst.ToLocalTime(utcNow);
			Assert.AreEqual(DateTimeKind.Local, local.Kind);
		}
	}
}
