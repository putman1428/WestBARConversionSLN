using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class Person
    {
        public string PersonId { get; set; }
        public string Name_Full_Formatted { get; set; }
        public string Birth_Dt_Tm { get; set; }
        public string Sex_Cd { get; set; }
        public string Mother_Maiden_Name { get; set; }
        public string Vip_Cd { get; set; }
        public string Deceased_Dt_Tm { get; set; }
        public string Deceased_Tz { get; set; }
        public string Religion_Cd { get; set; }
        public string Marital_Type_Cd { get; set; }
        public string Race_Cd { get; set; }
        public string Language_Cd { get; set; }
        public string SocialSecurityNumber { get; set; }
        public Address HomeAddress { get; set; }
        public Address EmailAddress { get; set; }
        public Phone HomePhone { get; set; }
        public Phone OtherPhone { get; set; }
        public Phone MobilePhone { get; set; }
        public Phone WorkPhone { get; set; }
        public string RelationShip { get; set; }
        public string Cerner_Relationship { get; set; }
        public string Name_Last { get; set; }
        public string Name_First { get; set; }
        public string Name_Middle { get; set; }
        public string Ethnic_Group_Cd { get; set; }
        public string PrimaryCarePhysician { get; set; }
        public string PrimaryCarePhysicianName { get; set; }
        public string FullName()
        {
            return (Name_Last.ToUpper() + "," + Name_First.ToUpper() + " " + Name_Middle.ToUpper()).Trim();
        }
    }
}
