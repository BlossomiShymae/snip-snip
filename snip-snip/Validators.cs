using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace snip_snip.Validators
{
    public class CommunityDragonUrlValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string url)
            {
                if (string.IsNullOrEmpty(url))
                    return new ValidationResult("The URL must have a value");
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    return new ValidationResult($"The URL must be a valid URI string");

                var uri = new Uri(url);
                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    return new ValidationResult($"The URL must be using http/https protocol");
                if (!url.Contains("communitydragon.org"))
                    return new ValidationResult($"The URL must be a valid CommunityDragon link");
                if (!url.EndsWith('/'))
                    return new ValidationResult($"The URL must be a directory");

                return ValidationResult.Success;
            }

            return new ValidationResult("The URL must be a string");
        }
    }
}