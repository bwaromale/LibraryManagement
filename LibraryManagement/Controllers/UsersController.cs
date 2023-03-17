using AutoMapper;
using BCrypt.Net;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

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
        //private string secretKey;
        public UsersController(IMapper mapper, IUser userRepo)
        {
            
            _mapper = mapper;
            this._response = new APIResponse();
            _userRepo = userRepo;
            
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetUsers()
        {
            try
            {
                //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var tokenHandler = new JwtSecurityTokenHandler();
                //var apiSettings = new ApiSettings();
                //var validationParameters = apiSettings.GetTokenValidationParameters();
                //SecurityToken validatedToken;
                //var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                var users = await _userRepo.GetAllAsync();
                //var map = _mapper.Map<RegisterDTO>(users);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = users;
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
                string hashPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);
                User admin = new User()
                {
                    UserName = registerDTO.UserName,
                    Password = hashPassword,
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
    }
}
