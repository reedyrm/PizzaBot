using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Order
    {
        public bool isActive { get; set; } = false;

        public String DeliveryMethod { get; set; }

        public List<Pizza> OrderedPizzas { get; set; } = new List<Pizza>();
    }
}
