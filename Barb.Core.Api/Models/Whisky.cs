namespace Barb.Core.Api.Models
{
    public class Whisky
    {
        public string Name { get; set; }
        public string Origin { get; set; }

        public int Id { get; set; }

        public Whisky()
        {
        }

        public Whisky(string name, string origin)
        {
            Name = name;
            Origin = origin;
        }
    }
}