using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Component.OIDC
{
    public class TokenResponseError
    {
        public string error { get; set; }

        public string error_description { get; set; }
    }
}