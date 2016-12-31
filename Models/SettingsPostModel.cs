using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
    public class SettingsPostModel
    {
        [Required]
        [Range(1,100)]
        public int Items { get; set; }

        [Required]
        [RegularExpression("en|de")]
        public string Language { get; set; }
    }
}