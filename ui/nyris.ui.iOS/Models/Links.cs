namespace Nyris.UI.iOS.Models
{
    public class Links
    {
        public string Main { get; set; }

        public string Mobile { get; set; }

        public override string ToString()
        {
            return $"Main: {Main}, \n" +
                   $"Mobile: {Mobile}";
        }
    }
}