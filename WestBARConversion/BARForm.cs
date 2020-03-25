using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace WestBARConversion
{
    public partial class BARForm : Form
    {
        public BARForm()
        {
            InitializeComponent();
        }

        private void btntest_Click(object sender, EventArgs e)
        {
           
        }
        public void DisplayResults(string status)
        {
            lblStatus.Text = status;
            lblStatus.Refresh();
            Application.DoEvents();
        }
        private void btnConvertBAR_AZ_Click(object sender, EventArgs e)
        {
            BARConversion b = new BARConversion();
            //b.Convert(this, "ZERO", "SHAZ", @"E:\Huron\Output", "SHAZ");
            b.Convert(this, "FB", "SHUT", @"E:\Huron\Output", "SHUT");
        }

        private void btnConvertBAR_AZ_MVHX(object sender, EventArgs e)
        {
            
        }

        private void btnConvertBAR_AZ_TSHX_SLHX_Click(object sender, EventArgs e)
        {
            BARConversion b = new BARConversion();
            //b.Convert(this, "FB", "'TSHX','SLHX'", @"E:\Huron\Output", "SHAZ");
            //b.Convert(this, "UB", "'TSHX','SLHX'", @"E:\Huron\Output", "SHAZ");
            //b.Convert(this, "BD", "'TSHX','SLHX'", @"E:\Huron\Output", "SHAZ");
            b.Convert(this, "ZERO", "'TSHX','SLHX'", @"E:\Huron\Output", "SHAZ");
        }

        private void btnConvertBAR_AZ_SBHX_Click(object sender, EventArgs e)
        {
            BARConversion b = new BARConversion();
            //b.Convert(this, "FB", "'SBHX'", @"E:\Huron\Output", "SHAZ");
            //b.Convert(this, "UB", "'SBHX'", @"E:\Huron\Output", "SHAZ");
            //b.Convert(this, "BD", "'SBHX'", @"E:\Huron\Output", "SHAZ");
            b.Convert(this, "ZERO", "'SBHX'", @"E:\Huron\Output", "SHAZ");
        }

        private void btnConvertBAR_AZ_MVHX1_Click(object sender, EventArgs e)
        {
            BARConversion b = new BARConversion();
            b.Convert(this, "FB", "'MVHX'", @"E:\Huron\Output", "SHAZ");
            //b.Convert(this, "UB", "'MVHX'", @"E:\Huron\Output", "SHAZ");
            //b.Convert(this, "BD", "'MVHX'", @"E:\Huron\Output", "SHAZ");
            //b.Convert(this, "ZERO", "'MVHX'", @"E:\Huron\Output", "SHAZ");

            //b.Convert(this, "FB", "'DHHX'", @"E:\Huron\Output", "SHUT");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int cntr = 1;
            int count = 0;
            int num = 0;
            bool eof = false;
            string rec = "";
            string[] recs;
            string record = "";
            DataIO fI1 = new DataIO();
            DataIO fI2 = new DataIO();
            DataIO fI3 = new DataIO();
            DataIO fI4 = new DataIO();
            DataIO fI5 = new DataIO();
            DataIO fO = new DataIO();
            DataIO fO2 = new DataIO();
            //Hashtable hash = new Hashtable();
            //Hashtable hashC = new Hashtable();
            //ArrayList lst = new ArrayList();

            fI1.OpenFile(@"E:\Huron\Output\SHAZ_TSHX_SLHX_FB_1.txt", DataIO.IO.INPUT);
            fO.OpenFile(@"E:\Huron\Output\TestAZ.txt", DataIO.IO.OUTPUT);
            //fO2.OpenFile(@"P:\TempFiles\DupRec2.txt", DataIO.IO.OUTPUT);

            while (eof == false)
            {
                fI1.ReadRecord();
                rec = fI1.RecordData;
                if (rec == "END OF FILE")
                    break;
                else
                {
                    if (rec.Substring(0, 2) == "10" || rec.Substring(0, 2) == "40")
                    {
                        if (rec.Substring(0, 2) == "40")
                            rec = rec.Substring(0, 14) + rec.Substring(14).Trim();
                        fO.WriteRecordNoCriteria(rec);
                    }

                }
                count++;
                if (count % 100 == 0)
                    DisplayResults(count.ToString());
            }
            //while (eof == false)
            //{
            //    fI1.ReadRecord();
            //    rec = fI1.RecordData;
            //    if (rec == "END OF FILE")
            //        break;
            //    else
            //    {
            //        recs = rec.Split('\t');
            //        if (hash.Contains(recs[0].Trim()) == false)
            //            fO2.WriteRecordNoCriteria(rec);



            //    }
            //    count++;
            //    if (count % 100 == 0)
            //        DisplayResults(count.ToString());
            //}


            lblStatus.Text = count.ToString();
            Application.DoEvents();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int cntr = 1;
            int count = 0;
            int num = 0;
            bool eof = false;
            string rec = "";
            string[] recs;
            string record = "";
            DataIO fI1 = new DataIO();
            DataIO fI2 = new DataIO();
            DataIO fI3 = new DataIO();
            DataIO fI4 = new DataIO();
            DataIO fI5 = new DataIO();
            DataIO fO = new DataIO();
            DataIO fO2 = new DataIO();
            //Hashtable hash = new Hashtable();
            //Hashtable hashC = new Hashtable();
            //ArrayList lst = new ArrayList();

            fI1.OpenFile(@"P:\Huron_Extracts\BARFiles\Encounter_20200317.csv", DataIO.IO.INPUT);
            //fO.OpenFile(@"E:\Huron\Output\TestAZ.txt", DataIO.IO.OUTPUT);
            //fO2.OpenFile(@"P:\TempFiles\DupRec2.txt", DataIO.IO.OUTPUT);

            while (eof == false)
            {
                fI1.ReadRecord_All();
                rec = fI1.RecordData;
                if (rec == "END OF FILE")
                    break;
                //else
                //{
                //    if (rec.Substring(0, 2) == "10" || rec.Substring(0, 2) == "40")
                //    {
                //        if (rec.Substring(0, 2) == "40")
                //            rec = rec.Substring(0, 14) + rec.Substring(14).Trim();
                //        fO.WriteRecordNoCriteria(rec);
                //    }

                //}
                count++;
                if (count % 100 == 0)
                    DisplayResults(count.ToString());
            }
        }

        private void btnConvertBARUT1_Click(object sender, EventArgs e)
        {
            BARConversion b = new BARConversion();
            b.Convert(this, "ZERO", "'DHHX'", @"E:\Huron\Output", "SHUT");
            //b.Convert(this, "UB", "'MVHX'", @"E:\Huron\Output", "SHUT");
            //b.Convert(this, "BD", "'MVHX'", @"E:\Huron\Output", "SHUT");
            //b.Convert(this, "ZERO", "'MVHX'", @"E:\Huron\Output", "SHUT");

            //b.Convert(this, "FB", "'DHHX'", @"E:\Huron\Output", "SHUT");
        }

        private void btnCorpLogReltn_Click(object sender, EventArgs e)
        {
            int cntr = 1;
            int count = 0;
            int num = 0;
            bool eof = false;
            string rec = "";
            string[] recs;
            string record = "";
            DataIO fI1 = new DataIO();
            DataIO fI2 = new DataIO();
            DataIO fI3 = new DataIO();
            DataIO fI4 = new DataIO();
            DataIO fI5 = new DataIO();
            DataIO fO = new DataIO();
            DataIO fO2 = new DataIO();
            //Hashtable hash = new Hashtable();
            //Hashtable hashC = new Hashtable();
            //ArrayList lst = new ArrayList();

            fI1.OpenFile(@"E:\Huron\Input\CORSP_LOG_RELTN.csv", DataIO.IO.INPUT);
            fO.OpenFile(@"E:\Huron\Input\CORSP_LOG_RELTN_REDUCED.csv", DataIO.IO.OUTPUT);
            
            while (eof == false)
            {
                fI1.ReadRecord_All();
                rec = fI1.RecordData;
                if (rec == "END OF FILE")
                    break;
                else
                {
                    recs = rec.Split(',');
                    if (recs[13]!= "0")
                    {
                        fO.WriteRecordNoCriteria(rec);
                    }

                }
                count++;
                if (count % 100 == 0)
                    DisplayResults(count.ToString());
            }
            
            lblStatus.Text = count.ToString();
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int cntr = 1;
            int count = 0;
            int num = 0;
            bool eof = false;
            string rec = "";
            string[] recs;
            string record = "";
            string ftype = "";
            DataIO fI1 = new DataIO();
            DataIO fI2 = new DataIO();
            DataIO fI3 = new DataIO();
            DataIO fI4 = new DataIO();
            DataIO fI5 = new DataIO();
            DataIO fO = new DataIO();
            DataIO fO2 = new DataIO();
            Hashtable hash = new Hashtable();
            string acct = "";
            string nacct = "";
            Hashtable hashC = new Hashtable();
            //ArrayList lst = new ArrayList();

            fI1.OpenFile(@"E:\Huron\Input\RCA_DRV_ATB_DET_FEB 13_MVHX_TAB.txt", DataIO.IO.INPUT);
            fI2.OpenFile(@"E:\Huron\Output\BAR\SHAZ_MVHX_FB_20200308_1.txt", DataIO.IO.INPUT);
            fI3.OpenFile(@"E:\Huron\Output\BAR\SHAZ_MVHX_UB_20200308_1.txt", DataIO.IO.INPUT);
            fI4.OpenFile(@"E:\Huron\Output\BAR\SHAZ_MVHX_BD_20200308_1.txt", DataIO.IO.INPUT);
            fI5.OpenFile(@"E:\Huron\Output\BAR\SHAZ_MVHX_ZERO_20200308_1.txt", DataIO.IO.INPUT);
            fO.OpenFile(@"E:\Huron\Input\BarResultsMVHX.txt", DataIO.IO.OUTPUT);

            while (eof == false)
            {
                fI1.ReadRecord_All();
                rec = fI1.RecordData;
                if (rec == "END OF FILE")
                    break;
                else
                {
                    recs = rec.Split('\t');
                    hash.Add(recs[2].Trim(),"");
                }
                count++;
                if (count % 100 == 0)
                    DisplayResults(count.ToString());
            }
            for (int x = 1; x <= 4; x++)
            {
                while (eof == false)
                {
                    if (x == 1)
                    {
                        fI2.ReadRecord_All();
                        rec = fI2.RecordData;
                        ftype = "FB";
                    }
                    else if (x == 2)
                    {
                        fI3.ReadRecord_All();
                        rec = fI3.RecordData;
                        ftype = "UB";
                    }
                    else if (x == 3)
                    {
                        fI4.ReadRecord_All();
                        rec = fI4.RecordData;
                        ftype = "BD";
                    }
                    else if (x == 4)
                    {
                        fI5.ReadRecord_All();
                        rec = fI5.RecordData;
                        ftype = "ZERO";
                    }
                    if (rec == "END OF FILE")
                        break;
                    else
                    {
                        if (rec.Substring(0, 2) == "20")
                        {
                            nacct = rec.Substring(2, 12);
                            acct = "15" + nacct.Substring(3);
                            if (hash.Contains(acct) == true)
                                fO.WriteRecordNoCriteria(nacct + '\t' + acct + '\t' + ftype + '\t' + "FOUND ON ATB");
                            else
                            {
                                
                                if (rec.Substring(16, 8).Trim() != "REF")
                                    fO.WriteRecordNoCriteria(nacct + '\t' + acct + '\t' + ftype + '\t' + "NOT FOUND ON ATB" + '\t' + "(CLIENT ACCOUNT)");
                                else
                                    fO.WriteRecordNoCriteria(nacct + '\t' + acct + '\t' + ftype + '\t' + "NOT FOUND ON ATB");
                            }
                            if (hashC.Contains(acct) == false)
                                hashC.Add(acct, "");
                        }

                    }
                    count++;
                    if (count % 100 == 0)
                        DisplayResults(count.ToString());
                }
            }
            fI1.CloseFile();
            fI1.OpenFile(@"E:\Huron\Input\RCA_DRV_ATB_DET_FEB 13_MVHX_TAB.txt", DataIO.IO.INPUT);
            while (eof == false)
            {
                fI1.ReadRecord_All();
                rec = fI1.RecordData;
                if (rec == "END OF FILE")
                    break;
                else
                {
                    recs = rec.Split('\t');
                    acct = recs[2].Trim();
                    nacct = "ZV0" + acct.Substring(2);
                    if (hashC.Contains(recs[2].Trim()) == false)
                        fO.WriteRecordNoCriteria(nacct + '\t' + acct + '\t' + "FOUND ON ATB NOT IN FILE");
                    else
                        fO.WriteRecordNoCriteria(nacct + '\t' + acct + '\t' + "FOUND ON ATB AND FILE");
                }
                count++;
                if (count % 100 == 0)
                    DisplayResults(count.ToString());
            }

            lblStatus.Text = count.ToString();
            Application.DoEvents();
        }
    }
}
