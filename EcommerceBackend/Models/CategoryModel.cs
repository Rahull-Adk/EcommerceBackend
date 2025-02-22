﻿namespace EcommerceBackend.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
