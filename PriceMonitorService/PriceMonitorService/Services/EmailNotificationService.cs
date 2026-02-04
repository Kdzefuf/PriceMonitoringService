using MailKit.Net.Smtp;
using MimeKit;

namespace PriceMonitorService.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IConfiguration _configuration;

        public EmailNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPriceChangeNotificationAsync(
            string email,
            string listingUrl,
            decimal oldPrice,
            decimal newPrice)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Price Monitor", _configuration["Email:From"]));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Изменение цены на квартиру";

                var priceChange = newPrice - oldPrice;
                var priceChangePercent = (priceChange / oldPrice) * 100;
                var priceChangeDirection = priceChange > 0 ? "выросла" : "снизилась";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                    <html>
                    <body>
                        <h2>Изменение цены на квартиру</h2>
                        <p>Цена на квартиру по ссылке <a href='{listingUrl}'>{listingUrl}</a> {priceChangeDirection}.</p>
                        <p><strong>Старая цена:</strong> {oldPrice:N0} ₽</p>
                        <p><strong>Новая цена:</strong> {newPrice:N0} ₽</p>
                        <p><strong>Изменение:</strong> {Math.Abs(priceChange):N0} ₽ ({Math.Abs(priceChangePercent):F2}%)</p>
                        <p>Дата проверки: {DateTime.Now}</p>
                    </body>
                    </html>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                Console.WriteLine($"Подключение к SMTP: {_configuration["Email:SmtpServer"]}:" +
                    $"{_configuration["Email:SmtpPort"]}");

                await client.ConnectAsync(_configuration["Email:SmtpServer"],
                    int.Parse(_configuration["Email:SmtpPort"]), MailKit.Security.SecureSocketOptions.None);

                Console.WriteLine($"Отправка письма на: {email}");
                await client.SendAsync(message);
                Console.WriteLine("Письмо успешно отправлено!");

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}