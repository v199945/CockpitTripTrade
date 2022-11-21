using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Component.OIDC
{
    /// <summary>
    /// A successful response to this token request
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// OAuth 2.0 Access Token
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// OAuth 2.0 Token Type value
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// This field is only present if access_type=offline is included in the authentication request
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// Expiration time of the Access Token in seconds since the response was generated.
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// ID Token
        /// </summary>
        public string id_token { get; set; }
    }
}