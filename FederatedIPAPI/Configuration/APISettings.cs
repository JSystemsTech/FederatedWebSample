using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FederatedIPAPI.Configuration
{
    public interface IAPISettings
    {
        string ConfirmationMethod { get;  }

        string Namespace { get;  }

        string SubjectName { get;  }

        Uri AudienceUri { get;  }

        string Issuer { get; }
        int DefaultValidFor { get;  }
        string ApiUser { get; }
        string ApiPassword { get; }
    }
    public class APISettings: IAPISettings
    {
        public string ConfirmationMethod { get; set; }

        public string Namespace { get; set; }

        public string SubjectName { get; set; }

        public Uri AudienceUri { get; set; }

        public string Issuer { get; set; }
        public int DefaultValidFor { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
        public string EncryptionKey { get; set; }
        
    }
}
