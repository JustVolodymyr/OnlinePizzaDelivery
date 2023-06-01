using Microsoft.AspNetCore.Mvc.Rendering;
using OnlinePizzaDelivery_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePizzaDelivery_DataAccess.Repository.IRepository
{
    public interface IPizzaRepository : IRepository<Pizza>
    {
        void Update(Pizza obj);

        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}
