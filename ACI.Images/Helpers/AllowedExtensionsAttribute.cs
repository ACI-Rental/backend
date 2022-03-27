using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ACI.ImageService.Helpers
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly List<string> _extensions;
        public AllowedExtensionsAttribute(string extensions)
        {
            _extensions = extensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    
        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
        
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"This extension is not allowed";
        }
    }
}