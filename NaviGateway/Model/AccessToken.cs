using System;

namespace NaviGateway.Model
{
    public class AccessToken
    {
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Token { get; set; }
    }
}