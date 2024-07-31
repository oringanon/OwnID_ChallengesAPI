namespace OwnID_ChallengesAPI.Models.Config
{
    public class EmailConfiguration : INotificationConfiguration
    {
        public int AppId { get; set; }
        public string SMTPServerURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }

    }
}
