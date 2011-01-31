using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace x10sharp
{
    public class Macro
    {
        private readonly List<X10CommandSet> commands = new List<X10CommandSet>();

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public Collection<X10CommandSet> Commands
        {
            get { return new Collection<X10CommandSet>(this.commands); }
        }
    }
}
