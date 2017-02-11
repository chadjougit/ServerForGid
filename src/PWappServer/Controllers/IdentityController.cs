using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PWappServer.Hubs;
using PWappServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

//using Microsoft.Data.Entity;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PWappServer.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private TestMessageHandler _notificationsMessageHandler { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public IdentityController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            ApplicationDbContext context, TestMessageHandler notificationsMessageHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<IdentityController>();
            _context = context;
            _notificationsMessageHandler = notificationsMessageHandler;
        }

        /// <summary>
        /// return current user's amount and his transaction.
        /// when user start his session, we using WebsocketId to write to session list information about this session
        /// (his username and WebsocketId)
        /// </summary>
        /// <param name="WebsocketId"></param>
        /// <returns></returns>
        [HttpPost("GetCurrentUserData")]
        public async Task<IActionResult> GetCurrentUserData([FromBody]string WebsocketId)
        {
            try
            {
                var currentuser = await GetCurrentUserAsync();
                //in case of logging

                //TODO: != null?
                //we writing to websocketsessions only when user start his session
                if (WebsocketId != "empty")
                {
                    ConnectionInfo newConnectionInfo = new ConnectionInfo() { UserName = currentuser.UserName, ConnectionId = WebsocketId };
                    WebSocketSessions.Session_Start(newConnectionInfo);
                }

                return Userdata(currentuser);
            }
            //TODO: log all exceptions
            catch (Exception ex)

            { return new JsonResult(ex) { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError }; }
        }

        private IActionResult Userdata(ApplicationUser currentuser)
        {
            List<Transaction> transactions = (_context.Transactions.Where(u => (currentuser.UserName == u.SenderUsername) || (currentuser.UserName == u.RecipientUsername))).ToList();
            transactions = transactions.Select(c => { if (currentuser.UserName == c.SenderUsername) { c.Amount = c.Amount * -1; }; return c; }).ToList();

            CurrentUserData currentUserData = new CurrentUserData() { UserPw = currentuser.PW, UserTransactions = transactions };

            var result = JsonConvert.SerializeObject(currentUserData);

            //return new JsonResult("error vlad test") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
            return new JsonResult(result) { StatusCode = (int)System.Net.HttpStatusCode.OK };
        }

        /// <summary>
        /// return al users with "user" claim 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// send transaction to selected user, send him message by his websocketId
        /// return user's data
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendTransactionToUser")]
        public async Task<IActionResult> SendTransactionToUser([FromBody]string[] values)
        {
            var Currentuser = await GetCurrentUserAsync();
            var username = values[0];
            int summ = 0;
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            bool res = int.TryParse(values[1], out summ);
            if (res == false || user == null || Currentuser.PW - summ <= 0)
            {
                return new JsonResult("wrong input data") { StatusCode = (int)System.Net.HttpStatusCode.BadRequest };
            }

            //var trans = _context.Transactions.Where(u => u.ApplicationUserId == Currentuser.Id);

            Currentuser.PW = Currentuser.PW - summ;
            user.PW = user.PW + summ;
            _context.Transactions.Add(new Transaction() { Amount = summ, SenderUsername = Currentuser.UserName, RecipientUsername = user.UserName, Date = DateTime.UtcNow });
            //     Currentuser.Transactions.Add(new Transaction() { Amount = summ, ApplicationUserId = Currentuser.Id, ApplicationUserId2 = user.Id });

            await _userManager.UpdateAsync(user);
            await _userManager.UpdateAsync(Currentuser);

            var reciveirIds = WebSocketSessions.Sessions.Where(u => u.UserName == user.UserName);

            foreach (var recivierId in reciveirIds)
            {
                await _notificationsMessageHandler.SendMessageToId(recivierId.ConnectionId, "new transaction alert");
            }

            
            _context.SaveChanges();
            /*
            _userManager.Dispose();
            _context.Dispose();
            */


           
            return Userdata(Currentuser);
           // return new JsonResult("OK") { StatusCode = (int)System.Net.HttpStatusCode.OK };
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
            else
            {
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
            _userManager.Dispose();
            _context.Dispose();
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