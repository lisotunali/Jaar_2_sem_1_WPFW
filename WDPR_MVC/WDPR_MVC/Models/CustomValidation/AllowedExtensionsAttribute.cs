using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace WDPR_MVC.Models.CustomValidation
{
    // TODO: If really needed split extensions from signature check
    public class AllowedImageExtensionsAttribute : ValidationAttribute
    {
        // List of permitted extensions
        private string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public AllowedImageExtensionsAttribute() : base("File extension is not supported") { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Don't validate if null
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var file = (IFormFile)value;

            // Get the uploaded file extension
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Check if the extension is valid
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return new ValidationResult(ErrorMessage);
            }

            // Check if file signature matches extension
            if (!hasValidFileSignature(file))
            {
                return new ValidationResult("File signature is invalid");
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

        // Image signatures we want to support
        private static readonly Dictionary<string, List<byte[]>> _fileSignature =
            new Dictionary<string, List<byte[]>>
        {
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            },
            { ".png", new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
                }
            },
            { ".gif", new List<byte[]>
                {
                    new byte[] { 0x47, 0x49, 0x46, 0x38 },
                }
            },
        };

        // Checks the file signature 
        // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads#validation
        private bool hasValidFileSignature(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var signatures = _fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
