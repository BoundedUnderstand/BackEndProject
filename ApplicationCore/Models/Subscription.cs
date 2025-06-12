using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Subscription
    {
        [Key]
        public int customerId { get; set; }
        public DateTime createdDate { get; set; }
        
        public DateTime? canceledDate { get; set; }
        
        public int subscriptionCost { get; set; }

        public string subscriptionInterval { get; set; } 

        public bool wassubscriptionPaid { get; set; }
    }
}
