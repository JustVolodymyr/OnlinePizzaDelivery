using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlinePizzaDelivery_Models.ViewModels
{
    public class PizzaVM
    {
        public Pizza Pizza { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
        public IEnumerable<SelectListItem> ApplicationTypeSelectList { get; set; }
    }
}
