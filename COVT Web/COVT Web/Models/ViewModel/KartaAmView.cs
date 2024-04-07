﻿using COVT_Web.Models.DB;

namespace COVT_Web.Models.ViewModel
{
    public class KartaAmView
    {
        public int id_karti_patsienta { get; set; }

        public DateTime data_priema { get; set; }

        public int id_bolezni { get; set; }

        public string bolezn { get; set; }

        public int? id_kobineta { get; set; }

        public int id_patsienta { get; set; }

        public string? patsient { get; set; }

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

        public string? zakl { get; set; }
    }
}
