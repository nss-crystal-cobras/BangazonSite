﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProductViewModels
{
    public class ProductCreateViewModel
    {

        public Product Product { get; set; }

        // Make a select list item for productTypes for dropdown
        public List<SelectListItem> ProductTypes { get; set; }
    }
}
