using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpbDotNetCore5.Models;

namespace SpbDotNetCore5.Controllers
{
[ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;
        private readonly UserContext _userContext;

        public UsersController(ILogger<UsersController> logger, IMapper mapper, UserContext userContext)
        {
            _logger = logger;
            _mapper = mapper;
            _userContext = userContext;
        }

        [HttpGet]
        [Route("users")]
        public async Task<ActionResult<IEnumerable<DtoUser>>> GetAllUsers()
        {
            _logger.LogInformation("Getting all users from db");
            try
            {
                var users = await _userContext.Users.Include(x => x.PhoneNumbers).ToArrayAsync();
                var dtoUsers =  _mapper.Map<IEnumerable<DbUser>, IEnumerable<DtoUser>>(users);
                return Ok(dtoUsers);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot get all users");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Route("users")]
        public async Task<ActionResult<UserSpec>> CreateUser(UserSpec user)
        {
            if (user == null) 
                return BadRequest("User spec is null");
            
            _logger.LogInformation($"Creating user {user.Name} {user.Surname}");
            
            try
            {
                DbUser dbUser = UsersController.CreateUserFromDto(user);
                await _userContext.Users.AddAsync(dbUser);
                await _userContext.SaveChangesAsync();
                _logger.LogInformation($"User {user.Name} {user.Surname} saved successfully");
                
                var dtoUser = _mapper.Map<DbUser, DtoUser>(dbUser);
                return CreatedAtAction(nameof(GetUser),new { id = dtoUser.Id }, dtoUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot create user {user.Name} {user.Surname}");
                return StatusCode(500, e.Message);
            }
        }

        private static DbUser CreateUserFromDto(UserSpec user)
        {
            DbUser newuser = new DbUser()
            {
                Id = Guid.NewGuid(),
                Name = user.Name,
                Surname = user.Surname,
                Birthday = user.Birthday,
                PhoneNumbers = new List<DbPhoneNumber>()
            };
            foreach (string phoneNumber in user.PhoneNumbers)
            {
                newuser.PhoneNumbers.Add(new DbPhoneNumber()
                {
                    Id = Guid.NewGuid(),
                    UserId = newuser.Id,
                    PhoneNumber = phoneNumber,
                });
            }

            return newuser;
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<ActionResult<DtoUser>> GetUser([FromRoute]Guid id)
        {
            _logger.LogInformation($"Getting user [{id}]...");
            try
            {
                DbUser dbUser = await _userContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (dbUser == null)
                    return NotFound($"User with id [{id}] is not found");
                var dtoUser = _mapper.Map<DbUser, DtoUser>(dbUser);
                return Ok(dtoUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot get user with id [{id}]");
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        [Route("users/{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] Guid id)
        {
            _logger.LogInformation($"Deleting user [{id}]...");
            try
            {
                DbUser dbUser = await _userContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (dbUser == null)
                    return NotFound($"User with id [{id}] is not found");
                    // this is wrong:
                    //return StatusCode(204);
                    // for details see https://stackoverflow.com/questions/4088350/is-rest-delete-really-idempotent:
                _userContext.Users.Remove(dbUser);
                await _userContext.SaveChangesAsync();
                _logger.LogInformation($"User [{id}] deleted successfully");
                return StatusCode(204);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot delete user with id [{id}]");
                return StatusCode(500, e.Message);
            }
        }
    }
}