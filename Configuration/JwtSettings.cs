﻿namespace EtaxService.Configuration
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Expire { get; set; }
    }
}
