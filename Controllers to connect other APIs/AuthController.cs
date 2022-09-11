using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExcelicareAPIGateWay.Models;
using System.Security.Cryptography;
using System.Net.Http;
using ExcelicareAPIGateWay.Filters;
using System.Web.Http.Description;

namespace ExcelicareAPIGateWay.Controllers
{
    /// <summary>
    /// Authentication controller to register user and generate JWT token
    /// Created By  : BRR
    /// Created Date: 01-07-2022
    /// </summary>
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class AuthController : ApiController
    {

        //public static User user = new User();
        //public UserDto userObj = new UserDto();
        /// <summary>
        ///  UserModel object to access authentication methods
        /// </summary>
        public UserModel usermodelObj= new UserModel();     
        /// <summary>
        /// Authenticating user credentials and generating JWT Token
        /// </summary>
        /// <param name="request">mandatory</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/register")]
        [RequestValidation]
        public IHttpActionResult Login(UserDto request)
        {
            if (request.Username == "")
            {
                return BadRequest("User not found.");
            }

            if (!usermodelObj.AuthenticateUser(request.Username,request.Password))
            {
                return BadRequest("Wrong credentials.");
            }

            //string token = CreateToken(request);
            string token = TokenManager.GenerateToken(request.Username,request.resource);
            //usermodelObj.SaveToken(token);
            return Ok(token);

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IHttpActionResult GenerateToken(UserDto request)
        //{    


        //    if (request.Username == "" || request.Password == "")
        //    {
        //        return BadRequest("Please provide required fields.");
        //    }

        //    if (!usermodelObj.AuthenticateUser(request.Username,request.Password))
        //    {
        //        return BadRequest("Wrong credentials.");
        //    }

        //    string token = CreateToken11(request);

        //    return Ok(token);

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>

        //[HttpPost]   
        //public IHttpActionResult Register_Old(UserDto request)
        //{

        //    CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
        //    user.Username = request.Username;
        //    user.PasswordHash = passwordHash;
        //    user.PasswordSalt = passwordSalt;

        //    return Ok(user);
        //}

        ///// <summary>
        ///// Testing ActionFilters
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/test")]
        //[TokenAuthenticationFilter]
        //public IHttpActionResult Test_Method()
        //{
        //    return Ok("Successfully valid");
        //}
        //private string CreateToken11(UserDto user)
        //{
        //    List<Claim> claims = new List<Claim>
        //    {
                              
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("yyyy-MM-dd")),
        //        new Claim(ClaimTypes.Email, "admin@excelicare.com"),
        //        new Claim(ClaimTypes.Name,user.Username),
        //        new Claim(ClaimTypes.Role,"Admin"),
        //        new Claim("DateOfRegistered", DateTime.UtcNow.ToString("yyyy-MM-dd"))

        //    };
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Jwt:Key"] + ""));
        //    var signIn = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);          

        //    var token = new JwtSecurityToken(ConfigurationManager.AppSettings["Jwt:Issuer"] + "",
        //                                   ConfigurationManager.AppSettings["Jwt:Issuer"] + "",
        //                                   claims,
        //                                   expires: DateTime.Now.AddDays(1),
        //                                    signingCredentials: signIn);

        //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        //    return jwt;
        //}

        //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //    }
        //}

        //private bool VerifyPaswordHash(string password,byte[] passwordHash,byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512(passwordSalt))
        //    {
        //        var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //        return computeHash.SequenceEqual(passwordHash);
        //    }
        //}
        //        /// <summary>
        ///// 
        ///// </summary>
        ///// <param name="login"></param>
        ///// <returns></returns>
        //public IHttpActionResult Authenticate([FromBody] UserModel login)
        //{

        //    IHttpActionResult response = Unauthorized();
        //    var user = AuthenticateUserPWD(login);

        //    if (user != null)
        //    {
        //        var tokenString = GenerateJSONWebToken(user);
        //        response = Ok(new { token = tokenString });
        //    }

        //    return response;
        //}

        //private string GenerateJSONWebToken(UserModel userInfo)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Jwt:Key"]+""));
        //    var signIn = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);       

        //    var claims = new[] {
        //        new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("yyyy-MM-dd")),
        //        new Claim(ClaimTypes.Email, userInfo.EmailAddress),
        //        new Claim(ClaimTypes.Name,userInfo.Username),
        //        new Claim(ClaimTypes.Role,"Admin"),
        //        new Claim("DateOfRegistered", userInfo.DateOfRegistered.ToString("yyyy-MM-dd"))
        //    };

        //    var token = new JwtSecurityToken(ConfigurationManager.AppSettings["Jwt:Issuer"]+"",
        //                                     ConfigurationManager.AppSettings["Jwt:Issuer"]+"",
        //                                     claims,
        //                                     expires: DateTime.Now.AddMinutes(60),
        //                                      signingCredentials: signIn);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

       
        //private UserModel AuthenticateUserPWD(UserModel login)
        //{
        //    UserModel user = null;

        //    //Validate the User Credentials           
        //    if (login.Username == "Telehealth")
        //    {
        //        user = new UserModel { Username = "Bhumpalli RamanaReddy", EmailAddress = "ramana.bhumpalli@excelicare.com", DateOfRegistered = new DateTime() };
        //    }
        //    return user;
        //}



    }
}
