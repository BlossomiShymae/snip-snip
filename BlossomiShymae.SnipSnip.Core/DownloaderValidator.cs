using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace BlossomiShymae.SnipSnip.Core
{
    public class DownloaderValidator : AbstractValidator<Downloader>
    {
        public DownloaderValidator()
        {
            RuleFor(x => x.Url).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("The URL must not be empty");
            RuleFor(x => x.Url).Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute)).WithMessage("The URL must be a valid URI string");
            RuleFor(x => x.Url).Must(IsValidCommunityDragonPath).WithMessage("The URL must be in a valid CommunityDragon path");
        }

        private bool IsValidCommunityDragonPath(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var uri = new Uri(url);
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return false;
            if (!url.Contains("communitydragon.org"))
                return false;
            if (!url.EndsWith('/'))
                return false;

            return true;
        }
    }
}