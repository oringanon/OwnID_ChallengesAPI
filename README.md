# OwnID_ChallengesAPI
General Notes:
If i had more time to complete the task - i would use Redis instead of the ConcurrentDictionary that i used for a better and
correct fit for scaleable solution in case we will want to deploy our app on kubernetes on multiple pods, for my code it means    that every instance /pod will have his own ConcurrentDictionary and my implementation using ConcurrentDictionary is not safe.

 i added comments for assumptions that i made, and places i would want to use logs.
