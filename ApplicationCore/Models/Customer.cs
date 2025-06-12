using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Customer
    {
        [Key]
        public int customerId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
