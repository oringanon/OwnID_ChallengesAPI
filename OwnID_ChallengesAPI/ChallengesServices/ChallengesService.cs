using OwnID_ChallengesAPI.ChallengesRepository;
using OwnID_ChallengesAPI.ChallengesServices.Notifications;
using OwnID_ChallengesAPI.Models.Config;
using OwnID_ChallengesAPI.Models.Requests;
using OwnID_ChallengesAPI.Models.Responses;
using System.Security.Cryptography;

namespace OwnID_ChallengesAPI.ChallengesServices
{
    public class ChallengesService
    {
        const int MAX_AUTH_ATTEMPTS = 2;
        readonly ConfigurationRepository _configurationRepository;
        readonly NotificationsService _notificationsService;
        readonly OtpHandler _otpHandler;
        private readonly Object _lockObj = new Object();
        //private readonly Fido2 _fido2;
        public ChallengesService(ConfigurationRepository configurationRepository, NotificationsService notificationsService, OtpHandler otpHandler)
        {
            _configurationRepository = configurationRepository;
            _notificationsService = notificationsService;
            _otpHandler = otpHandler;
            //_fido2 = fido2;
        }

        public string GenerateOtp(int length = 8)
        {
            if (length <= 0) throw new ArgumentException("Length must be a positive integer", nameof(length));

            const string validCharacters = "0123456789";
            var otp = new char[length];
            var bytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            for (int i = 0; i < length; i++)
            {
                otp[i] = validCharacters[bytes[i] % validCharacters.Length];
            }

            return new string(otp);
        }

        public ChallengeResponse StartChallenge(string appId, ChallengeRequest challengesRequest)
        {
            ChallengeResponse response = new ChallengeResponse();
            if (string.IsNullOrEmpty(appId) || (string.IsNullOrEmpty(challengesRequest.Email) && string.IsNullOrEmpty(challengesRequest.PhoneNumber)))
            {
                throw new Exception("Bad request - Invalid request parameters");
            }

            try
            {
                var challengeId = Guid.NewGuid().ToString();
                response.ChallengeId = challengeId;
                var otp = GenerateOtp();
                INotificationConfiguration requestedConfiguration = string.IsNullOrEmpty(challengesRequest.Email) ?
                    _configurationRepository.GetConfiguration<SMSConfiguration>(appId) :
                    _configurationRepository.GetConfiguration<EmailConfiguration>(appId);

                if (requestedConfiguration == null)
                {
                    //I would use logger -> "no records found for {appId}" ...
                    throw new Exception($"Bad request - The given AppId - {appId} is not valid");
                }

                var res = _notificationsService.Send<INotificationConfiguration>(requestedConfiguration, otp).Result;
                if (!res)
                {
                    //I would use logger in order to document the actual reason for failure -
                    //but no need to expouse detailed technical error descriptions about the reason for failure  
                    throw new Exception($"Bad request - Operation failed for {appId}");
                }
                //save
                var addOtpRes = _otpHandler.AddOtpRecord(appId, challengeId, otp);
                if (!addOtpRes)
                {
                    //I would use logger -> Failed to save OTP
                    throw new Exception($"Bad request - Operation failed for {appId}");
                }
            }
            catch (Exception ex)
            {
                //I would use logger -> ex.Message
                throw new Exception($"Bad request - Operation failed");
            }
            return response;
        }

        public bool Solve(string appId, string challengeId, string otp)
        {

            lock (_otpHandler.GetLockObject(_otpHandler.createKey(appId, challengeId)))
            {
                var otpRecord = _otpHandler.GetOtpRecord(appId, challengeId);
                if (otpRecord == null)
                {
                    //I would add log here with more info
                    throw new KeyNotFoundException("Challenge not found");
                }

                if (!otp.Equals(otpRecord.Otp))
                {
                    if (otpRecord.AttemptsCounter >= MAX_AUTH_ATTEMPTS)
                    {
                        _otpHandler.DeleteOtpRecord(appId, challengeId);
                        //I would add log here with more info
                        throw new UnauthorizedAccessException("Authentication failed - Too many attempts");
                    }
                    _otpHandler.UpdateOtpCounter(appId, challengeId);
                    //I would add log here with more info
                    throw new UnauthorizedAccessException("Authentication failed - Wrong OTP");
                }
                // success
                if (!_otpHandler.DeleteOtpRecord(appId, challengeId))
                {
                    throw new Exception("Server Error");
                }
            }

            return true;

        }

        /*public AssertionOptions StartFidoAuthentication(string username)
        {
            var user = new Fido2User
            {
                Name = username,
                Id = Encoding.UTF8.GetBytes(username)
            };

            var options = _fido2.GetAssertionOptions(new List<PublicKeyCredentialDescriptor>(), userVerification: UserVerificationRequirement.Required);
            return options;
        }*/

        /*public async Task<AssertionVerificationResult> CompleteFidoAuthentication(AuthenticatorAssertionRawResponse assertionResponse, AssertionOptions options, List<PublicKeyCredentialDescriptor> credentials)
        {
            var result = await _fido2.MakeAssertionAsync(assertionResponse, options, credentials, (args) => Task.FromResult(true));
            return result;
        }*/
    }
}
