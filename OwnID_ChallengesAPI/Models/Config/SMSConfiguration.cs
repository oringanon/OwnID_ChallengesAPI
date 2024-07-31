namespace OwnID_ChallengesAPI.Models.Config
{
    public class SMSConfiguration : INotificationConfiguration
    {
        public int AppId { get; set; }
        public string SMSServerURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromNumber { get; set; }
    }
}
