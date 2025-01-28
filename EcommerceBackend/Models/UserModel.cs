namespace EcommerceBackend.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public bool IsVerified { get; set; } = false;
    }
}
