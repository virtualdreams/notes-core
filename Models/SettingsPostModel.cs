using System.ComponentModel.DataAnnotations;

namespace notes.Models
{
    public class SettingsPostModel
    {
        [Required]
        [RegularExpression("[0-9]{1,3}")]
        public int Items { get; set; }

        [Required]
        [RegularExpression("en|de")]
        public string Language { get; set; }
    }
}