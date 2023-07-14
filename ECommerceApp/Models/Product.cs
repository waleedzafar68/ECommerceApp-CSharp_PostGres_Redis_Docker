namespace ECommerceApp.Models
{
    public class Product : CommonAttributes
    {
        private string type = "Product";
        public virtual decimal price {  get; set; }
        public  virtual string description { get; set; }
        public  virtual string category { get; set; }
    }
}
