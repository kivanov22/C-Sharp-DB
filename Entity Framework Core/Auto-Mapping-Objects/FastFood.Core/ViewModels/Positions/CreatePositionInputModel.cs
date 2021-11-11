using System.ComponentModel.DataAnnotations;

namespace FastFood.Core.ViewModels.Positions
{
    public class CreatePositionInputModel
    {
        [Required]
        [StringLength(30, MinimumLength = 3,ErrorMessage ="Position name must be between 3 and 30 characters long")]
        public string PositionName { get; set; }
    }
}
