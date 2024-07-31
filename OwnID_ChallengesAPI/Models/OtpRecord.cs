namespace OwnID_ChallengesAPI.Models
{
    public class OtpRecord
    {
        public OtpRecord(string otp, int attemptsCounter)
        {
            this.Otp = otp;
            this.AttemptsCounter = attemptsCounter;
        }

        public string Otp { get; set; }
        public int AttemptsCounter { get; set; }
    }
}
