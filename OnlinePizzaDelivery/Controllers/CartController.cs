using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OnlinePizzaDelivery_DataAccess;
using OnlinePizzaDelivery_DataAccess.Repository.IRepository;
using OnlinePizzaDelivery_Models;
using OnlinePizzaDelivery_Models.ViewModels;
using OnlinePizzaDelivery_Utility;
using OnlinePizzaDelivery_Utility.BrainTree;
using System.Security.Claims;
using System.Text;

namespace OnlinePizzaDelivery.Controllers
{
    [Authorize]
    public class CartController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _userRepository;
        private readonly IPizzaRepository _pizzaRepo;
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;
        private readonly IBrainTreeGate _brain;
        [BindProperty]
        public PizzaUserVM PizzaUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IApplicationUserRepository userRepository, IPizzaRepository pizzaRepo, IOrderHeaderRepository orderHRepo, IOrderDetailRepository orderDRepo, IBrainTreeGate brain)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _pizzaRepo = pizzaRepo;
            _orderHRepo = orderHRepo;
            _orderDRepo = orderDRepo;
            _brain = brain;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> pizzaInCart = shoppingCartList.Select(i => i.PizzaId).ToList();
            IEnumerable<Pizza> pizzaListTemp = _pizzaRepo.GetAll(u => pizzaInCart.Contains(u.Id));
            IList<Pizza> pizzaList = new List<Pizza>();

            foreach (var cartObj in shoppingCartList)
            {
                Pizza pizzaTemp = pizzaListTemp.FirstOrDefault(u => u.Id == cartObj.PizzaId);
                pizzaTemp.TempCount = cartObj.Count;
                pizzaList.Add(pizzaTemp);
            }

            return View(pizzaList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Pizza> PizzaList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Pizza pizza in PizzaList)
            {
                shoppingCartList.Add(new ShoppingCart { PizzaId = pizza.Id, Count = pizza.TempCount });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;
            applicationUser = new ApplicationUser();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var userId = User.FindFirstValue(ClaimTypes.Name);

            applicationUser = _userRepository.FirstOrDefault(u => u.Id == claim.Value);

            var gatway = _brain.GetGateway();
            var clientToken = gatway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;

            //}

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> pizzaInCart = shoppingCartList.Select(i => i.PizzaId).ToList();
            IEnumerable<Pizza> pizzaList = _pizzaRepo.GetAll(u => pizzaInCart.Contains(u.Id));

            PizzaUserVM = new PizzaUserVM()
            {
                //ApplicationUser = _userRepository.FirstOrDefault(u => u.Id == claim.Value),
                ApplicationUser = applicationUser
            };
            foreach (var cartObj in shoppingCartList)
            {
                Pizza pizzaTemp = _pizzaRepo.FirstOrDefault(u => u.Id == cartObj.PizzaId);
                pizzaTemp.TempCount = cartObj.Count;
                PizzaUserVM.PizzaList.Add(pizzaTemp);

            }

            return View(PizzaUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, PizzaUserVM PizzaUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "templates" + Path.DirectorySeparatorChar.ToString() +
            "Inquiry.html";

            var subject = "New Inquiry";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            //Name: { 0}
            //Email: { 1}
            //Phone: { 2}
            //Pizzas: {3}

            StringBuilder pizzaListSB = new StringBuilder();
            foreach (var pizza in PizzaUserVM.PizzaList)
            {
                pizzaListSB.Append($" - Name: {pizza.Name} <span style='font-size:14px;'> (ID: {pizza.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                PizzaUserVM.ApplicationUser.FullName,
                PizzaUserVM.ApplicationUser.Email,
                PizzaUserVM.ApplicationUser.PhoneNumber,
                pizzaListSB.ToString());


            await _emailSender.SendEmailAsync(PizzaUserVM.ApplicationUser.Email, subject, messageBody);


            OrderHeader orderHeader = new OrderHeader()
            {
                CreatedByUserId = claim.Value,
                FinalOrderTotal = /*orderTotal*/ PizzaUserVM.PizzaList.Sum(x => x.TempCount * x.Price),
                City = PizzaUserVM.ApplicationUser.City,
                StreetAddress = PizzaUserVM.ApplicationUser.StreetAddress,
                State = PizzaUserVM.ApplicationUser.State,
                PostalCode = PizzaUserVM.ApplicationUser.PostalCode,
                FullName = PizzaUserVM.ApplicationUser.FullName,
                Email = PizzaUserVM.ApplicationUser.Email,
                PhoneNumber = PizzaUserVM.ApplicationUser.PhoneNumber,
                OrderDate = DateTime.Now,
                OrderStatus = WC.StatusPending
            };
            _orderHRepo.Add(orderHeader);
            _orderHRepo.Save();

            foreach (var pizza in PizzaUserVM.PizzaList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = orderHeader.Id,
                    PricePerCount = pizza.Price,
                    Count = pizza.TempCount,
                    PizzaId = pizza.Id
                };
                _orderDRepo.Add(orderDetail);
            }
            _orderDRepo.Save();

            string nonceFromTheClient = collection["payment_method_nonce"];

            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                PaymentMethodNonce = nonceFromTheClient,
                OrderId = orderHeader.Id.ToString(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var gateway = _brain.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.Target.ProcessorResponseText == "Approved")
            {
                orderHeader.TransactionId = result.Target.Id;
                orderHeader.OrderStatus = WC.StatusApproved;
            }
            else
            {
                orderHeader.OrderStatus = WC.StatusCancelled;
            }

            _orderHRepo.Save();


            return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(h => h.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }

        public IActionResult Remove(int id)
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.PizzaId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Pizza> PizzaList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Pizza pizza in PizzaList)
            {
                shoppingCartList.Add(new ShoppingCart { PizzaId = pizza.Id, Count = pizza.TempCount });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
