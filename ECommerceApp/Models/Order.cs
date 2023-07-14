using Microsoft.AspNetCore.Server.HttpSys;
using System.Xml.Schema;

namespace ECommerceApp.Models
{
    public class Order : CommonAttributes
    {
        private string Type = "Order";
        public virtual string Address { get; set; } ="";
       

    }
}
