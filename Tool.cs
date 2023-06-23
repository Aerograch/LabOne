using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabOne
{
    public class Tool
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public Tool() { }

        public Tool(string name, string description, string type)
        {
            Name = name;
            Description = description;
            Type = type;
        }

        public Tool(List<string> props)
        {
            Name = props[0];
            Description = props[1];
            Type = props[2];
        }
    }
}
