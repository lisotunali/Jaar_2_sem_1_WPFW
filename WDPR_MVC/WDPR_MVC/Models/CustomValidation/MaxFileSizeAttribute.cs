using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace WDPR_MVC.Models.CustomValidation
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        public long MaximumSize { get; }

        public MaxFileSizeAttribute(long maximumSize)
        {
            MaximumSize = maximumSize;
        }

        public override bool IsValid(object value)
        {
            // Don't validate if null
            if (value == null)
            {
                return true;
            }

            var file = (IFormFile)value;

            // If file larger than max size
            if (file.Length > MaximumSize)
            {
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaximumSize);
        }
    }
}
