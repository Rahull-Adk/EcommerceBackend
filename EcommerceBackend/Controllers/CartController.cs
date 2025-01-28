using EcommerceBackend.Models;
using ECommerceBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;


namespace EcommerceBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public CartController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet("/cart")]
        public async Task<IActionResult> GetCartItems()
        {
            var currentUserEmail = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var cartItems = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.UserId == user.Id && oi.Order.Status == "Cart")
                .ToListAsync();

            return Ok(cartItems);
        }

        [Authorize]
        [HttpPost("/cart/add")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var currentUserEmail = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var cartOrder = await _context.Orders.FirstOrDefaultAsync(o => o.UserId == user.Id && o.Status == "Cart");
            if (cartOrder == null)
            {
                cartOrder = new OrderModel
                {
                    UserId = user.Id,
                    User = user,
                    OrderDate = DateTime.UtcNow,
                    Status = "Cart",
                    Items = new List<OrderItemModel>()
                };
                _context.Orders.Add(cartOrder);
            }

            var orderItem = new OrderItemModel
            {
                Order = cartOrder,
                Product = product,
                Price = product.Price,
                Quantity = quantity
            };

            cartOrder.Items.Add(orderItem);
            await _context.SaveChangesAsync();

            return Ok(orderItem);
        }

        [Authorize]
        [HttpPost("/cart/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var currentUserEmail = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var cartOrder = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Status == "Cart");

            if (cartOrder == null || !cartOrder.Items.Any())
            {
                return BadRequest("No items in cart");
            }

            cartOrder.Status = "Pending";
            cartOrder.OrderDate = DateTime.UtcNow;
            cartOrder.TotalAmount = cartOrder.Items.Sum(i => i.Price * i.Quantity);

            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(cartOrder.TotalAmount * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            await _context.SaveChangesAsync();

            return Ok(new { cartOrder, paymentIntent.ClientSecret });
        }
    }
}