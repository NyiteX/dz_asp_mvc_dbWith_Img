using System.ComponentModel.DataAnnotations;

namespace dz_asp_mvc_db.Models
{
    public record class UserModel(int? Id, string? Login, string? Password, string? Email, byte[]? Pic);
}
