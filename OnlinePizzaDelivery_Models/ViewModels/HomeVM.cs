using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaDelivery_Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Pizza> Pizzas { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
