namespace Guess_Word_Backend.Models
{
    public class Word
    {
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
