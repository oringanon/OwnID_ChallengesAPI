using System.Text.Json;
using System.Text;
using OwnID_ChallengesAPI.Models.Config;

namespace OwnID_ChallengesAPI.ChallengesServices.Notifications
{
    public class NotificationsService
    {
        private readonly HttpClient _httpClient;
        const string WEBHOOK_URL = "https://webhook.site/60638b33-7db0-428a-90d3-064138f32156";
        public NotificationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SendSMS(SMSConfiguration smsConfiguration, string otp)
        {
            var payload = new { otp };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(WEBHOOK_URL, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending OTP: {ex.Message}");
                return false;
            }
        }

        // Assuming that in our case there is no need to actually send Emails\SMS - I didnt used the configuration
        // data and didnt handled separately these two actions - in this point no need to have code that is not really 
        // in use 
        public async Task<bool> Send<T>(INotificationConfiguration notificationConfiguration, string otp)
        {
            var payload = new { otp };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(WEBHOOK_URL, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending OTP: {ex.Message}");
                return false;
            }
        }
    }
}
