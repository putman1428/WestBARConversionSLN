using System;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.Text;


namespace WestBARConversion
{
	public class DataIO
	{
		private FileStream fileStream;
		private StreamReader reader;
		private StreamWriter writer;
		private FileInfo fileInfo;
		private static SqlConnection conn;
		private string [] recArray;
		private int Counter = 0;
		public string RecordData = "";
		private int NumberOfBytes = 0;
		byte[] recBytes;
		private string returnValues = "";
		private static OleDbConnection connA;
		private static OleDbDataReader AccessReader;
		private Hashtable InMemoryIndexes = new Hashtable(); 
		public enum IO
		{
			INPUT,
			OUTPUT
		}
		public enum SearchType
		{
			PRIMARY,
			ALTERNATE
		}

		public bool OpenFile(string fileName, IO accessType)
		{
			if (accessType == IO.INPUT) 
			{
				fileInfo = new FileInfo(fileName);
				fileStream = fileInfo.OpenRead();
				reader = new StreamReader (fileStream);
			}
			else if (accessType == IO.OUTPUT)
			{
				fileInfo = new FileInfo(fileName);
				fileInfo.Delete();
				fileStream = fileInfo.OpenWrite();
				writer = new StreamWriter (fileStream);
			}
			return true;
		}
		public bool CloseFile()
		{
            try
            {
                fileStream.Close();
                return true;
            }
            catch { return false; }
		}
		public static bool FileExits(string fName)
		{
			FileInfo fInfo = new FileInfo(fName);
			if(fInfo.Exists == true)
				return true;
			else
				return false;
		}
		
