using System.ComponentModel.DataAnnotations;

namespace COVT_Web.Models.DB
{
    public class BolezniDb
    {
        [Key]
        [Required]
        public int id_bolezni { get; set; }

        [Required]
        public string nazvanie { get; set; }

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
