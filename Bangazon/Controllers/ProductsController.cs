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
        public IActionResult Create()
        {
            // Grab product type from database
            var productTypesComplete = _context.ProductType;

            // Create new select list item of productTypes
            List<SelectListItem> productTypes = new List<SelectListItem>();

            // Insert new position into productTypes list
            productTypes.Insert(0, new SelectListItem
            {
                Text = "Assign a Product Category",
                Value = ""
            });

            // Loop over product types in database
            foreach (var pt in productTypesComplete)
            {
                // Create a new select list item of li
                SelectListItem li = new SelectListItem
                {
                    // Give a value to li
                    Value = pt.ProductTypeId.ToString(),
                    // Provide text to li
                    Text = pt.Label
                };
                // Add li to productTypes select list item
                productTypes.Add(li);
            }

            // Create instance of viewModel for Product Create
            ProductCreateViewModel viewModel = new ProductCreateViewModel();

            // Assign productTypes select list item to the product Types in ProductCreateViewModel
            viewModel.ProductTypes = productTypes;

            // View Data for dropdowns
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
                // Add the user back
                productCreate.Product.User = await GetCurrentUserAsync();

                // Add the product
                _context.Add(productCreate.Product);

                // Save changes to database
                await _context.SaveChangesAsync();

                // Redirect to details view with id of product made using new object
                return RedirectToAction(nameof(Details), new { id = productCreate.Product.ProductId.ToString() });
            }
            // Get data from ProductTypeId to be displayed in dropdown
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", productCreate.Product.ProductTypeId);

            //Get data from UserId to be displayed in dropdown
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", productCreate.Product.UserId);

            // Grab product type from database
            var productTypesComplete = _context.ProductType;

            // Create new select list item of productTypes
            List<SelectListItem> productTypes = new List<SelectListItem>();

            // Insert new position into productTypes list
            productTypes.Insert(0, new SelectListItem
            {
                Text = "Assign a Product Category",
                Value = ""
            });

            // Loop over product types in database
            foreach (var pt in productTypesComplete)
            {
                // Create a new select list item of li
                SelectListItem li = new SelectListItem
                {
                    // Give a value to li
                    Value = pt.ProductTypeId.ToString(),
                    // Provide text to li
                    Text = pt.Label
                };
                // Add li to productTypes select list item
                productTypes.Add(li);
            }

            // Make productTypes in ProductCreateViewModel equal to productTypes
            productCreate.ProductTypes = productTypes;

            // Return product view
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
