using System;

namespace Huron
{
	/// <summary>
	/// Summary description for HSMEnums.
	/// </summary>
	public class HuronEnums
    {
		/// <summary>
		/// Returns a string of State Names.
		/// </summary>
		public static string StateNames = "ALABAMA,ALASKA,ARIZONA,ARKANSAS,CALIFORNIA,COLORADO,CONNECTICUT,DELAWARE,DISTRICT OF COLUMBIA," +
													"FLORIDA,GEORGIA,HAWAII,IDAHO,ILLINOIS,INDIANA,IOWA,KANSAS,KENTUCKY,LOUISANA,MAINE," +
													"MARYLAND,MASSACHUSETTS,MICHIGAN,MINNESOTA,MISSISSIPPI,MISSOURI,MONTANA,NEBRASKA," +
													"NEVADA,NEW HAMPSHIRE,NEW JERSEY,NEW MEXICO,NEW YORK,NORTH CAROLINA,NORTH DAKOTA," +
													"OHIO,OKLAHOMA,OREGON,PENNSYLVANIA,RHODE ISLAND,SOUTH CAROLINA,SOUTH DAKOTA,TENNESSEE," +
													"TEXAS,UTAH,VERMONT,VIRGINIA,WASHINGTON,WEST VIRGINIA,WISCONSIN,WYOMING";
		/// <summary>
		/// Returns a string of State Abbreviations.
		/// </summary>
		public static string StateAbbrs = "AL,AK,AZ,AR,CA,CO,CT,DE,DC,FL,GA,HI,ID,IL,IN,IO,KS,KY,LA,ME,MD," +
											 "MA,MI,MN,MS,MO,MT,NE,NV,NH,NJ,NM,NY,NC,ND,OH,OK,OR,PA,RI,SC," +
											 "SD,TN,TX,UT,VT,VA,WA,WV,WI,WY";
		/// <summary>
		/// Returns a string of Canadian Provinces.
		/// </summary>
		public static string CanadaProvinces = "ALBERTA,BRITISH COLUMBIA,MANITOBA,NEW BRUNSWICK,NEWFOUNDLAND,NOVA SCOTIA,ONTARIO," +
													"PRINCE EDWARD ISLAND,QUEBEC,SASKATCHEWAN,YUKON,N.W. TERRITORY,NANAVUT";
		/// <summary>
		/// Returns a string of Name suffixes.
		/// </summary>
		public static string Suffix = "JR,JR.,SR,SR.,I,II,III,IV,V,VI,VII,VIII,I,II.,III.,IV.,V.,VI.,VII.,VIII.,MD,MD.";
		/// <summary>
		/// Returns a string of Name prefixes.
		/// </summary>
		public static string Prefix = "DR.,DR,MR,MR.,MRS,MRS.,MISS,MISS.,REV,REV.";
		/// <summary>
		/// Returns a string of abbreviated months.
		/// </summary>
		public static string MonthAbbrs = "JAN,FEB,MAR,APR,MAY,JUN,JUL,AUG,SEP,OCT,NOV,DEC";
		/// <summary>
		/// Returns a string of month names.
		/// </summary>
		public static string MonthNames = "JANUARY,FEBRUARY,MARCH,APRIL,MAY,JUNE,JULY,AUGUST,SEPTEMBER,OCTOBER,NOVEMBER,DECEMBER";
		/// <summary>
		/// Enumeration of Time types.
		/// </summary>
		public enum TimeType
		{
			/// <summary>
			/// Standard time.
			/// ex.) 12:43 AM
			/// </summary>
			Standard = 0,
			/// <summary>
			/// Military time based on the number of hours in a day.
			/// ex.) 0130
			/// </summary>
			Military = 1
		}
		/// <summary>
		/// Enumeration of Name types.
		/// </summary>
		public enum NameType
		{
			/// <summary>
			/// LastName, FirstName MiddleInit format
			/// </summary>
			LastNameFirstName = 0,
			/// <summary>
			/// FirstName MiddleInit LastName format
			/// </summary>
			FirstNameLastName = 1
		}
		/// <summary>
		/// Enumeration of date types.
		/// </summary>
		public enum DateType
		{
			/// <summary>
			/// 2-Digit Month 2-Digit Day 2-Digit Year 
			/// ex.)010299
			/// </summary>
			mmddyy = 0,
			/// <summary>
			/// 4-Digit Year 2-Digit Month 2-Digit Day 
			/// ex.)19990217
			/// </summary>
			yyyymmdd = 1,
			/// <summary>
			/// 2-Digit Month 2-Digit Day 4-Digit Year 
			/// ex.)07281997
			/// </summary>
			mmddyyyy = 2,
			/// <summary>
			/// 2-Digit Month slash 2-Digit Day slash 2-Digit Year 
			/// ex.)01/02/99
			/// </summary>
			mmddyySlash = 3,
			/// <summary>
			/// 4-Digit Year slash 2-Digit Month slash 2-Digit Day 
			/// ex.)1999/02/17
			/// </summary>
			yyyymmddSlash = 4,
			/// <summary>
			/// 2-Digit Month slash 2-Digit Day slash 4-Digit Year
			/// ex.)01/02/1999
			/// </summary>
			mmddyyyySlash = 5,
			/// <summary>
			/// 2-Digit Month dash 2-Digit Day dash 2-Digit Year 
			/// ex.)01-02-99
			/// </summary>
			mmddyyDash = 6,
			/// <summary>
			/// 4-Digit Year dash 2-Digit Month dash 2-Digit Day 
			/// ex.)2001-07-22 
			/// </summary>
			yyyymmddDash = 7,
			/// <summary>
			/// 2-Digit Month dash 2-Digit Day dash 4-Digit Year
			/// ex.)01-02-1999
			/// </summary>
			mmddyyyyDash = 8,
			/// <summary>
			/// 3-Letter Abbreviated Month Day 4-Digit Year
			/// ex.)Jan 1, 2003
			/// </summary>
			abrMonDayYear = 9,
			/// <summary>
			/// Full-Letter Month Day 4-Digit Year
			/// ex.)January 1, 2003
			/// </summary>
			fullMonDayYear = 10,
			/// <summary>
			/// Number of days from a given date
			/// ex.)34247
			/// </summary>
			scalarDate = 11
		}
		public enum FieldType
		{
			String,
			Numeric,
			AlphaNumeric,
			Date,
			Time,
			CanadaProvince,
			SSN,
			PhoneNbr,
			AdmitDate,
			DischargeDate,
			PhoneNumber,
			OverByte,
			CityStateZip,
			CityState,
			State,
			Zip
		}
		/// <summary>
		/// Date at which the scale begins.
		/// </summary>
		public enum ScalarDateType
		{
			/// <summary>
			/// Saint scale date: Dec 30, 1899
			/// </summary>
			x18991230 = 1
		}
		public enum FileType
		{
			Fixed,
			Variable,
			Delimited,
			Byte
		}
		/// <summary>
		/// Describes the type of file being opened.
		/// </summary>
		public enum FileMode
		{
			/// <summary>
			/// Input file.
			/// </summary>
			Input,
			/// <summary>
			/// Output file.
			/// </summary>
			Output
		}
		public enum InputFormat
		{
			/// <summary>
			/// 2-Digit Month 2-Digit Day 2-Digit Year 
			/// ex.)010299
			/// </summary>
			DATE_mmddyy,
			/// <summary>
			/// 4-Digit Year 2-Digit Month 2-Digit Day 
			/// ex.)19990217
			/// </summary>
			DATE_yyyymmdd,
			/// <summary>
			/// 2-Digit Month 2-Digit Day 4-Digit Year 
			/// ex.)07281997
			/// </summary>
			DATE_mmddyyyy,
			/// <summary>
			/// 2-Digit Month slash 2-Digit Day slash 2-Digit Year 
			/// ex.)01/02/99
			/// </summary>
			DATE_mmddyySlash,
			/// <summary>
			/// 4-Digit Year slash 2-Digit Month slash 2-Digit Day 
			/// ex.)1999/02/17
			/// </summary>
			DATE_yyyymmddSlash,
			/// <summary>
			/// 2-Digit Month slash 2-Digit Day slash 4-Digit Year
			/// ex.)01/02/1999
			/// </summary>
			DATE_mmddyyyySlash,
			/// <summary>
			/// 2-Digit Month dash 2-Digit Day dash 2-Digit Year 
			/// ex.)01-02-99
			/// </summary>
			DATE_mmddyyDash,
			/// <summary>
			/// 4-Digit Year dash 2-Digit Month dash 2-Digit Day 
			/// ex.)2001-07-22 
			/// </summary>
			DATE_yyyymmddDash,
			/// <summary>
			/// 2-Digit Month dash 2-Digit Day dash 4-Digit Year
			/// ex.)01-02-1999
			/// </summary>
			DATE_mmddyyyyDash,
			/// <summary>
			/// 3-Letter Abbreviated Month Day 4-Digit Year
			/// ex.)Jan 1, 2003
			/// </summary>
			DATE_abrMonDayYear,
			/// <summary>
			/// Full-Letter Month Day 4-Digit Year
			/// ex.)January 1, 2003
			/// </summary>
			DATE_fullMonDayYear,
			/// <summary>
			/// Saint scale date: Dec 30, 1899
			/// </summary>
			DATE_18991230,
			/// <summary>
			/// Standard time.
			/// ex.) 12:43 AM
			/// </summary>
			TIME_Standard,
			/// <summary>
			/// Military time based on the number of hours in a day.
			/// ex.) 0130
			/// </summary>
			TIME_Military,
			/// <summary>
			/// LastName, FirstName MiddleInit format
			/// </summary>
			NAME_LastNameFirstName,
			/// <summary>
			/// FirstName MiddleInit LastName format
			/// </summary>
			NAME_FirstNameLastName
		}
	}
}
