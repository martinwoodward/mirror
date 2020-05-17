using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using msftghes.Services.MicrosoftGraph;
using Constants = msftghes.Infrastructure.Constants;

namespace msftghes.Controllers
{
    // [Authorize(Roles = "a9cccbf0-6fb5-41ec-a45b-e76e57fd241a")] // Using groups ids in the Authorize attribute
    public class UserProfileController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IMSGraphService graphService;

        public UserProfileController(ITokenAcquisition tokenAcquisition, IMSGraphService MSGraphService)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.graphService = MSGraphService;
        }

        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead, Constants.ScopeDirectoryReadAll })]        
        public async Task<IActionResult> Index()
        {
            // Using group ids/names in the IsInRole method
            // var isinrole = User.IsInRole("a9cccbf0-6fb5-41ec-a45b-e76e57fd241a");

            ViewData["User"] = HttpContext.User;

            string accessToken = await tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(new[] { Constants.ScopeUserRead, Constants.ScopeDirectoryReadAll });

            User me = await graphService.GetMeAsync(accessToken);
            ViewData["Me"] = me;

            try
            {
                var photo = await graphService.GetMyPhotoAsync(accessToken);
                ViewData["Photo"] = photo;
            }
            catch
            {
                //swallow
            }
            
            IList<Group> groups = await graphService.GetMyMemberOfGroupsAsync(accessToken);            
            ViewData["Groups"] = groups;

            return View();
        }
    }
}