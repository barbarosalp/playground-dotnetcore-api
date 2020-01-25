namespace Barb.Core.Api.Models
{
    public class Whisky
    {
        private string Name { get; }
        private string Origin { get; }
        
        public Whisky(string name, string origin)
        {
            Name = name;
            Origin = origin;
        }
        
        

    }
}