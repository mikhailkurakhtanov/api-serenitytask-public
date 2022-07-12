using System.Collections.Generic;
using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Client;

namespace SerenityTask.API.Models.Responses.Session
{
    public class GetFriendsAndSessionRequestsResponse
    {
        [JsonProperty(PropertyName = "friendsInfo")]
        public ICollection<FriendInfo> FriendsInfo { get; set; }

        [JsonProperty(PropertyName = "sessionRequests")]
        public ICollection<List<SessionRequest>> SessionRequests { get; set; }
    }
}