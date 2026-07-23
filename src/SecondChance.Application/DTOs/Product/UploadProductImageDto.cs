using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Product
{
    public class UploadProductImageDto
    {
        public IFormFile File { get; set; } = null!;
    }
}
