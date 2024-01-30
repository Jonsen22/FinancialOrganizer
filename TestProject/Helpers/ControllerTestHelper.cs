using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Helper
{
    public static class ControllerTestHelper
    {
        public static void SetAuthenticatedUser(ControllerBase controller, string userId)
        {
            var identity = new GenericIdentity("testuser");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

            var principal = new GenericPrincipal(identity, roles: null);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

    }
}
