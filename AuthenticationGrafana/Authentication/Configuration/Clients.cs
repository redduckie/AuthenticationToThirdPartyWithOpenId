using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {

                new Client
                {
                    ClientName = "Grafana",
                    ClientId = "clientId1",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris= new List<string>
                    {
                        "http://localhost:3000/login/generic_oauth"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    },
                    RequireConsent = false,
                    ClientSecrets =
                    {
                        new Secret("secret1".Sha256())
                    }
                }
            };

        }
    }
}
