using ApplicationCore.Models;
using Infrastructure.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubscriptionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Subscription
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            return await _context.Subscriptions.Include(s => s.customer).ToListAsync();
        }

        // GET: api/Subscription/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscription(int id)
        {
            var subscription = await _context.Subscriptions.Include(s => s.customer).FirstOrDefaultAsync(s => s.id == id);

            if (subscription == null)
            {
                return NotFound();
            }

            return subscription;
        }

        // PUT: api/Subscription/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscription(int id, Subscription subscription)
        {
            if (id != subscription.id)
            {
                return BadRequest();
            }

            _context.Entry(subscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Subscription
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Subscription>> PostSubscription(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubscription", new { id = subscription.id }, subscription);
        }

        // DELETE: api/Subscription/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.id == id);
        }
    }
}
