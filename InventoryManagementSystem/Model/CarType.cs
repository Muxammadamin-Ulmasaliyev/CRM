﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Model
{
    public class CarType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }

    }
}
