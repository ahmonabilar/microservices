using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace microservice.identity.client.Pages
{
    [Authorize]
    public class SecureModel : PageModel
    {
        public void OnGet() { }
    }
}
