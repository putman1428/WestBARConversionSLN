

namespace WestBARConversion.CernerModel
{//sb.Append("SELECT  charge_item_id, charge_description, item_extended_price, posted_dt_tm, service_dt_tm, charge_type_cd, item_quantity, payor_id, perf_phys_id, ord_phys_id, posted_id,  ");
            
    public class Charge
    {
        public string Charge_Item_Id { get; set; }
        public string Service_Dt_Tm { get; set; }
        public string Posted_Dt_Tm { get; set; }
        public string Item_Extended_Price { get; set; }
        public string Charge_Description { get; set; }
        public string Charge_Type_Cd { get; set; }
        public string Quantity { get; set; }
        public string Payor_Id { get; set; }
        public string Perf_Phy_Id { get; set; }
        public string Ord_Phy_Id { get; set; }
        public string User_Id { get; set; }

        public string BILL_ITEM_ID { get; set; }
    }
}
