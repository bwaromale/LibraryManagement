using AutoMapper;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly IUser _userRepo;
        private readonly IEmail _emailService;

        //private string secretKey;
        public UsersController(IMapper mapper, IUser userRepo, IEmail emailService)
        {
            
            _mapper = mapper;
            this._response = new APIResponse();
            _userRepo = userRepo;
            _emailService = emailService;
            
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetUsers()
        {
            try
            {
                IEnumerable<User> users = await _userRepo.GetAllAsync();
                var map = _mapper.Map<IEnumerable<UserResponseDto>>(users);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = map;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.Message.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPost("register")]
        public async Task<ActionResult<APIResponse>> RegisterUser([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if(registerDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Invalid input"};
                    return BadRequest(_response);
                }
                //var map = _mapper.Map<User>(registerDTO);
                var userExist =  _userRepo.GetAsync(u => u.UserName == registerDTO.UserName);
                if(userExist != null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { $"'{registerDTO.UserName}' already exist. Choose a different username" };
                    return BadRequest(_response);
                }
                string hashPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);
                User newUser = new User()
                {
                    UserName = registerDTO.UserName,
                    Password = hashPassword,
                    Email = registerDTO.Email,
                    FirstName = registerDTO.FirstName,
                    LastName = registerDTO.LastName,
                    CreatedDate = DateTime.Now,
                    Role = "User"
                };
                await _userRepo.CreateAsync(newUser);
                _response.Result = registerDTO;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.Message.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPost("register/admin")]
        public async Task<ActionResult<APIResponse>> RegisterAdmin([FromBody]AdminRegisterDTO registerDTO)
        {
            try
            {
                if(registerDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Invalid input" };
                    return BadRequest(_response);
                }
                var adminExist = await _userRepo.GetAsync(a => a.UserName == registerDTO.UserName && a.Role == "Admin");
                if(adminExist != null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add($"Admin Username '{registerDTO.UserName}' already exists.");
                    return BadRequest(_response);

                }
                
                User admin = new User()
                {
                    UserName = registerDTO.UserName,
                    Password = PasswordHash(registerDTO.Password),
                    FirstName = registerDTO.FirstName,
                    LastName = registerDTO.LastName,
                    Email = registerDTO.Email,
                    CreatedDate = DateTime.Now,
                    Role = "Admin"
                };
                await _userRepo.CreateAsync(admin);
                
                _response.StatusCode=HttpStatusCode.OK;
                _response.Result = registerDTO;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.Message.ToString()};
                return BadRequest(_response);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var loginResponse = await _userRepo.Login(loginRequest);
                if (loginResponse.Token == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Username or Password is incorrect");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = loginResponse;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.Message.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult<APIResponse>> ForgotPassword(string userName, string email)
        {
            try
            {
                var user = await _userRepo.GetAsync(u => u.UserName == userName && u.Email == email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid username or email");
                    return BadRequest(_response);
                }
                user.PasswordResetToken = CreateToken();
                user.ResetTokenExpires = DateTime.Now.AddDays(1);
                await _userRepo.SaveAsync();

                EmailDto emailDto = new EmailDto()
                {
                    From = "noreply@shawnlibrary.com",
                    To = email,
                    Subject = "Password Reset Notification",
                    Body = $"Dear, {user.FirstName} {user.LastName}, a password reset request. The token for your password reset is {user.PasswordResetToken}",

                };
                _emailService.SendEmail(emailDto);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = user.PasswordResetToken;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message.ToString());
                return BadRequest(_response);
            }
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult<APIResponse>> ResetPassword([FromBody]ResetPasswordDto resetPassword)
        {
            try
            {
                if(resetPassword == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid input");
                    return BadRequest(_response);
                }

                var user = await _userRepo.GetAsync(u => u.UserName == resetPassword.Username && u.Email == resetPassword.Email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Invalid username or email");
                    return NotFound(_response);
                }

                //var checkToken = await _userRepo.GetAsync(c => c.PasswordResetToken == resetPassword.ResetToken && c.ResetTokenExpires < DateTime.Now);
                var checkToken = await _userRepo.GetAsync(c => c.PasswordResetToken == resetPassword.ResetToken);
                if (checkToken == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid password reset token");
                    return BadRequest(_response);
                }

                if (string.IsNullOrEmpty(resetPassword.Password))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid input");
                    return BadRequest(_response);
                }

                user.Password = PasswordHash(resetPassword.Password);
                user.UpdatedTime = DateTime.Now;
                user.PasswordResetToken = null;
                user.ResetTokenExpires = null;

                await _userRepo.SaveAsync();
                EmailDto emailDto = new EmailDto()
                {
                    From = "noreply@shawnlibrary.com",
                    To = resetPassword.Email,
                    Subject="Reset Password Notification",
                    Body = $"Dear, {user.FirstName} {user.LastName}, this is to inform you that password has successfully reset to your new choice. You can now log in with your new credentials",

                };
                _emailService.SendEmail(emailDto);
                _response.StatusCode=HttpStatusCode.OK;
                _response.Result = "Password Reset Successful";

                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message.ToString());
                return BadRequest(_response);
            }
        }
        private string CreateToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        private string PasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        
    }
}
