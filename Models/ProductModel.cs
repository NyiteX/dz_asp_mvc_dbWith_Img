namespace dz_asp_mvc_db.Models
{
    public record class ProductModel(int? Id, string? Name, string? Discription, double? Price, long? Count, byte[]? Pic);
}
