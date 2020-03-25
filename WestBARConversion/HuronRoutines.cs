using System;
using System.IO;
using System.Collections;

namespace Huron
{
	/// <summary>
	/// Summary description for HSMRoutines.
	/// </summary>
	public class HuronRoutines
    {
		/// <summary>
		/// Returns the date string.
		/// </summary>
		/// <param name="dt">Input date.</param>
		/// <param name="inputDtFormat">Input date format.</param>
		/// <param name="outputDtFormat">Output date format.</param>
		/// <returns></returns>
		private static string prior = "";
		private static Hashtable InsDict = new Hashtable();
		public static string FormatYearHNAC(string year)
		{
			string str = "";
            if (year == "00" || year == "01" || year == "02" || year == "03" || year == "04" || year == "05" || year == "06" || year == "07" || year == "08" || year == "09" || year == "10" || year == "11")
				str = "20" + year;
			else
				str = "19" + year;
			return str;
		}
        public static string FormatYearHNACBD(string year)
        {
            string str = "";
            if (year == "00" || year == "01" || year == "02" || year == "03" || year == "04" || year == "05" || year == "06" || year == "07" || year == "08" || year == "09")
                str = "20" + year;
            else
                str = "19" + year;
            return str;
        }
		public static string FormatDateHNAC(string dt)
		{
			string str = "";
			if(HuronRoutines.IsDate(dt))
			{
				DateTime dttime = DateTime.Parse(dt);
				string day = String.Format("{0:00}", dttime.Day);
				string month = String.Format("{0:00}", dttime.Month);
				string year = String.Format("{0:0000}", dttime.Year);
				str = year + month + day;
			}
			return str;
		}
		public static string FormatDate(string dt,Huron.HuronEnums.DateType inputDtFormat,Huron.HuronEnums.DateType outputDtFormat)
		{
			return DateRoutine(dt,inputDtFormat,outputDtFormat,"","");
		}
		/// <summary>
		/// Returns the date string.
		/// </summary>
		/// <param name="dt">Input date.</param>
		/// <param name="inputDtFormat">Input date format.</param>
		/// <param name="outputDtFormat">Output date format.</param>
		/// <param name="centIndicator">Century Indicator. Includes values 0,18,1,19,2,20.</param>
		/// <returns></returns>
		public static string FormatDate(string dt, Huron.HuronEnums.DateType inputDtFormat, Huron.HuronEnums.DateType outputDtFormat,string centIndicator)
		{
			return DateRoutine(dt,inputDtFormat,outputDtFormat,centIndicator,"");
		}
		public static string FormatDate(string dt, Huron.HuronEnums.DateType inputDtFormat,Huron.HuronEnums.DateType outputDtFormat,Huron.HuronEnums.ScalarDateType scalarDt)
		{
			string s = scalarDt.ToString().Substring(1);
			s = s.Substring(0,4) + "/" + s.Substring(4,2) + "/" + s.Substring(6);
			return DateRoutine(dt,inputDtFormat,outputDtFormat,"",s);
		}
		public static string FormatDate(string dt,Huron.HuronEnums.DateType inputDtFormat,Huron.HuronEnums.DateType outputDtFormat,DateTime scalarDt)
		{
			string s = scalarDt.Year+"/"+scalarDt.Month+"/"+scalarDt.Day;
			return DateRoutine(dt,inputDtFormat,outputDtFormat,"",s);
		}
		//Returna a date based on date,input, and output information.
		private static string DateRoutine(string dt,Huron.HuronEnums.DateType inputDateFormat,Huron.HuronEnums.DateType outputDateFormat,string ind,string scalarDt)
		{
			string day = "";
			string month = "";
			string year = "";
			string yr = "";
			string returnDt = "";
			double a = 0;
			DateTime dateTime;
			dt = dt.Trim();
			ind = ind.Trim();
			
			if(inputDateFormat.ToString() == "mmddyy")
			{
				dt = Right(dt,6);
				yr = Right(dt,2);
				if(ind == "")
				{
					if (yr == "00" || yr == "01" || yr == "02" || yr == "03" || yr == "04" || yr == "05")
						ind = "20";
				}	
			}
			if(inputDateFormat.ToString() == "yyyymmdd" || inputDateFormat.ToString() == "mmddyyyy")
				dt = Right(dt,8);
			if(dt == "" || dt == "//" || dt == "000000" || dt == "00000000" || dt == "00000") return "";
			if(ind == "00" || ind == "0" || ind == "18" || ind == "018")
				ind = "18";
			else if(ind == "01" || ind == "1" || ind == "19" || ind == "019" || ind == "")
				ind = "19";
			else if(ind == "02" || ind == "2" || ind == "20" || ind == "020")
				ind = "20";
			else
				ind = "";

			if(inputDateFormat.ToString() == "mmddyy" || inputDateFormat.ToString() == "mmddyyyy")
			{
				if(dt.Length == 6)
					dt = dt.Substring(0,2) + "/" + dt.Substring(2,2) +  "/" + ind + dt.Substring(4);
				else if(dt.Length > 6)
					dt = dt.Substring(0,2) + "/" + dt.Substring(2,2) +  "/" + dt.Substring(4);
			}
			else if(inputDateFormat.ToString() == "yyyymmdd")
			{
				if(dt.Length >= 8)
					dt = dt.Substring(0,4) + "/" + dt.Substring(4,2) +  "/" + dt.Substring(6);
			}
			else if(inputDateFormat.ToString() == "mmddyySlash" || inputDateFormat.ToString() == "mmddyyDash")
			{
				if(dt.Length == 8)
					dt = dt.Substring(0,6) + ind + dt.Substring(6,2);
			}
			else if(inputDateFormat.ToString() == "scalarDate")
			{
				a = double.Parse(dt);
				DateTime beginDt = DateTime.Parse(scalarDt);
				DateTime answer = beginDt.AddDays(a);
			
				day = string.Format("{0:00}",answer.Day);
				month = string.Format("{0:00}",answer.Month);
				year = answer.Year.ToString();
				dt = month + "/" + day + "/" + year;
			}

			if (IsDate(dt) == true)
			{
				dateTime = DateTime.Parse(dt);
				day = string.Format("{0:00}",dateTime.Day);
				month = string.Format("{0:00}",dateTime.Month);
				year = string.Format("{0:00}",dateTime.Year);
			}
			else
				return "";
			returnDt = OutputDate(outputDateFormat,month,day,year,scalarDt);
			return returnDt;
		}
		public static string FormatDate(string dt)
		{
			string newDt = "";
			string[] dtItems;
			if(dt.Trim() == "") return "";
			dtItems = dt.Split('/');
			if(dtItems.Length == 3)
			{
				string month = dtItems[0];
				string day = dtItems[1];
				string year = dtItems[2];
				if(year.Length == 2)
				{
					if (year == "00" || year == "01" || year == "02" || year == "03" || year == "04" || year == "05" || year == "06" || year == "07")
						year = "20" + year;
					else
						year = "19" + year;
					newDt = year + month + day;
				}
				else
					newDt = year + month + day;
			}
			return newDt;
		}
		public static string FormatDate_YYYYMMDD(string dt)
		{
			try
			{
				DateTime testDt = DateTime.Parse(dt);
				return testDt.Year.ToString() + testDt.Month.ToString().PadLeft(2,'0') + testDt.Day.ToString().PadLeft(2, '0');
			}
			catch
			{
				return "0";
			}
		}
		/// <summary>
		/// Determines if a file exists.
		/// </summary>
		/// <param name="fName">Full path and file name of file.</param>
		/// <returns></returns>
		public static bool FileExits(string fName)
		{
			FileInfo fInfo = new FileInfo(fName);
			if(fInfo.Exists == true)
				return true;
			else
				return false;
		}
		/// <summary>
		/// Determines if a string is a date.
		/// </summary>
		/// <param name="dt">Input date string.</param>
		/// <returns></returns>
		public static bool IsDate (string dt)
		{
			try
			{
				DateTime testDt = DateTime.Parse(dt);
				return true;
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// Returns a formatted procedure code.
		/// </summary>
		/// <param name="ProcCode">Procedure code.</param>
		/// <returns></returns>
		public static string FormatProcedure(string procCode)
		{
			string proc = "";
			procCode = procCode.Trim();
			if(procCode.Length > 2)
			{
				if(procCode.Length == 3)
					proc = procCode.Substring(1-1, 2) + "." + procCode.Substring(3-1, 1);
				else if(procCode.Length == 4)
					proc = procCode.Substring(1-1, 2) + "." + procCode.Substring(3-1, 2);
				else if(procCode.Length == 5)
					proc = procCode.Substring(1-1, 2) + "." + procCode.Substring(3-1, 3);
				else
					proc = procCode;
			}
			if(proc.Trim() == ".") proc = "";
			return proc;
		}
		public static string FormatZip(string zip)
		{
			zip = zip.Replace("-", "").Trim();
			if(zip.Trim() == "00000") return "";
			if(zip.Trim() == "0") return "";
			zip = zip.Trim();
			if(zip.Length > 5)
				zip = zip.Substring(0,5) + "-" + zip.Substring(5);
			if(zip.IndexOf("20202-") > -1) return "";
			if(zip.IndexOf("20-202-") > -1) return "";
			return zip;
		}
		public static string FormatNbr(string nbr, int length, char filler)
		{
			nbr = nbr.Replace("+","");
			nbr = nbr.Replace(",","");
			nbr = nbr.Replace("$","");
			nbr = nbr.Replace(".","");
			nbr = nbr.Trim();
			if(nbr.IndexOf("-") > -1)
			{
				nbr = nbr.Replace("-","");
				nbr = "-" + nbr.PadLeft(length-1,filler);
			}
			else
				nbr = nbr.PadLeft(length,filler);
			return nbr;
		}
		public static bool IsZeroBalance(string nbr)
		{
			nbr = nbr.Replace("+","");
			nbr = nbr.Replace(",","");
			nbr = nbr.Replace("$","");
			nbr = nbr.Replace(".","");
			nbr = nbr.Trim();
			if(IsNumeric(nbr) == true)
			{
				if(int.Parse(nbr) == 0)
					return true;
				else
					return false;
			}
			else
				return false;
		}
		public static string FormatNbrPositive(string nbr, int length, char filler)
		{
			nbr = nbr.Replace("+","");
			nbr = nbr.Replace(",","");
			nbr = nbr.Replace("$","");
			nbr = nbr.Replace(".","");
			nbr = nbr.Replace("-","");
			nbr = nbr.Trim();
			nbr = nbr.PadLeft(length,filler);
			return nbr;
		}
		public static string FormatPhone(string nbr)
		{
			nbr = nbr.Trim();
			if(nbr == "0") return "";
			if(nbr.Length >= 10)
				nbr = nbr.Substring(0,3) + "-" + nbr.Substring(3,3) + "-" + nbr.Substring(6);
			return nbr;
		}
		public static string FormatID(string nbr,int length,string prefix,char filler)
		{
			nbr = nbr.Trim();
			nbr = nbr.PadLeft(length,filler);
			nbr = prefix + nbr;
			return nbr;
		}
		/// <summary>
		/// Returns a formatted diagnosis code.
		/// </summary>
		/// <param name="dxCode">Diagnosis Code.</param>
		/// <returns></returns>
		public static string FormatDiagnosis(string dxCode)
		{
			string dx = "";
			if (dxCode.Trim().Length > 3)
			{
				if (dxCode.Length == 4)
					if (dxCode.Substring(1-1, 1) == "V")
						dx = dxCode.Substring(1-1, 3) + "." + dxCode.Substring(4-1, 1);
					else if (dxCode.Substring(1-1, 1) == "E")
						dx = dxCode;
					else
						dx = dxCode.Substring(1-1, 3) + "." + dxCode.Substring(4-1, 1);
					
				else if (dxCode.Length == 5)
					if (dxCode.Substring(1-1, 1) == "V")
						dx = dxCode.Substring(1-1, 3) + "." + dxCode.Substring(4-1, 2);
					else if (dxCode.Substring(1-1, 1) == "E")
						dx = dxCode.Substring(1-1, 4) + "." + dxCode.Substring(5-1, 1);
					else
						dx = dxCode.Substring(1-1, 3) + "." + dxCode.Substring(4-1, 2);
			}										
			else
				dx = dxCode;
			return dx;
		}
		/// <summary>
		/// Returns the number of characters from the right side of a string.
		/// </summary>
		/// <param name="r">Input string.</param>
		/// <param name="num">Number of characters to return from the right side of the input string.</param>
		/// <returns></returns>
		public static string Right(string r, int num)
		{
			if(num == 0) return "";
			r = r.Trim();
			int len = r.Length;
			if (len <= num)
				return r;
			else
			{
				r = r.Substring((r.Length - num));
				return r;
			}
		}
		
		/// <summary>
		/// Returns the value of a string with the overbyte character determined.
		/// </summary>
		/// <param name="num">Input value.</param>
		/// <returns></returns>
		public static string OverByteAmt(string num)
		{
			string chk = "";
			string amt = "";
			string test = "";
			bool neg = false;
		    
			test = num;
			chk = Right(num, 1);
			chk = chk.ToUpper();
			string letter = "A;B;C;D;E;F;G;H;I;{;J;K;L;M;N;O;P;Q;R;}";
			string number = "1;2;3;4;5;6;7;8;9;0;1;2;3;4;5;6;7;8;9;0";
			string nLetter = "J;K;L;M;N;O;P;Q;R;}";
			if(letter.IndexOf(chk) > -1)
			{
				if(nLetter.IndexOf(chk) > -1)
					neg = true;
				chk = number.Substring(letter.IndexOf(chk),1);
			}
			else
			{
				amt = decimal.Parse(test.Trim()).ToString();
				return amt;
			}
			if (neg == false)
				amt = decimal.Parse(test.Substring(1-1, test.Length - 1) + chk).ToString();
			else
				amt = decimal.Parse("-" + test.Substring(1-1, test.Length - 1) + chk).ToString();
			return amt;
		}
		/// <summary>
		/// Determines if a string is numeric.
		/// </summary>
		/// <param name="num">Input value.</param>
		/// <returns></returns>
		public static bool IsNumeric(string num)
		{
			if(num.Trim() == "") return false;
			for(int n = 0; n<=num.Length-1; n++)
			{
				if(Char.IsNumber(num,n) == false)
					return false;
			}
			return true;
		}
		public static bool IsNumericDouble(string num)
		{
			int y = 0;
			if(num.Trim() == "") return false;
			for(int n = 0; n<=num.Length-1; n++)
			{
				if(num.Substring(n,1) == ".")
					y++;
				else if(Char.IsNumber(num,n) == false)
					return false;
			}
			return true;
		}
		/// <summary>
		/// Replaces a value in a string based on a position.
		/// </summary>
		/// <param name="s">Input string.</param>
		/// <param name="pos">Postion where the value is to be replaced in the string.</param>
		/// <param name="replaceVal">Value that will replace the value at a certain position.</param>
		/// <returns></returns>
		public static string ReplaceValue(string s,int pos,string replaceVal)
		{
			string str = "";
			
			if(s.Trim() == "") return "";
			for(int n = 0; n<=s.Length-1; n++)
			{
				if(n == pos)
					str = str + replaceVal;
				else
					str = str + s.Substring(n,1);
			}
			return str;
		}
		
		public static string CheckInsurance(string ins,string mapIns,int flg,bool returnMapValue,string dupIns)
		{
			bool isDuplicate = false;
			bool is4 = false;
			string insurance = "";
			string insItem = "";
			string insNbr = "";
			string num = "";
			int nbr = 0;

			if(flg == 1)
				InsDict.Clear();

			if(ins.Trim() == "") 
				return "";
			else if(mapIns.Trim() == ins)
			{
				num = ins.Substring(0,3);
				if(HuronRoutines.IsNumeric(num) == true)
				{
					if(int.Parse(num) >= 100 && int.Parse(num) < 200)
					{
						dupIns = "co";
						insItem = "ZCONV.CO";
					}
					else if(int.Parse(num) >= 200 && int.Parse(num) < 300)
					{
						dupIns = "mr";
						insItem = "ZCONV.MR";
					}
					else if(int.Parse(num) >= 300 && int.Parse(num) < 400)
					{
						dupIns = "co";
						insItem = "ZCONV.CO";
					}
					else if(int.Parse(num) >= 400 && int.Parse(num) < 500)
					{
						dupIns = "bc";
						insItem = "ZCONV.BC";
					}
					else if(int.Parse(num) >= 500 && int.Parse(num) < 600)
					{
						dupIns = "bs";
						insItem = "ZCONV.BS";
					}
					else if(int.Parse(num) >= 600 && int.Parse(num) < 650)
					{
						dupIns = "hm";
						insItem = "ZCONV.HM";
					}
					else if(int.Parse(num) >= 650 && int.Parse(num) < 700)
					{
						dupIns = "co";
						insItem = "ZCONV.CO";
					}
					else if(int.Parse(num) >= 700 && int.Parse(num) < 750)
					{
						dupIns = "mc";
						insItem = "ZCONV.MC";
					}
					else if(int.Parse(num) >= 750 && int.Parse(num) < 800)
					{
						dupIns = "mc";
						insItem = "ZCONV.MC";
					}
					else if(int.Parse(num) >= 800 && int.Parse(num) < 900)
					{
						dupIns = "ch";
						insItem = "ZCOMV.CH";
					}
					else if(int.Parse(num) >= 900)
					{
						dupIns = "wc";
						insItem = "ZCONV.WC";
					}
					else
						insItem = ins;
					mapIns = insItem;
				}
				else
					insItem = ins;
			}
			else
				insItem = mapIns;

			if(InsDict.Contains(insItem) == true)
			{
				isDuplicate = true;
				insNbr = InsDict[insItem].ToString();
				InsDict.Remove(insItem);
				if(dupIns.Trim() == "")
				{
					nbr = int.Parse(insNbr) + 1;
					insurance = "OTH" + "." + nbr.ToString();
					InsDict.Add(insItem,nbr);
				}
				else
				{
					nbr = int.Parse(insNbr) + 1;
					insurance = dupIns + "." + nbr.ToString();
					InsDict.Add(insItem,nbr);
				}
				if(InsDict.Contains(dupIns) == true)
				{
					insNbr = InsDict[dupIns].ToString();
					nbr = int.Parse(insNbr) + 1;
					insurance = dupIns + "." + nbr.ToString();
					InsDict.Remove(dupIns);
					InsDict.Add(dupIns,nbr);
				}
				else
					InsDict.Add(dupIns,nbr);
			}
			if(isDuplicate == false)
			{	
				if(returnMapValue == true)
				{
					if(mapIns.Trim() == ins)
						insurance = ins;
					else
						insurance = mapIns;
				}
				else
					insurance = ins;

				if(mapIns.Trim() == ins)
				{
					if(is4 == true)
					{
						if(InsDict.Contains("ZCONV+") == false) 
							InsDict.Add("ZCONV+",1);
					}
					else
					{
						if(InsDict.Contains(ins) == false) 
							InsDict.Add(ins,1);
					}
				}
				else
				{
					if(InsDict.Contains(mapIns) == false) 
						InsDict.Add(mapIns,1);
				}
			}
			return insurance;
		}
		public static string CheckInsurance2(string ins,string mapIns,int flg,bool returnOrig,string dupIns)
		{
			string insurance = "";
			string outIns = "";
			int dup = 0;
			int a = 0;
			int t = 1;
			int num = 1;
			if(ins.Trim() == "") return "";
			if(flg == 1)
			{
				prior = " ";
				dup = 0;
			}
			if(mapIns == "")
			{
				if(int.Parse(ins.Trim()) >= 400001 && int.Parse(ins.Trim()) <= 400999)
				{
					if(prior.IndexOf(ins + "^") > -1)
					{
						while(num == 1)
						{
							a = prior.IndexOf(ins + "^",t);
							if(a == -1) break;
							dup++;
							t = a + 1;
						}
						dup++;
						insurance = ins + " " + dup.ToString().PadRight(1,'0');
						if(insurance == ins + " 2")
						{
							if(prior.IndexOf(ins + " 2") > -1)
							{
								dup = dup + 1;
								insurance = ins + "." + dup.ToString().PadRight(1,'0');
							}
						}
						if(insurance == ins + " 3")
						{
							if(prior.IndexOf(ins + " 3") > -1)
							{
								dup = dup + 2;
								insurance = ins + "." + dup.ToString().PadRight(1,'0');
							}
						}
						if(insurance == ins + " 4")
						{
							if(prior.IndexOf(ins + " 4") > -1)
							{
								dup = dup + 3;
								insurance = ins + "." + dup.ToString().PadRight(1,'0');
							}
						}
						prior = prior + insurance + "^";
						insurance = insurance.Replace(ins,"ZCONV");
					}
					else
					{
						if(returnOrig == true)
							insurance = mapIns;
						else
							insurance = ins;
							
						prior = prior + ins + "^";
					}
					return insurance;
				}
			}
			if(prior.IndexOf(ins + "^") > -1)
			{
				while(num == 1)
				{
					a = prior.IndexOf(ins + "^",t);
					if(a == -1) break;
					dup++;
					t = a + 1;
				}
				dup++;
				insurance = ins + " " + dup.ToString().PadRight(1,'0');
				if(insurance == ins + " 2")
				{
					if(prior.IndexOf(ins + " 2") > -1)
					{
						dup = dup + 1;
						insurance = ins + " " + dup.ToString().PadRight(1,'0');
						outIns = dupIns + "." + dup.ToString().PadRight(1,'0');
					}
				}
				if(insurance == ins + " 3")
				{
					if(prior.IndexOf(ins + " 3") > -1)
					{
						dup = dup + 2;
						insurance = ins + " " + dup.ToString().PadRight(1,'0');
						outIns = dupIns + "." + dup.ToString().PadRight(1,'0');
					}
				}
				if(insurance == ins + " 4")
				{
					if(prior.IndexOf(ins + " 4") > -1)
					{
						dup = dup + 3;
						insurance = ins + " " + dup.ToString().PadRight(1,'0');
						outIns = dupIns + "." + dup.ToString().PadRight(1,'0');
					}
				}
				prior = prior + insurance + "^";
			}
			else
			{
				if(returnOrig == true)
				{
					insurance = mapIns;
					outIns = mapIns;
				}
				else
				{
					insurance = ins;
					outIns = ins;
				}
				prior = prior + ins + "^";
			}
			return outIns;
		}
		public static string CheckInsurance3(string Ins, string MapIns, int flg, bool ReturnOrig) 					
		{
			int dup = 0;
			int a = 0;
			int t = 1;
			bool start = false;
			string chkInsurance = "";

			if (Ins.Trim() == "")  return "";
			if (flg == 1)
			{
				prior = " ";
				dup = 0;
			}
		    
			if (prior.IndexOf(Ins + "^",0) != -1)
			{
				while(start == false)
				{
					a = prior.IndexOf(Ins + "^",t);
					if (a == -1) break;
					dup++;
					t = a + 1;
				}
				chkInsurance = "OTH." + string.Format("{0:0}", dup);
				
				if (chkInsurance == "OTH.1")
					if (prior.IndexOf("OTH.1^",1) > 0)
						chkInsurance = "OTH." + string.Format("{0:0}", dup + 1);
				
				if (chkInsurance == "OTH.2")
					if (prior.IndexOf("OTH.2^",1) > 0)
						chkInsurance = "OTH." + string.Format("{0:0}", dup + 2);
				
				prior = prior + chkInsurance + "^";
			}
			else
			{
				if (ReturnOrig == true)
					chkInsurance = MapIns;
				else
					chkInsurance = Ins;
				prior = prior + Ins + "^";
			}
			
			return chkInsurance;
		}
		private static string OutputDate(Huron.HuronEnums.DateType output,string m, string d, string y,string sDt)
		{
			string outDt = "";
			string[] array;
			string abrMonth = Huron.HuronEnums.MonthAbbrs;
			string fullMonth = Huron.HuronEnums.MonthNames;
			const string defaultDate = "Dec 30, 1899";
			switch(output)
			{
				case Huron.HuronEnums.DateType.mmddyy:
					outDt = m+d+y.Substring(2);
					break;
				case Huron.HuronEnums.DateType.yyyymmdd:
					outDt = y+m+d;
					break;
				case Huron.HuronEnums.DateType.mmddyyyy:
					outDt = m+d+y;
					break;
				case Huron.HuronEnums.DateType.mmddyySlash:
					outDt = m+"/"+d+"/"+y.Substring(2);
					break;
				case Huron.HuronEnums.DateType.yyyymmddSlash:
					outDt = y+"/"+m+"/"+d;
					break;
				case Huron.HuronEnums.DateType.mmddyyyySlash:
					outDt = m+"/"+d+"/"+y;
					break;
				case Huron.HuronEnums.DateType.mmddyyDash:
					outDt = m+"-"+d+"-"+y.Substring(2);
					break;
				case Huron.HuronEnums.DateType.yyyymmddDash:
					outDt = y+"-"+m+"-"+d;
					break;
				case Huron.HuronEnums.DateType.mmddyyyyDash:
					outDt = m+"-"+d+"-"+y;
					break;
				case Huron.HuronEnums.DateType.abrMonDayYear:
					array = abrMonth.Split(',');
					outDt = array[int.Parse(m)-1] +" "+ int.Parse(d).ToString() + ", " + y;
					break;
				case Huron.HuronEnums.DateType.fullMonDayYear:
					array = fullMonth.Split(',');
					outDt = array[int.Parse(m)-1] +" "+ int.Parse(d).ToString() + ", " + y;
					break;
				case Huron.HuronEnums.DateType.scalarDate:
					if(sDt == "")
						sDt = defaultDate;
					DateTime scalarDt = DateTime.Parse(sDt);					
					DateTime dt = DateTime.Parse(m+"/"+d+"/"+y);
					TimeSpan num = dt.Subtract(scalarDt);
					outDt = int.Parse(num.TotalDays.ToString()).ToString();
					break;
			}
			return outDt;
		}
		public static string BGetSSN(string num)
		{
			if(num.Trim() == "") return "";
			num = num.Substring(0,3) + "-" + num.Substring(3,2) + "-" + num.Substring(5);
			if(num.Trim() == "000-00-0000" || num.Trim() == "999-99-9999")
				return "";
			else
				return num;
		}
		public static string GetCityState(string rec,string type)
		{
			char t = '\t';
			string text = "";
			string[] recs;
			string state = "";
			string city = "";
			string statenames = "," + Huron.HuronEnums.StateAbbrs + ",";
			rec = rec.TrimEnd().Replace(t,' ');
			rec = rec.Replace("   ",t.ToString());
			recs = rec.Split(t);
			if(statenames.IndexOf(rec.Trim()) > -1)
			{
				state = rec.Trim();
				city = "";
			}
			else if(recs.Length > 1)
			{
				state = recs[recs.GetUpperBound(0)].Trim().Replace(".","").Replace(",","");
				if(statenames.IndexOf(state) == -1)
					state = "";
				city = recs[0].Trim();
			}
			else
				city = rec.Trim();
			if(type == "STATE")
				return state;
			else
				return city;
		}
		public static string BGetSex(string sex)
		{
			if(sex.Trim() == "M" || sex.Trim() == "F" || sex.Trim() == "U")
				return sex.Trim();
			else
				return "U";
		}
		public static string FormatRace(string race)
		{
			if(race.Trim() == "W" || race.Trim() == "B" || race.Trim() == "A" || race.Trim() == "I" || race.Trim() == "H" || race.Trim() == "M" || race.Trim() == "O" || race.Trim() == "U")
				return race.Trim();
			else
				return "U";
		}
		public static string FormatMarSts(string m)
		{
			if(m.Trim() == "S" || m.Trim() == "M" || m.Trim() == "X" || m.Trim() == "D" || m.Trim() == "W" || m.Trim() == "P" || m.Trim() == "U")
				return m.Trim();
			else
				return "U";
		}
		public static string BGetName(string name,string nameType)
		{
			if(name.Trim() == "DECEASED")
				return "";
			if(nameType == "ALL")
			{
				if(name.Trim() == "")
					return "UNKNOWN,UNKNOWN";
				if(name.IndexOf("&",0) > -1)
					return "COMPANY," + name.Trim();
				if(name.Trim().IndexOf(" ",0) == -1)
					return "COMPANY," + name.Trim();
			}
			int num = name.IndexOf(" ",0);
			if(num > 0)
				name = name.Insert(num,",");
			name = name.Replace(", ",",");
			return name;
		}
		public static string FormatDT(string dt,string returnVal)
		{
            if (dt.Trim() == "")
                return "";
            else if (IsDate(dt) == true)
            {
                DateTime dateTime = DateTime.Parse(dt);
                if (returnVal == "date")
                {
                    string day = string.Format("{0:00}", dateTime.Day);
                    string month = string.Format("{0:00}", dateTime.Month);
                    string year = string.Format("{0:0000}", dateTime.Year);
                    return year + month + day;
                }
                else
                {
                    string time = dateTime.ToString("HHmm");
                    return time;
                }
            }
            else
                return "";
		}
		public static string BGetDate_2(string dt)
		{
			string newdt = "";
			if(dt.Trim() == "000000" || dt.Trim() == "00000000" || dt.Trim() == "0")
				newdt = "";
			else if(dt.Trim().Length == 6)
			{
				dt = dt.Substring(0,2) + "/" + dt.Substring(2,2) +  "/" + dt.Substring(4);
				if (IsDate(dt) == true)
				{
					DateTime dateTime = DateTime.Parse(dt);
					string day = string.Format("{0:00}",dateTime.Day);
					string month = string.Format("{0:00}",dateTime.Month);
					string year = string.Format("{0:0000}",dateTime.Year);
					newdt = year+month+day;
				}
				else
					newdt = "";
			}
			else
			{
				if(dt.Trim() == "0")
					newdt = "";
				else
					newdt = dt;
				
			}
			if(newdt == "20000000")
				newdt = "";
			return newdt;
		}
		public static string BGetDate_Check(string dt)
		{
			if(dt.Trim() == "000000" || dt.Trim() == "00000000" || dt.Trim() == "0")
				return "";
			else if(dt.Trim().Length == 8)
			{
				dt = dt.Substring(0,4) + "/" + dt.Substring(4,2) +  "/" + dt.Substring(6);
				if (IsDate(dt) == true)
				{
					DateTime dateTime = DateTime.Parse(dt);
					string day = string.Format("{0:00}",dateTime.Day);
					string month = string.Format("{0:00}",dateTime.Month);
					string year = string.Format("{0:0000}",dateTime.Year);
					return year+month+day;
				}
				else
					return "";
			}
			else
				return "";
		}
		public static bool CPSIDate_Check(string dt)
		{
			if(dt.Trim() == "000000" || dt.Trim() == "00000000" || dt.Trim() == "0")
				return false;
			else if(dt.Trim().Length == 8)
			{
				dt = dt.Substring(0,4) + "/" + dt.Substring(4,2) +  "/" + dt.Substring(6);
				if (IsDate(dt) == true)
					return true;
				else
					return false;
			}
			else
				return false;
		}
		
		public static string MGetDate(string dt)
		{
            if (IsDate(dt) == false)
                return "";
            else
            {
                DateTime dateTime = DateTime.Parse(dt);
                string day = string.Format("{0:00}", dateTime.Day);
                string month = string.Format("{0:00}", dateTime.Month);
                string year = string.Format("{0:0000}", dateTime.Year);
                return year + month + day;
            }
		}
		public static string BGetMRN(string mrn)
		{
			mrn = mrn.Trim();
			mrn = "M" + mrn.PadLeft(9,'0').Substring(0,9);
			return mrn;
		}
		public static string BGetMRNBAR(string mrn)
		{
			if(mrn.Trim() == "NEED MRN NUMBER") return "";
			mrn = mrn.Trim();
			mrn = "M" + mrn.PadLeft(9,'0').Substring(0,9);
			return mrn;
		}
		public static string FormatSex(string sex)
		{
            sex = sex.ToUpper();
            if (sex == "363")
				return "M";
            else if (sex == "362")
                return "F";
			else
				return "U";
		}
		public static string SSN(string num)
		{
			if(num.Trim() == "") return "000-00-0000";
			if(num.Trim() == "_________") return "000-00-0000";
			num = num.Substring(0,3) + "-" + num.Substring(3,2) + "-" + num.Substring(5);
			return num;
		}
		public static string FormatName3(string name)
		{
			bool first = true;
			int n = 0;
			string newName = "";
			string convertName = "";
			name = name.Trim();
			for (n = 0; n <= name.Length - 1; n++)
			{
				if (Char.IsNumber(name, n) == false)
					newName = newName + name.Substring(n,1);
			}
			newName = newName.Trim();
			for (n = 0; n <= newName.Length - 1; n++)
			{
				if (newName.Substring(n, 1) == " " && first == true)
				{
					convertName = convertName + ",";
					first = false;
				}
				else
					convertName = convertName + newName.Substring(n, 1);
			}
			return convertName;
		}
		public static string FormatName(string name)
		{
			string convertName = "";
			name = name.Trim().ToUpper();
			convertName = name.Replace(", ",",") + "#";
            convertName = convertName.Replace(",#", "");
            convertName = convertName.Replace("#", "").Replace("JR.", "JR");
			return convertName;
		}
        public static string Format4CharTime(string dt)
        {
            string time = "";
            string timeOut = "";
            string[] timeItems;
            string[] dtTime;
            if (dt.IndexOf(":", 0) > -1)
            {
                dt = dt.Trim();
                dtTime = dt.Split(' ');
                if (dtTime.Length > 1)
                {
                    time = dtTime[1].Trim();
                    timeItems = time.Split(':');
                    timeOut = timeItems[0].ToString().PadLeft(2, '0') + timeItems[1].ToString().PadLeft(2, '0');
                }
            }
            if (timeOut == "")
                timeOut = "2359";
            return timeOut;
        }
	}
}
