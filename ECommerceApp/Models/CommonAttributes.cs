using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace ECommerceApp.Models
{
    public abstract class CommonAttributes
    {
        private string _name;

        public virtual DateTime DateofEntry
        {
            get; set;
        }

        public virtual int ID
        {
            get; set;
        }

        public virtual string Name
        {
            get { return _name; }
            set { if (value != null) { _name = value; } }
        }
    }
}
