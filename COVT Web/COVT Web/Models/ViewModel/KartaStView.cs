namespace COVT_Web.Models.ViewModel
{
    public class KartaStView
    {
        public int id_karti { get; set; }

        public DateTime data_postupleniya { get; set; }

        public int id_bolezni { get; set; }

        public string bolezn { get; set; }

        public int id_patsienta { get; set; }

        public string patsient { get; set; }

        public string? adres { get; set; }

        public DateTime dr { get; set; }

        public int id_vracha { get; set; }

        public string vrach { get; set; }

        public string? zhalobi { get; set; }

        public string? ist_zab { get; set; }

        public string? nast_stat { get; set; }

        public string? mest_stat { get; set; }

        public string? dop_met_obsl { get; set; }

        public string? diagnoz { get; set; }

        public string? plan_obsl { get; set; }

        public string? plan_lech { get; set; }

        public string? nablyudenie { get; set; }

        public string? khod_operat { get; set; }

        public string? zakl { get; set; }

        public string? vip_epikr { get; set; }

        public DateTime? data_vipiski { get; set; }

        public string? predoper { get; set; }

        public string? osman { get; set; }

        public string? osmspets { get; set; }

        public string? ist_zhizni { get; set; }
    }
}
