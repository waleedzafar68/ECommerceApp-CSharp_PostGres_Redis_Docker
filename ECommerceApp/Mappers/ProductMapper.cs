
    using FluentNHibernate.Mapping;
    using ECommerceApp.Models;
    namespace ECommerceApp.Mappers;
    public class ProductMapper : ClassMap<Product>
{
    public ProductMapper() {
        Id(x => x.ID);
        Map(x => x.Name);
        Map(x => x.DateofEntry);
        Map(x => x.price);
        Map(x => x.description);
        Map(x => x.category);
    }
}
