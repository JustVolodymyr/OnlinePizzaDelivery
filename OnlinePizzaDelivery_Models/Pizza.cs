using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinePizzaDelivery_Models
{
    public class Pizza
    {
        public Pizza() 
        {
            TempCount = 1;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Ingredients { get; set; } 
        public string Description { get; set; }
        [Range(1, int.MaxValue)]
        public decimal Price { get; set; }
        public string Image { get; set; }
        [Display(Name = "Category Type")]
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [NotMapped]
        [Range(1,100,ErrorMessage = "Sqft must be greater than 0")]
        public int TempCount { get; set; }
    }
}
