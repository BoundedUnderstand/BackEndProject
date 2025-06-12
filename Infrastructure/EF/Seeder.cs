using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF
{
    public class Seeder
    {
        public static void Seed (AppDbContext context)
        {
            if (context.Subscriptions.Any())
            {
                return; // Database has been seeded
            }

            var path = Path.Combine(AppContext.BaseDirectory, "Data", "Subscriptions.csv");

            var lines = File.ReadAllLines(path);

            var Subscriptions = new List<Subscription>();

            var customers = new Dictionary<int, Customer>();
            foreach (var line in lines.Skip(1))
           
            {
                var parts = line.Split(',');
                if (parts.Length < 3) continue; // Skip invalid line
                DateTime? cancel = new DateTime();

                var customer = new Customer { customerId = int.Parse(parts[0]) };

                if (!customers.TryGetValue(int.Parse(parts[0]), out Customer cust))
                {
                    cust = new Customer()
                    {
                        customerId = int.Parse(parts[0]),
                    };
                    customers[int.Parse(parts[0])] = cust;
                }


                if (string.IsNullOrEmpty(parts[2]))
                {
                    cancel = null;
                }
                else
                {
                    cancel = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                
                bool wasPaid = false;
                if (parts[5].ToLower() == "no")
                {
                    wasPaid = false;
                }
                else
                {
                    wasPaid = true;
                }
                var subscription = new Subscription
                {
                    CustomerId = customer.customerId,
                    createdDate = DateTime.Parse(parts[1]),
                    canceledDate = cancel,
                    subscriptionCost = decimal.Parse(parts[3]),
                    subscriptionInterval = parts[4],
                    wassubscriptionPaid = wasPaid,
                };
                Subscriptions.Add(subscription);
                
            }

            context.ChangeTracker.Clear();
            
            context.Customers.AddRange(customers.Values);
            context.Subscriptions.AddRange(Subscriptions);
            context.SaveChanges();
        }
    }
}
