using ECommerceBackend.Data;
using EcommerceBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("/orders")]
        public async Task<IActionResult> CreateOrder()
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

            await _context.SaveChangesAsync();

            return Ok(cartOrder);
        }

        [Authorize]
        [HttpGet("/orders")]
        public async Task<IActionResult> GetOrders()
        {
            var currentUserEmail = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.Id && o.Status != "Cart")
                .ToListAsync();

            return Ok(orders);
        }

        [Authorize]
        [HttpGet("/orders/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var currentUserEmail = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            return Ok(order);
        }
    }
}