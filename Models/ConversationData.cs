using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class ConversationData
    {
        //State Management properties
        public bool HasWelcomed { get; set; } = false;

        public bool HasSelectedPizza { get; set; } = false;

        public bool HasSelectedPizzaSize { get; set; } = false;

        //Data
        public PizzaSize SelectedPizzaSize { get; set; } = PizzaSize.SMALL;

        public PizzaType SelectedPizzaType { get; set; } = PizzaType.NONE;
    }

    public enum PizzaSize
    {
        SMALL = 1,

        MEDIUM = 2,

        LARGE = 3,

        EXTRA_LARGE = 4
    }

    public enum PizzaType
    {
        NONE = 0,

        PEPPERONI = 1,

        CHEESE = 2,

        MEAT_LOVERS = 3,

        VEGGIE_DELIGHT = 4
    }
}
