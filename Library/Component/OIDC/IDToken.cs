using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Component.OIDC
{
    /// <summary>
    /// A JWT that contains identity information about the user
    /// </summary>
    public class IDToken
    {
        /// <summary>
        /// Issuer Identifier 
        /// </summary>
        public string iss { get; set; }

        /// <summary>
        /// Subject Identifier: ID
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string aud { get; set; }

        /// <summary>
        /// String value used to associate a Client session with an ID Token, and to mitigate replay attack
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// The time the ID token expires
        /// </summary>
        public string exp { get; set; }

        /// <summary>
        /// The time the ID token was issued
        /// </summary>
        public string iat { get; set; }

        /// <summary>
        /// Access token hash:　Provides validation that the access token is tied to the identity token.
        /// </summary>
        public string at_hash { get; set; }

        /// <summary>
        /// ID Group: e.g. CI、AE
        /// </summary>
        public string[] group { get; set; }
    }
}