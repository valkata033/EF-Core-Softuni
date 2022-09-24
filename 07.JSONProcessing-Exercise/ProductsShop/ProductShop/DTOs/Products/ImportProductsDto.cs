using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.DTOs.Products
{
    [JsonObject]
    public class ImportProductsDto
    {
        [JsonProperty("Name")]
        [Required]
        [MaxLength(3)]
        public string Name { get; set; }

        [JsonProperty("Price")]
        [Required]
        public decimal Price { get; set; }

        [JsonProperty("SellerId")]
        [Required]
        public int SellerId { get; set; }

        [JsonProperty("BuyerId")]
        public int? BuyerId { get; set; }
    }
}
