using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaDelivery_Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Pizza = new Pizza();
        }

        public Pizza Pizza { get; set; }
        public bool ExistsInCart { get; set; }
    }
}
