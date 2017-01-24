using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PWappServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using IdentityModel;
using Newtonsoft.Json;
//using Microsoft.Data.Entity;




// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PWappServer.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
 
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public IdentityController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<IdentityController>();
            _context = context;
        }

        /// <summary>
        /// Gets all the users (user role).
        /// </summary>
        /// <returns>Returns all the users</returns>
        // GET api/identity/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var claim = new Claim("role", "administrator");

            var user = await GetCurrentUserAsync();

            var users = await _userManager.GetUsersForClaimAsync(claim);

            return new JsonResult(user);
        }


        [HttpGet("GetTransactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var currentuser = await GetCurrentUserAsync();

            List<Transaction> trans =  (_context.Transactions.Where(u => (currentuser.UserName == u.SenderUsername) || (currentuser.UserName == u.RecipientUsername) )).ToList() ;

            //   trans.ForEach(x => );
            //   trans = trans.ForEach(x => 500); 


            var qry = from e in trans
                      let qryq = (e.Amount = 500)
                      select qryq;

            trans = trans.Select(c => { if (currentuser.UserName == c.SenderUsername) { c.Amount = c.Amount * -1; }; return c; }).ToList();

        //    trans = trans.Where(u => (currentuser.Id == u.ApplicationUserId)).Select(c => { c.Amount = c.Amount * -1; return c; }).ToList();

            //Where(u => (currentuser.Id == u.ApplicationUserId));


            var res = JsonConvert.SerializeObject(trans);




            return new JsonResult(res);
        }

      

        [HttpGet("GetTime")]
        public async Task<IActionResult> GetTime()
        {
            

            var currentuser = await GetCurrentUserAsync();
            StaticClass.Session_Start(currentuser.UserName);




            DateTime saveUtcNow = DateTime.UtcNow;

            var utc = DateTime.UtcNow;


            return new JsonResult(saveUtcNow);
        }


        [HttpGet("GetAmount")]
        public async Task<IActionResult> GetAmount()
        {


            var currentuser = await GetCurrentUserAsync();



            return new JsonResult(currentuser.PW);
        }







        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var claim = new Claim("role", "user");

            var currentuser = await GetCurrentUserAsync();

            var users = await _userManager.GetUsersForClaimAsync(claim);

            List<string> userresult = new List<string>();

            foreach (var user in users)
            {
                if (currentuser.UserName != user.UserName)
                userresult.Add(user.UserName);
            }


            return new JsonResult(userresult);
        }

        [HttpPost("FindByName")]
        public async Task<IActionResult> FindByName([FromBody]string[] values)
        {

            var Currentuser = await GetCurrentUserAsync();

           // var test = Currentuser.Transactions;

          //  var test2 = Currentuser.Transaction2;
            var username = values[0];
            var summ = Convert.ToInt32(values[1]);

            var user = await _userManager.FindByNameAsync(username);

            //var trans = _context.Transactions.Where(u => u.ApplicationUserId == Currentuser.Id);

            Currentuser.PW = Currentuser.PW - summ;
            user.PW = user.PW + summ;
            _context.Transactions.Add(new Transaction() { Amount = summ, SenderUsername = Currentuser.UserName, RecipientUsername = user.UserName, Date = DateTime.UtcNow });
            //     Currentuser.Transactions.Add(new Transaction() { Amount = summ, ApplicationUserId = Currentuser.Id, ApplicationUserId2 = user.Id });


            await _userManager.UpdateAsync(user);
            await _userManager.UpdateAsync(Currentuser);





           
            _context.SaveChanges();
            _userManager.Dispose();
            _context.Dispose();
            return new JsonResult(user);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <returns>IdentityResult</returns>

        
    // POST: api/identity/Create
    [HttpPost("Create")]
        // [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody]CreateViewModel model)
   //     public async Task<IActionResult> Create([FromBody]string[] values)
        {

            var userExist = await _userManager.FindByNameAsync(model.username);
            IdentityResult result;
            if (userExist != null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = "UserExist", Description = "User already exist" });
            }
            else {


                var user = new ApplicationUser
                {
                    UserName = model.username,
                    Email = model.username,
                    Name = model.name,
                    PW = 500
                };



                // Claims.
                var claims = new IdentityUserClaim<string>[] {
            new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = "user" }
            // Add new claims, for example other roles.
        };
                foreach (var claim in claims)
                {
                    user.Claims.Add(claim);
                }

                result = await _userManager.CreateAsync(user, model.password);

                // Option: enable account confirmation and password reset.
            }


            return new JsonResult(result);
    }
    

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <returns>IdentityResult</returns>
        // POST: api/identity/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody]string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.DeleteAsync(user);

            return new JsonResult(result);
        }

        // Add other methods.
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

    }


}
