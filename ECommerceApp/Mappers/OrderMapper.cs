using FluentNHibernate.Mapping;
using ECommerceApp.Models;
namespace ECommerceApp.Mappers
{
    public class OrderMapper : ClassMap<Order>
    {
        public OrderMapper() {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.DateofEntry);
            Map(x => x.Address);
        }
    }
}
