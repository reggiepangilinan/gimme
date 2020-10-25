using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Gimme.Core.Validators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GimmeSettingsFileMustExistsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (!File.Exists(Constants.GIMME_SETTINGS_FILENAME))
            {
                return new ValidationResult("😂 Can't read file gimmeSettings.json. Make sure you've initialized gimme or you're in a correct working directory."); ;
            }
            return ValidationResult.Success;
        }
    }
}
