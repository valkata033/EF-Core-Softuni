using Newtonsoft.Json;
using ProductShop.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs.User
{
    public class ExportUsersWithSoldProductsDto
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string  LastName { get; set; }

        [JsonProperty("soldProducts")]
        public ExportSoldProductsDto[] SoldProducts { get; set; }
    }
}