		public void SetRecordLength(int length)
		{
			recBytes = new byte[length];
			NumberOfBytes = length;
		}
		public long GetFileLength()
		{
			return fileInfo.Length;
		}
        public static void OpenSQLDatabase(string serverName, string databaseName)
        {
            string source = "server=SHCDSDBP01;integrated security=SSPI;persist security info=False;packet size=4096;database=" + databaseName;
            conn = new SqlConnection(source);
            conn.Open();
            //string source = @"Server=JP01\JP00;Database=" + databaseName + ";Integrated Security=True";
            ////string source = "server=" + serverName + ";uid=sa;pwd=putmjap;database=" + databaseName;
            //conn = new SqlConnection(source);
            //conn.Open();
        }
		public static void OpenAccessDatabase(string databaseName,string table,string orderBy)
		{
			string source = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + databaseName;
			connA = new OleDbConnection(source);
			connA.Open();
			string select = "Select * from " + table + " order by " + orderBy;
			OleDbCommand cmd = new OleDbCommand(select,connA);
			AccessReader = cmd.ExecuteReader();	
		}
		public static void CloseSQLDatabase()
		{
			conn.Close();
		}
		public static void CloseAccessDatabase()
		{
			connA.Close();
		}
        public void Index(WestBARConversion.BARForm frm, string database, string sqlTable, string primVal, string altVal, string idx, string remoteServer, string remotePath)
		{
			string primary = "";
			string secondary = "";
			string rec = "";
			string[] recs;
			string[] indxs;
			string outRec = "";
			long position = 0;
			long recNum = 0;
			bool eof = false;
            char tab = '\t';

			FileStream fs;
			StreamWriter sw;
			if (fileInfo.Exists == true)
			{
				string text = fileInfo.FullName.ToString();
				int num = text.IndexOf(".",0);
				if(num > -1)
					text = text.Substring(0,num);
				else
					num = text.Length;
				FileInfo f = new FileInfo(text + ".dat");
				if (f.Exists == false)
				{
					fs = f.OpenWrite();
					sw = new StreamWriter(fs);
					DateTime sDate = DateTime.Now;
					frm.DisplayResults(sDate.ToLongTimeString());
					while(eof == false)
					{
						rec = reader.ReadLine();
						if (rec == "" || rec == null)
							break;
						else if (char.Parse(rec.Substring(1-1,1)) == (char) 26)
							break;
						else
						{
							if(idx == "")
							{
								if (altVal.Trim() == "")
									primary = GetValueInFile(primVal,rec);
								else
								{
									primary = GetValueInFile(primVal,rec);
									secondary = GetValueInFile(altVal,rec);
								}
								primary = primary.Trim();
								secondary = secondary.Trim();
								outRec = position.ToString() + "%" + primary + "%" + secondary;
								sw.WriteLine(outRec);
								sw.Flush();
								primary = "";
								secondary = "";
								recNum++;
								if (recNum % 100 == 0)
									frm.DisplayResults("Creating Index File " + f.Name.ToUpper() + "..." +  recNum.ToString());
								//position = (rec.Length + 1) + position;
								position = (rec.Length + 2) + position;
							}
							else
							{
								//recs = rec.Split('\t');
                                recs = rec.Split('|');
								if (idx.IndexOf("|",0) > -1)
								{
									indxs = idx.Split('|');
									for(int iCnt = 0;iCnt < indxs.Length;iCnt++)
										primary = primary + recs[int.Parse(indxs[iCnt])];
								}
								else
									primary = recs[int.Parse(idx)];
			
								outRec = position.ToString() + "%" + primary + "%" + secondary;
								sw.WriteLine(outRec);
								sw.Flush();
								primary = "";
								secondary = "";
								recNum++;
								if (recNum % 100 == 0)
									frm.DisplayResults("Creating Index File " + f.Name.ToUpper() + "..." +  recNum.ToString());
								//position = (rec.Length + 1) + position;
								position = (rec.Length + 2) + position;
							}
						}	
					}
                    sw.Close();
					fs.Close();
                    

					frm.DisplayResults("Removing..." + sqlTable);
					SqlCommand removeCommand = new SqlCommand("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" + sqlTable + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) DROP TABLE " + sqlTable,conn);
					removeCommand.ExecuteNonQuery();

					SqlCommand createCommand = new SqlCommand("CREATE TABLE [dbo].[" + sqlTable + "] ([RecPosition] [numeric](11, 0) NULL ,[PrimaryVal] [varchar] (25) NULL ,[SecondaryVal] [varchar] (25) NULL ) ON [PRIMARY]",conn);
					createCommand.CommandType = CommandType.Text;
					createCommand.ExecuteNonQuery();

					DataTable tbl = null;// ImportTemplate.IndexTable();
                    tbl.TableName = sqlTable;

                    Import(f.FullName, false, tbl, "%", 3);

                   // //FileInfo fi = new FileInfo(f.FullName);
                   // //fi.CopyTo(@"\\" + remoteServer + remotePath.Replace(":","$") + f.Name,true);

                   // string command = "BULK INSERT " + database + ".." + sqlTable + " FROM '" + f.FullName + "' WITH (DATAFILETYPE = 'char',FIELDTERMINATOR = '%',ROWTERMINATOR = '\n')";
                   // ////command = command.Replace(f.FullName, @"\\mphsmdev02\Input\" + f.Name);
                   //// //@"D:\HospitalData\jap\Delta\temp\Pathology\"  mphsmsqldev01
                   // //command = command.Replace(f.FullName, remotePath + fi.Name);
                   // SqlCommand bulkCommand = new SqlCommand(command, conn);
                   // bulkCommand.CommandType = CommandType.Text;
                   // bulkCommand.CommandTimeout = 1800;
                   // bulkCommand.Connection = conn;
                   // bulkCommand.ExecuteNonQuery();

                    SqlCommand idxCommand = new SqlCommand("CREATE  INDEX [IX_PRIMARY] ON [dbo].[" + sqlTable + "]([PrimaryVal]) ON [PRIMARY]", conn);
                    idxCommand.CommandType = CommandType.Text;
                    idxCommand.CommandTimeout = 1800;
                    idxCommand.ExecuteNonQuery();

					DateTime eDate = DateTime.Now;
					frm.DisplayResults(sDate.ToLongTimeString() + "  " + eDate.ToLongTimeString());
				}
			}
		}
        public void Import(string textFileName, bool hasHeader, DataTable dtTable, string delimeter, int expectedFieldCount)
        {
            //SqlDbTable.DeleteTable(_sqlConnectionString, dtTable.TableName);
            //SqlDbTable.CreateTable(_sqlConnectionString, dtTable);
            StringBuilder sb = new StringBuilder();

            StreamReader sr = new StreamReader(textFileName);
            int recCnt = 0;
            int batchCnt = 0;

            string recIn = sr.ReadLine();
            if (hasHeader)
                recIn = sr.ReadLine();
            while (recIn != null)
            {
                recCnt++;
                if (recIn.Trim().Length > 0)
                {
                    batchCnt++;
                    if (batchCnt == 50000)
                    {
                        WriteToServer(dtTable);
                        dtTable.Rows.Clear();
                        batchCnt = 0;
                    }
                    string newRec = FixString(recIn, delimeter);
                    string[] flds = newRec.Split(char.Parse(delimeter));
                    if (flds.Length == expectedFieldCount)
                    {
                        sb.Append(newRec);
                        DataRow dtRow = ProcessRecord(sb.ToString(), delimeter, dtTable, expectedFieldCount);
                        if (dtRow != null)
                            dtTable.Rows.Add(dtRow);
                        sb = new StringBuilder();
                    }
                    else
                    {
                        sb.Append(newRec.TrimEnd());
                    }
                }
                recIn = sr.ReadLine();
            }
            sr.Close();
            WriteToServer(dtTable);
        }
        private DataRow ProcessRecord(string recIn, string delimeter, DataTable dtTable, int expectedFieldCount)
        {
            DataRow dtRow = null;
            string[] flds = recIn.Split(char.Parse(delimeter));
            if (flds.Length == expectedFieldCount)
            {
                dtRow = dtTable.NewRow();
                for (int i = 0; i < flds.Length; i++)
                {
                    string data = flds[i].Trim();
                    if (dtTable.Columns[i].DataType == typeof(string))
                        dtRow[i] = data;
                    if (dtTable.Columns[i].DataType == typeof(int) && data.Length > 0)
                        dtRow[i] = data;
                    if (dtTable.Columns[i].DataType == typeof(decimal) && data.Length > 0)
                        dtRow[i] = flds[i];
                    if (dtTable.Columns[i].DataType == typeof(DateTime) && data.Length > 0)
                        dtRow[i] = flds[i];

                }
            }
            return dtRow;
        }
        private string FixString(string recIn, string delimter)
        {
            //string[] f1 = recIn.Split(char.Parse("|"));
            //string[] f2 = result.Split(char.Parse("\t"));

            string result = recIn.Replace('"' + "|" + '"', delimter);
            result = result.Replace("|" + '"', delimter);
            result = result.Replace('"' + "|", delimter);

            if (result.StartsWith("\""))
                result = result.Substring(1);
            if (result.EndsWith("\""))
                result = result.Substring(0, result.Length - 1);

            return result;
        }
        private void WriteToServer(DataTable dtTable)
        {
            string _sqlConnectionString = SqlConnectionString();
            SqlConnection conn = new SqlConnection(_sqlConnectionString);
            using (var bulk = new SqlBulkCopy(conn))
            {
                conn.Open();
                bulk.DestinationTableName = dtTable.TableName;
                bulk.WriteToServer(dtTable);
            }
        }
        public static string SqlConnectionString()
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
            b.DataSource = "SHCDSDBP01";
            b.IntegratedSecurity = true;
            b.InitialCatalog = "WestConversionDB";
            return b.ConnectionString;

        }
        public void IndexAccessFile(WestBARConversion.BARForm frm, string sqlTable, string primVal, string altVal, string idx)
		{
			string primary = "";
			string secondary = "";
			string rec = "";
			string[] recs;
			string[] indxs;
			string outRec = "";
			long position = 0;
			long recNum = 0;
			bool eof = false;
			
			FileStream fs;
			StreamWriter sw;
			if (fileInfo.Exists == true)
			{
				string text = fileInfo.FullName.ToString();
				int num = text.IndexOf(".",0);
				text = text.Substring(0,num);
				FileInfo f = new FileInfo(text + ".dat");
				if (f.Exists == false)
				{
					fs = f.OpenWrite();
					sw = new StreamWriter(fs);
					DateTime sDate = DateTime.Now;
					frm.DisplayResults(sDate.ToLongTimeString());
					while(eof == false)
					{
						rec = reader.ReadLine();
						if (rec == "" || rec == null)
							break;
						else if (char.Parse(rec.Substring(1-1,1)) == (char) 26)
							break;
						else
						{
							if(idx == "")
							{
								if (altVal.Trim() == "")
									primary = GetValueInFile(primVal,rec);
								else
								{
									primary = GetValueInFile(primVal,rec);
									secondary = GetValueInFile(altVal,rec);
								}
								outRec = position.ToString() + "%" + primary + "%" + secondary;
								sw.WriteLine(outRec);
								sw.Flush();
								primary = "";
								secondary = "";
								recNum++;
								if (recNum % 100 == 0)
									frm.DisplayResults("Creating Index File " + f.Name.ToUpper() + "..." +  recNum.ToString());

								position = (rec.Length + 2) + position;
							}
							else
							{
								recs = rec.Split('\t');
								if (idx.IndexOf("|",0) > -1)
								{
									indxs = idx.Split('|');
									for(int iCnt = 0;iCnt < indxs.Length;iCnt++)
										primary = primary + recs[int.Parse(indxs[iCnt])];
								}
								else
									primary = recs[int.Parse(idx)];
			
								outRec = position.ToString() + "%" + primary + "%" + secondary;
								sw.WriteLine(outRec);
								sw.Flush();
								primary = "";
								secondary = "";
								recNum++;
								if (recNum % 100 == 0)
									frm.DisplayResults("Creating Index File " + f.Name.ToUpper() + "..." +  recNum.ToString());

								position = (rec.Length + 2) + position;
							}
						}	
					}
					fs.Close();
                    sw.Close();
					
					frm.DisplayResults("Removing..." + sqlTable);
					SqlCommand removeCommand = new SqlCommand("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" + sqlTable + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) DROP TABLE " + sqlTable,conn);
					removeCommand.ExecuteNonQuery();

					SqlCommand createCommand = new SqlCommand("CREATE TABLE [dbo].[" + sqlTable + "] ([RecPosition] [numeric](10, 0) NULL ,[PrimaryVal] [varchar] (25) NULL ,[SecondaryVal] [varchar] (25) NULL ) ON [PRIMARY]",conn);
					createCommand.CommandType = CommandType.Text;
					createCommand.ExecuteNonQuery();

					string command = "BULK INSERT Sumter.." + sqlTable + " FROM '" + f.FullName + "' WITH (DATAFILETYPE = 'char',FIELDTERMINATOR = '%',ROWTERMINATOR = '\n')";
					SqlCommand bulkCommand = new SqlCommand(command,conn);
					bulkCommand.CommandType = CommandType.Text;
					bulkCommand.CommandTimeout = 1800;
					bulkCommand.Connection = conn;
					bulkCommand.ExecuteNonQuery();

					SqlCommand idxCommand = new SqlCommand("CREATE  INDEX [IX_PRIMARY] ON [dbo].[" + sqlTable + "]([PrimaryVal]) ON [PRIMARY]",conn);
					idxCommand.CommandType = CommandType.Text;
					idxCommand.CommandTimeout = 1800;
					idxCommand.ExecuteNonQuery();

					DateTime eDate = DateTime.Now;
					frm.DisplayResults(sDate.ToLongTimeString() + "  " + eDate.ToLongTimeString());
				}
			}
		}
		public void AddIndexToMemory(string SQLTable)
		{
			string select = "";
			string pos = "";
			string val1 = "";
			string item = "";
			char[] a = {'*'};
			SqlCommand dataCommand = new SqlCommand();
			dataCommand.Connection = conn;
			select = "Select * from " + SQLTable + " order by RecPosition";
			
			dataCommand.CommandText = select;
			SqlDataReader dataReader = dataCommand.ExecuteReader();
			while(dataReader.Read())
			{
				pos = dataReader.GetSqlDecimal(0).ToString().Trim();
				val1= dataReader.GetSqlString(1).ToString().Trim();
				if(InMemoryIndexes.Contains(val1))
				{
					item = InMemoryIndexes[val1].ToString();
					InMemoryIndexes.Remove(val1);
					InMemoryIndexes.Add(val1,item + "*" + pos);
				}
				else
					InMemoryIndexes.Add(val1,pos);
			}
			dataReader.Close();
			dataCommand.Dispose();
		}
		private string GetValueInFile(string position, string rec)
		{
			string val = "";
			string[] vals;
			string[] pos;
			int num = 0;
			vals = position.Split(' ');
			for(int nbr=0;nbr < vals.Length;nbr++)
			{
				pos = vals[nbr].Split(',');
				if(int.Parse(pos[1]) == -1)
				{
					num = int.Parse(pos[0]);
					if(rec.Length >= num)
						val = val + rec.Substring(int.Parse(pos[0])).Trim();
					else
						val = "";
				}
				else
				{
					num = int.Parse(pos[0]) + int.Parse(pos[1]);
					if(rec.Length >= num)
						val = val + rec.Substring(int.Parse(pos[0]),int.Parse(pos[1]));
					else
						val = "";
				}
			}
			return val;
		}
		public void WriteRecord (string outputRec)
		{
			if(outputRec.Trim().Length > 3)
				writer.WriteLine(outputRec);
			else if(outputRec.Trim() == "99") 
				writer.WriteLine(outputRec);
			writer.Flush();
		}
		public void WriteRecordNoCriteria (string outputRec)
		{
			writer.WriteLine(outputRec);
			writer.Flush();
		}
		public void WriteRecordNoLineFeed (string outputRec)
		{
			writer.Write(outputRec);
			writer.Flush();
		}
		public bool FindRecord(string SQLTable, string searchVal, SearchType search)
		{
			string select = "";
			bool find = false;
			string val = "";
			int cnt = 0;
			Counter = 0;
			char[] a = {'*'};
			SqlCommand dataCommand = new SqlCommand();
			dataCommand.Connection = conn;
			if (search == SearchType.PRIMARY)
			{
				select = "Select RecPosition from " + SQLTable + " where PrimaryVal = '" + searchVal + "' order by RecPosition";
			}
			else if (search == SearchType.ALTERNATE)
			{
				select = "Select RecPosition from " + SQLTable + " where SecondaryVal = '" + searchVal + "' order by RecPosition";
			}
			dataCommand.CommandText = select;
			SqlDataReader dataReader = dataCommand.ExecuteReader();
			while(dataReader.Read())
			{
				val= val + dataReader.GetSqlDecimal(0).ToString().Trim() + "*";
				cnt++;
				find = true;
			}
			returnValues = val;
			dataReader.Close();
			if (find == true)
			{
				recArray = val.Split(a);
				FindNext();
			}
			dataCommand.Dispose();
			return find;
		}
		public bool FindRecord(string SQLTable, string searchVal, SearchType search,bool condition)
		{
			string select = "";
			bool find = false;
			string val = "";
			int cnt = 0;
			Counter = 0;
			char[] a = {'*'};
			SqlCommand dataCommand = new SqlCommand();
			dataCommand.Connection = conn;
			if (search == SearchType.PRIMARY)
			{
				select = "Select RecPosition from " + SQLTable + " where PrimaryVal = '" + searchVal + "' order by RecPosition";
			}
			else if (search == SearchType.ALTERNATE)
			{
				select = "Select RecPosition from " + SQLTable + " where SecondaryVal = '" + searchVal + "' order by RecPosition";
			}
			dataCommand.CommandText = select;
			dataCommand.CommandTimeout = 1800;
			SqlDataReader dataReader = dataCommand.ExecuteReader();
			while(dataReader.Read())
			{
				val= val + dataReader.GetSqlDecimal(0).ToString().Trim() + "*";
				cnt++;
				find = true;
			}
			returnValues = val;
			dataReader.Close();
			if (find == true)
			{
				recArray = val.Split(a);
				FindNext();
			}
			dataCommand.Dispose();
			return find;
		}
		private string GetRecord(long recPos)
		{
			long pos = fileStream.Seek(recPos,SeekOrigin.Begin);
			fileStream.Position = pos;
			//reader. (fileStream);

			//string r = reader.ReadLine();
			fileStream.Read(recBytes,0,NumberOfBytes);
			string r = GetString();
			return r;
		}
		private string GetString()
		{
			string rec = "";
			char t = ' ';
			char OD = '\x000d';
			char OA = '\x000a';
			foreach(byte j in recBytes)
			{
				//if(t == OD && (char)j == OA)
				if((char)j == OA)
				{
					if(rec.Length > 0)
						//rec = rec.Substring(0,rec.Length-1);
						rec = rec.Substring(0,rec.Length);
					break;
				}
				else
				{
					rec = rec + (char)j;
					t = (char)j;
				}
				
			}
			rec = rec.Replace(OD.ToString(),"");
			return rec;
		}
		public void ReadRecord()
		{
			string r = reader.ReadLine();
			if (r == "" || r == null)
			{
				RecordData = "END OF FILE";
			}
			else if (char.Parse(r.Substring(1-1,1)) == (char) 26)
			{
				RecordData = "END OF FILE";
			}
			else
			{
				RecordData = r;
			}
		}
		public void ReadRecord_All()
		{
			string r = reader.ReadLine();
			if(r == "")
				RecordData = r;
			else if(r == null)
				RecordData = "END OF FILE";
			else if (char.Parse(r.Substring(1-1,1)) == (char) 26)
			{
				RecordData = "END OF FILE";
			}
			else
			{
				RecordData = r;
			}
		}
        public void ReadRecord_Alt()
        {
            try
            {
                string r = reader.ReadLine();
                if (r == "")
                    RecordData = r;
                else
                {
                    if (char.Parse(r.Substring(1 - 1, 1)) == (char)26)
                    {
                        RecordData = "END OF FILE";
                    }
                    else
                    {
                        RecordData = r;
                    }
                }
            }
            catch
            {
                RecordData = "END OF FILE";
            }
        }
		public void ReadRecordNext()
		{
			string r = reader.ReadLine();
			if (r == "" || r == null)
			{
				RecordData = "END OF FILE";
			}
			else if (char.Parse(r.Substring(1-1,1)) == (char) 26)
			{
				RecordData = "END OF FILE";
			}
			else
			{
				RecordData = r;
			}
		}
		public void ReadRecordEH(int num)
		{
			string r = reader.ReadLine();
			if (r == "" || r == null)
			{
				if(num > 1540000)
					RecordData = "END OF FILE";
			}
			else if (char.Parse(r.Substring(1-1,1)) == (char) 26)
			{
				RecordData = "END OF FILE";
			}
			else
			{
				RecordData = r;
			}
		}
		public static string TestString(string test)
		{
			int nbr = 0;
			string newStr = "";
			for(int y=0;y<test.Length;y++)
			{
				nbr = (int)char.Parse(test.Substring(y,1));
				if(nbr == 1 || nbr == 2)
					newStr = newStr + test.Substring(y,1);
				else if(nbr >=32 && nbr <=126)
					newStr = newStr + test.Substring(y,1);
				else
					newStr = newStr + " ";
			}
			return newStr;
		}
		public static string AccessRecordData(string tbl)
		{
			string k = "";
			object[] recs = new Object[100];
			int numOfObjs = 0;
			char t = '\t';
			string rec = "";
			if(AccessReader.Read() == false)
				return "END OF FILE";
			else
			{
				numOfObjs = AccessReader.GetValues(recs);
				for(int y=0;y<numOfObjs;y++)
				{
					k = recs[y].ToString();
					k = TestString(k);
					rec = rec + t.ToString() + k;
				}
				rec = rec.Substring(1);
			}
			return rec;
		}
		public bool FindRecordInMemory(string searchVal)
		{
			string select = "";
			bool find = false;
			string val = "";
			int cnt = 0;
			Counter = 0;
			char[] a = {'*'};
			if(InMemoryIndexes.Contains(searchVal))
			{
				val = InMemoryIndexes[searchVal].ToString();
				val = val + "*";
				find = true;
			}
			returnValues = val;
			if (find == true)
			{
				recArray = val.Split(a);
				FindNext();
			}
			return find;
		}
		public void FindNext()
		{
			if (Counter < recArray.GetUpperBound(0))
				RecordData = GetRecord(long.Parse(recArray[Counter]));
			else
				RecordData = "END OF RECORD";
			Counter++;
		}
		public void ReInitializeReadtoBeginning()
		{
			char[] a = {'*'};
			Counter = 0;
			recArray = returnValues.Split(a);
			FindNext();
		}
	}
}
