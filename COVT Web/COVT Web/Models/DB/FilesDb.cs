using System.ComponentModel.DataAnnotations;


namespace COVT_Web.Models.DB
{
    public class FilesDb
    {
        [Key]
        [Required]
        public int id_file { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public int id_patsienta { get; set; }

        public string? opisanie { get; set; }

        public string? name { get; set; }

        [Required]
        public byte[] Fl { get; set; }
    }
}
