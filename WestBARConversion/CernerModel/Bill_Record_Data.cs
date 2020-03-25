

namespace WestBARConversion.CernerModel
{
    public class Bill_Record_Data
    {
        public string PFT_ENCNTR_ID { get; set; }
        public string PFT_PRORATION_ID { get; set; }
        public string ENCNTR_PLAN_RELTN_ID { get; set; }
        public string HEALTH_PLAN_ID { get; set; }
        public string BO_HP_RELTN_ID { get; set; }
        public string TOTAL_BILLED_AMOUNT { get; set; }
        public string TOTAL_BILLED_DR_CR_FLAG { get; set; }
        public string TOTAL_PAID_AMOUNT { get; set; }
        public string TOTAL_PAID_DR_CR_FLAG { get; set; }
        public string TOTAL_ADJ_AMOUNT { get; set; }
        public string TOTAL_ADJ_DR_CR_FLAG { get; set; }
        public string BOHP_ACTIND { get; set; }
        public string AMOUNT_OWED { get; set; }
        public string AMOUNT_OWED_DR_CR_FLAG { get; set; }
        public string BOHP_ENDEFFDATE { get; set; }
        public string PRIORITY_SEQ { get; set; }
        public string ORIG_AMT_DUE { get; set; }
        public string ORIG_AMOUNT_DR_CR_FLAG { get; set; }
        public string HIGH_AMT { get; set; }
        public string HIGH_AMOUNT_DR_CR_FLAG { get; set; }

        public string CURR_AMT_DUE { get; set; }
        public string CURR_AMOUNT_DR_CR_FLAG { get; set; }
        public string NON_COVERED_AMT { get; set; }
        public string NON_COVERED_AMT_DR_CR_FLAG { get; set; }
        public string TOTAL_ADJ { get; set; }
        public string Proration_TotAdjDRCRFlag { get; set; }
        public string Proration_EndEffDt { get; set; }
        public string Proration_ActInd { get; set; }
        public string BILL_RELTN_ID { get; set; }
        public string CORSP_ACTIVITY_ID { get; set; }
        public string BillReltnActInd { get; set; }
        public string BillReltnEndEffDt { get; set; }
        public string PARENT_ENTITY_ID { get; set; }
        public string PARENT_ENTITY_NAME { get; set; }
        public string BILL_VRSN_NBR { get; set; }
        public string BILL_NBR_DISP { get; set; }
        public string BILL_STATUS_CD { get; set; }
        public string BILL_STATUS_REASON_CD { get; set; }
        public string BillRec_ActInd { get; set; }
        public string BillRec_EndEffDt { get; set; }
        public string BILL_NBR_DISP_KEY { get; set; }
        public string BALANCE { get; set; }
        public string BILL_CLASS_CD { get; set; }
        public string GEN_DT_TM { get; set; }
        public string PAGE_CNT { get; set; }
        public string BILL_TYPE_CD { get; set; }
        public string BALANCE_DR_CR_FLAG { get; set; }
        public string BALANCE_FWD { get; set; }
        public string BALANCE_FWD_DR_CR_FLAG { get; set; }
        public string BALANCE_DUE { get; set; }
        public string BALANCE_DUE_DR_CR_FLAG { get; set; }
        public string NEW_AMOUNT { get; set; }
        public string NEW_AMOUNT_DR_CR_FLAG { get; set; }
        public string CLAIM_STATUS_CD { get; set; }
        public string SUBMIT_DT_TM { get; set; }
        public string LAST_PAYMENT_DT_TM { get; set; }
        public string LAST_ADJUSTMENT_DT_TM { get; set; }
        public string CLAIM_SERIAL_NBR { get; set; }
        public string TRANSMISSION_DT_TM { get; set; }
        public string FROM_SERVICE_DT_TM { get; set; }    
        public string TO_SERVICE_DT_TM { get; set; }
        public string STATEMENT_TO_DT_TM { get; set; }
        public string STATEMENT_FROM_DT_TM { get; set; }
    }
}
