using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.Models
{
    public class Category
    {
        [Key] //dice a entity framework che Id è la primary key, Prendera anche automaticamente Id o Il nome
              //del modello + Id EX. ModelId
        public int Id { get; set; }
        [Required]//va a dire a entity framework e il database che questo campo è obbligatorio per questo oggetto
        [DisplayName("Category Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Must be within 1-100")]
        public int DisplayOrder { get; set; }
    }
}
