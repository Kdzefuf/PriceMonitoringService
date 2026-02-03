namespace PriceMonitorService.Models
{
    public class SubscriptionRequest
    {
        public string ListingUrl { get; set; }
        public string Email { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ListingUrl) || string.IsNullOrWhiteSpace(Email))
                return false;

            try
            {
                var uri = new Uri(ListingUrl);
                return uri.Host.Contains("prinzip.su", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public string GetApartmentId()
        {
            try
            {
                var uri = new Uri(ListingUrl);
                var segments = uri.Segments;

                for (int i = segments.Length - 1; i >= 0; i--)
                {
                    var segment = segments[i].Trim('/');
                    if (int.TryParse(segment, out _))
                    {
                        return segment;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}