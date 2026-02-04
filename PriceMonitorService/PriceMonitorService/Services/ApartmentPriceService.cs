using PriceMonitorService.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PriceMonitorService.Services
{
    public class ApartmentPriceService : IApartmentPriceService
    {
        private readonly HttpClient _httpClient;

        public ApartmentPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<decimal?> GetPriceFromListingAsync(string listingUrl)
        {
            try
            {
                var apartmentId = ExtractApartmentIdFromUrl(listingUrl);

                if (string.IsNullOrEmpty(apartmentId))
                {
                    Console.WriteLine($"Не удалось получить id квартиры из URL: {listingUrl}");
                    return null;
                }

                var apiUrl = $"https://prinzip.su/api/v1/public/apartments/{apartmentId}";

                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API запрос не выполнен: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var apartmentData = JsonSerializer.Deserialize<ApartmentApiResponse>(jsonResponse, options);


                if (apartmentData?.Pricings == null || apartmentData.Pricings.Count == 0)
                {
                    Console.WriteLine($"Не найдены данные цены для квартиры {apartmentId}");
                    return null;
                }

                var pricing = apartmentData.Pricings.FirstOrDefault();

                if (pricing == null)
                {
                    Console.WriteLine($"Не найдена действительная цена для квартиры {apartmentId}");
                    return null;
                }

                if (pricing?.Price == null)
                {
                    Console.WriteLine($"Цена не найдена для квартиры {apartmentId}");
                    return null;
                }

                decimal price = pricing.Price.Value;
                Console.WriteLine($"Цена для квартиры {apartmentId}: {price:N0} ₽");
                return price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении цены из API для {listingUrl}: {ex.Message}");
                Console.WriteLine($"Детали: {ex}");
                return null;
            }
        }

        private static string? ExtractApartmentIdFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при извлечении id квартиры из URL: {ex.Message}");
                return null;
            }
        }
    }
}