using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Identity;

namespace Bangazon.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        //access the currently authenticated user
        private readonly UserManager<ApplicationUser> _userManager;

        //inject the UserManager service in the constructor
        public ProductsController(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        //any method that needs to see who the user is can invoke the method
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Types()
        {
            var model = new ProductTypesViewModel();

            // Build list of Product instances for display in view
            // The LINQ statement groups the student entities by enrollment date, calculates the number of entities in each group, and stores the results in a collection of EnrollmentDateGroup view model objects.
            // LINQ is awesome
            model.GroupedProducts = await (
                from t in _context.ProductType
                join p in _context.Product
                on t.ProductTypeId equals p.ProductTypeId
                group new { t, p } by new { t.ProductTypeId, t.Label } into grouped
                select new GroupedProducts
                {
                    TypeId = grouped.Key.ProductTypeId,
                    TypeName = grouped.Key.Label,
                    ProductCount = grouped.Select(x => x.p.ProductId).Count(),
                    Products = grouped.Select(x => x.p).Take(3)
                }).ToListAsync();

            return View(model);
        }
        
        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Product.Include(p => p.ProductType).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        //Given 1+ users have purchased a product, when a user views the details, 
        //--then the quantity display should show how many are remaining, not how many were originally for sale
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        // JD
        public IActionResult Create()
        {
            // Grab product type from database and create new select list item of productTypes
            var productTypesComplete = _context.ProductType;
            List<SelectListItem> productTypes = new List<SelectListItem>();

            // Insert into list
            productTypes.Insert(0, new SelectListItem
            {
                Text = "Assign a Product Category",
                Value = ""
            });

            foreach (var pt in productTypesComplete)
            {
                // Create a new select list item
                SelectListItem li = new SelectListItem
                {
                    
                    Value = pt.ProductTypeId.ToString(),
                    Text = pt.Label
                };
                // Add to productTypes
                productTypes.Add(li);
            }

         
            ProductCreateViewModel viewModel = new ProductCreateViewModel();

            
            viewModel.ProductTypes = productTypes;

            // View dropdowns
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");

            // Return view of ProductCreateViewModel
            return View(viewModel);
        }


        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel productCreate)
        {

            // If the user is already in the ModelState
            // Remove user from model state
            ModelState.Remove("Product.User");

            // If model state is valid
            if (ModelState.IsValid)
            {
                // Add the user back then add the product finally save changes to database
                productCreate.Product.User = await GetCurrentUserAsync();

                _context.Add(productCreate.Product);

                await _context.SaveChangesAsync();

                // Redirect to details view via the new object
                return RedirectToAction(nameof(Details), new { id = productCreate.Product.ProductId.ToString() });
            }
            // Get data to be displayed in dropdown
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", productCreate.Product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", productCreate.Product.UserId);

      
            var productTypesComplete = _context.ProductType;

            // Create new list
            List<SelectListItem> productTypes = new List<SelectListItem>();

            // Insert into productTypes
            productTypes.Insert(0, new SelectListItem
            {
                Text = "Assign a Product Category",
                Value = ""
            });

            // Loop in order to display list items
            foreach (var pt in productTypesComplete)
            {
                
                SelectListItem li = new SelectListItem
                {
                    
                    Value = pt.ProductTypeId.ToString(),
                    Text = pt.Label
                };
                // Add to productTypes 
                productTypes.Add(li);
            }

            // put all current data into product create.productTypes in order to return
            productCreate.ProductTypes = productTypes;

            
            return View(productCreate);
        }



        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,ProductTypeId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }

    }
}
