using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookShop.Models.Domain
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The AuthorName field is required.")]
        public string AuthorName { get; set; }


        [Required(ErrorMessage = "The TotalPages field is required.")]
        public int TotalPages { get; set; }

        [Required(ErrorMessage = "The Price field is required.")]
        public decimal Price { get; set; }

        [ValidateNever]
        public string ImageFileName { get; set; }

        public int GenreID { get; set; }
        [ForeignKey("GenreID")]

        [ValidateNever]
        public Genre Genres { get; set; }

    }
}
