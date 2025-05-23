﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
    public class CategoryRequestDTO
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string? CategoryImageUrl { get; set; }
        public bool Status { get; set; }
    }
}
