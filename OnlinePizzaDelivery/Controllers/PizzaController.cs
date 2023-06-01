using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlinePizzaDelivery_DataAccess;
using OnlinePizzaDelivery_DataAccess.Repository.IRepository;
using OnlinePizzaDelivery_Models;
using OnlinePizzaDelivery_Models.ViewModels;
using OnlinePizzaDelivery_Utility;
using System.Data;

namespace OnlinePizzaDelivery.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class PizzaController : Controller
    {
        private readonly IPizzaRepository _pizzaRepo;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PizzaController(IPizzaRepository prodRepo, IWebHostEnvironment _webHostEnvironment)
        {
            _pizzaRepo = prodRepo;
            webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Pizza> objList = _pizzaRepo.GetAll(includeProperties: "Category");
            return View(objList);
        }

        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {

            PizzaVM pizzaVM = new PizzaVM()
            {
                Pizza = new Pizza(),
                CategorySelectList = _pizzaRepo.GetAllDropdownList(WC.CategoryName),
                ApplicationTypeSelectList = _pizzaRepo.GetAllDropdownList(WC.ApplicationTypeName),
            };

            if (id == null)
            {
                //this is for create
                return View(pizzaVM);
            }
            else
            {
                pizzaVM.Pizza = _pizzaRepo.Find(id.GetValueOrDefault());
                if (pizzaVM.Pizza == null)
                {
                    return NotFound();
                }
                return View(pizzaVM);
            }
        }


        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(PizzaVM pizzaVM)
        {
            if (ModelState.IsValid/* && db.Pizza.Any(o => o.CategoryId == pizzaVM.Pizza.CategoryId)*/)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = webHostEnvironment.WebRootPath;

                if (pizzaVM.Pizza.Id == 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    pizzaVM.Pizza.Image = fileName + extension;

                    _pizzaRepo.Add(pizzaVM.Pizza);
                }
                else
                {
                    //updating
                    var objFromDb = _pizzaRepo.FirstOrDefault(u => u.Id == pizzaVM.Pizza.Id, isTracking:false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        pizzaVM.Pizza.Image = fileName + extension;
                    }
                    else
                    {
                        pizzaVM.Pizza.Image = objFromDb.Image;
                    }
                    _pizzaRepo.Update(pizzaVM.Pizza);
                }

                _pizzaRepo.Save();
                TempData[WC.Success] = "Pizza Added Successfully";
                return RedirectToAction("Index");
            }
            pizzaVM.CategorySelectList = _pizzaRepo.GetAllDropdownList(WC.CategoryName);
            pizzaVM.ApplicationTypeSelectList = _pizzaRepo.GetAllDropdownList(WC.ApplicationTypeName); 


            return View(pizzaVM);
        }

        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Pizza Pizza = _pizzaRepo.FirstOrDefault(u=>u.Id == id, includeProperties: "Category");
            if (Pizza == null)
            {
                return NotFound();
            }

            return View(Pizza);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _pizzaRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WC.Error] = "Error while removing Pizza";
                return NotFound();
            }

            string upload = webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }


            _pizzaRepo.Remove(obj);
            _pizzaRepo.Save();
            TempData[WC.Success] = "Pizza Removed to Cart Successfully";
            return RedirectToAction("Index");


        }


    }
}
