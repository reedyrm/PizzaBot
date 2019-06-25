using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class UserData
    {
        public String Id { get; set; }

        public String Name { get; set; }

        public int Age { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
