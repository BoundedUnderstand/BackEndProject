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

        /// <summary>
        /// Zwraca listę wszystkich subskrypcji z przypisanymi klientami.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Subscription>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            return await _context.Subscriptions.Include(s => s.customer).ToListAsync();
        }

        /// <summary>
        /// Zwraca subskrypcję o podanym ID.
        /// </summary>
        /// <param name="id">ID subskrypcji</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Subscription), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Subscription>> GetSubscription(int id)
        {
            var subscription = await _context.Subscriptions.Include(s => s.customer).FirstOrDefaultAsync(s => s.id == id);

            if (subscription == null)
            {
                return NotFound();
            }

            return subscription;
        }

        /// <summary>
        /// Aktualizuje istniejącą subskrypcję.
        /// </summary>
        /// <param name="id">ID subskrypcji</param>
        /// <param name="subscription">Zaktualizowane dane subskrypcji</param>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Tworzy nową subskrypcję.
        /// </summary>
        /// <param name="subscription">Dane nowej subskrypcji</param>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Subscription), StatusCodes.Status201Created)]
        public async Task<ActionResult<Subscription>> PostSubscription(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubscription", new { id = subscription.id }, subscription);
        }

        /// <summary>
        /// Usuwa subskrypcję o podanym ID.
        /// </summary>
        /// <param name="id">ID subskrypcji</param>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
