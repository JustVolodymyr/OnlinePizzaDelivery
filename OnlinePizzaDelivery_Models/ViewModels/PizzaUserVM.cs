using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaDelivery_Models.ViewModels
{
    public class PizzaUserVM
    {
        public PizzaUserVM()
        {
            PizzaList = new List<Pizza>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IList<Pizza> PizzaList { get; set; }
    }
}
