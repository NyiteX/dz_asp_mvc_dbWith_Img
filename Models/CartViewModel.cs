namespace dz_asp_mvc_db.Models
{
    public class CartViewModel
    {
        public ProductModel? Product { get; set; }
        public long? Count { get; set; }
        public long? Price { get; set; }

        public CartViewModel(ProductModel? model, long? count, long? price)
        {
            Product = model;
            Count = count;
            Price = price;
        }
    }

}
