using System.ComponentModel.DataAnnotations;

namespace COVT_Web.Models.DB
{
    public class PatsientiDb
    {
        [Required]
        [Key]
        public int id_patsienta { get; set; }

        [Required(ErrorMessage = "Пожалуйста выберите врача")]
        public int id_vracha { get; set; }

        [Required(ErrorMessage = "Пожалуйста заполните ФИО")]
        public string familiya { get; set; }

        [Required(ErrorMessage = "Пожалуйста заполните дату рождения")]
        public DateTime data_rozhdeniya { get; set; }

        [Required(ErrorMessage = "Пожалуйста заполните наличие операции")]
        public string nalichie_operatsii { get; set; }

        [Required(ErrorMessage = "Пожалуйста заполните номер телефона")]
        public string telefon { get; set; }

        [Required(ErrorMessage = "Пожалуйста заполните адрес")]
        public string adres { get; set; }

    }

}
