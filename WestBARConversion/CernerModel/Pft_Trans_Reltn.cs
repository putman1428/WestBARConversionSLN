using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class Pft_Trans_Reltn
    {
        //, ,, parent_entity_name, bill_vrsn_nbr, amount, dr_cr_flag, benefit_order_id, ");
        //sb.Append("trans_type_cd, revenue_summary_id
        public string Parent_Entity_Id { get; set; }
        public string Activity_Id { get; set; }
        public string Parent_Entity_Name { get; set; }
        public string Bill_Vrsn_Nbr { get; set; }
        public string Amount { get; set; }
        public string Dr_Cr_Flag { get; set; }
        public string Benefit_Order_Id { get; set; }
        public string Trans_Type_Cd { get; set; }
        public string Revenue_Summary_Id { get; set; }

    }
}
