using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlisverisListesi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlisverisListesi.Controllers
{
    [Authorize] // Sadece oturum açmış kullanıcıların erişimine izin verir
    public class ShoppingListsController : Controller
    {
        private readonly ShoppingListDbContext _context;

        public ShoppingListsController(ShoppingListDbContext context)
        {
            _context = context;
        }

        // GET: ShoppingLists
        public async Task<IActionResult> Index()
        {

            if (!User.Identity.IsAuthenticated)
            {
                // Kullanıcı giriş yapmadıysa, giriş yapmaları için yönlendirin.
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(User.FindFirstValue("UserId"));// Kullanıcının kimliğini alır
            var shoppingLists = await _context.ShoppingLists
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return View(shoppingLists);
        }

        // GET: ShoppingLists/Create
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Kullanıcı giriş yapmadıysa, giriş yapmaları için yönlendirin.
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListId,UserId,Name,IsCompleted")] ShoppingList shoppingList)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Kullanıcı giriş yapmadıysa, giriş yapmaları için yönlendirin.
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue("UserId")); // Kullanıcının kimliğini alır
                shoppingList.UserId = userId; // Oturum açmış kullanıcının kimliğini otomatik olarak alır
                _context.Add(shoppingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingList);
        }
        // GET: ShoppingLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingLists
                .Include(s => s.ListItems)// Alışveriş listesine ait öğeleri dahil ediyoruz (varsa)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(m => m.ListId == id && (m.UserId == int.Parse(User.FindFirstValue("UserId")) || m.UserId == null));

            if (shoppingList == null)
            {
                return NotFound();
            }

            return View(shoppingList);
        }
        //GET: ShoppingLists/AddProduct/5
        public IActionResult AddProduct()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(int id, [Bind("ListId,ProductId")] ListItem listItem)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Kullanıcı giriş yapmadıysa, giriş yapmaları için yönlendirin.
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                listItem.ListId = id;
                _context.ListItems.Add(listItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(listItem);
        }

        // GET: ShoppingLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingLists.FindAsync(id);

            if (shoppingList == null || shoppingList.UserId != int.Parse(User.FindFirstValue("UserId")))
            {
                return NotFound();
            }
            //var categories = _context.Products.ToList();
            //ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            //ViewBag.CategoryNames = new SelectList(categories, "ProductId", "Name");
            return View(shoppingList);
        }

        // POST: ShoppingLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ListId,UserId,Name,IsCompleted")] ShoppingList shoppingList)
        {
            if (id != shoppingList.ListId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirstValue("UserId"));
                    shoppingList.UserId = userId;
                    _context.Update(shoppingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingListExists(shoppingList.ListId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingList);
        }

        // GET: ShoppingLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(m => m.ListId == id && (m.UserId == int.Parse(User.FindFirstValue("UserId")) || m.UserId == null));

            if (shoppingList == null)
            {
                return NotFound();
            }

            return View(shoppingList);
        }

        // POST: ShoppingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingList = await _context.ShoppingLists.FindAsync(id);

            if (shoppingList == null || shoppingList.UserId != int.Parse(User.FindFirstValue("UserId")) && shoppingList.UserId != null)
            {
                return NotFound();
            }

            _context.ShoppingLists.Remove(shoppingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: ShoppingLists/DeleteConfirmed/5
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingLists.FindAsync(id);

            if (shoppingList == null || shoppingList.UserId != int.Parse(User.FindFirstValue("UserId")))
            {
                return NotFound();
            }

            _context.ShoppingLists.Remove(shoppingList);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingListExists(int id)
        {
            return _context.ShoppingLists.Any(e => e.ListId == id);
        }


    }
}
