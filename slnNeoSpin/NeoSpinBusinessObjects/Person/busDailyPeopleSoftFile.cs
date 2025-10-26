using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDailyPeopleSoftFile
    {
        public string employee_id { get; set; }
        public string employee_record_number { get; set; }
        public string business_unit { get; set; }
        public string plan_type { get; set; }
        public string benefit_plan { get; set; }
        public DateTime coverage_begin_date { get; set; }
        public DateTime deduction_begin_date { get; set; }
        public string coverage_election { get; set; }
        public DateTime election_date { get; set; }
        public string coverage_code { get; set; }
        public decimal flat_amount { get; set;}
        public string direct_deposit { get; set; }
        public string inside_mail { get; set; }
        public string company { get; set; }
        public string calculation_routine { get; set; }
        public int person_id { get; set; }

        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization(int AintOrgID)
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization();
            }
            ibusOrganization.FindOrganization(AintOrgID);
        }
    }
}
