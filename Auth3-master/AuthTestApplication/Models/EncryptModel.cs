using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthTestApplication.Models
{
    public class EncryptModel
    {
        public string UserText { get; set; }

        public string Encrypted { get; set; }

        public string Decrypted { get; set; }
    }
}