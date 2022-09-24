using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs.CategoryProducts
{
    public class ImportCategoriesProductsDto
    {
        [JsonProperty("CategoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("ProductId")]
        public int ProductId { get; set; }
    }
}
