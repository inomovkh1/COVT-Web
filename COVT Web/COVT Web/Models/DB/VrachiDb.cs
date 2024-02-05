using System.ComponentModel.DataAnnotations;

namespace COVT_Web.Models.DB
{
    public class VrachiDb
    {
        [Required]
        [Key]
        public int id_vracha { get; set; }

        [Required]
        public string familiya { get; set; }

        [Required]
        public string imya { get; set; }

        [Required]
        public string otchestvo { get; set; }

        [Required]
        public string spetsialnost { get; set; }

        [Required]
        public DateTime stazh_raboti { get; set; }

        public string? obr { get; set; }
    }
}
