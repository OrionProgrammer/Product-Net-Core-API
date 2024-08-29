using API.Controllers;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers { }

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;

    public UserController(
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IOptions<AppSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        var user = _unitOfWork.UserRepository.Authenticate(model.Email, model.Password);
        await _unitOfWork.Complete();

        if (user is null)
        {
            return NotFound(new
            {
                errors = "Username or Password is incorrect!" 
            });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // return user info and authentication token
        return Ok(new
        {
            id = user.UserId,
            name = user.Name,
            surname = user.Surname,
            email = user.Email,
            Token = tokenString
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        // map model to entity
        var user = _mapper.Map<User>(model);

        try
        {
            // create user
            await _unitOfWork.UserRepository.InsertAsync(user);
            await _unitOfWork.Complete();

            return CreatedAtAction(nameof(Register), new { id = user.UserId });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception 
            return StatusCode(StatusCodes.Status500InternalServerError, new { errors = ex.Message });
        }
    }

    [HttpGet("test")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    public async Task<User> Test()
    {
        long i = 1;

        var u = _unitOfWork.UserRepository.Authenticate("asheenk@gmail.com", "ash");
        await _unitOfWork.Complete();

        return u;

    }

}

