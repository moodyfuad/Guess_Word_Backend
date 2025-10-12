using System.ComponentModel.DataAnnotations;

namespace Guess_Word_Backend.Models
{
    public class Word
    {
        [Key]
        public int Id { get; set; }
        public string AsString { get {
                string val = "";
                foreach (var item in Letters.Select(l => l.Name))
                {
                    val += item;
                }
                return val;
            } }

        public List<Letter> Letters = [];
    }
}
