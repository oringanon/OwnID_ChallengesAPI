using OwnID_ChallengesAPI.Models;
using System.Collections.Concurrent;

namespace OwnID_ChallengesAPI.ChallengesRepository
{
    public class OtpHandler
    {
        private readonly ConcurrentDictionary<string, OtpRecord> _availbleOtps;
        private readonly ConcurrentDictionary<string, object> _locks;

        public OtpHandler()
        {
            _availbleOtps = new ConcurrentDictionary<string, OtpRecord>();
            _locks = new ConcurrentDictionary<string, object>();
        }

        public bool AddOtpRecord(string appId, string challengeId, string otp)
        {
            return _availbleOtps.TryAdd(createKey(appId, challengeId), new OtpRecord(otp, 0));
        }

        public OtpRecord GetOtpRecord(string appId, string challengeId)
        {
            var result = _availbleOtps.TryGetValue(createKey(appId, challengeId), out OtpRecord otpRecord);
            return result ? otpRecord : null;
        }

        public void UpdateOtpCounter(string appId, string challengeId)
        {
            var key = createKey(appId, challengeId);
            lock (GetLockObject(key))
            {
                if (_availbleOtps.TryGetValue(key, out var otpRecord))
                {
                    otpRecord.AttemptsCounter++;
                }
            }
        }

        public bool DeleteOtpRecord(string appId, string challengeId)
        {
            return _availbleOtps.TryRemove(createKey(appId, challengeId), out _);
        }

        public TResult ExecuteAtomically<TResult>(string appId, string challengeId, Func<OtpRecord, TResult> operation)
        {
            var key = createKey(appId, challengeId);
            lock (GetLockObject(key))
            {
                if (_availbleOtps.TryGetValue(key, out var otpRecord))
                {
                    return operation(otpRecord);
                }
                //I would add log here with more info
                throw new KeyNotFoundException($"No OTP record found for appId {appId} and challengeId {challengeId}");
            }
        }

        public object GetLockObject(string key)
        {
            return _locks.GetOrAdd(key, _ => new object());
        }

        public string createKey(string prefix, string suffix)
        {
            return $"{prefix}_{suffix}";
        }
    }
}

