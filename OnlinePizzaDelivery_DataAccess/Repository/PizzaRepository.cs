using Microsoft.AspNetCore.Mvc.Rendering;
using OnlinePizzaDelivery_DataAccess.Repository.IRepository;
using OnlinePizzaDelivery_Models;
using OnlinePizzaDelivery_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePizzaDelivery_DataAccess.Repository
{
    public class PizzaRepository : Repository<Pizza>, IPizzaRepository
    {
        private readonly ApplicationDbContext _db;
        public PizzaRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == WC.CategoryName)
            {
                return _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            return null;
        }

        public void Update(Pizza obj)
        {
            _db.Pizza.Update(obj);
        }
    }
}
