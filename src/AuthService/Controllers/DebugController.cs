using AuthService.Contracts;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("debug")]
    public class DebugController : ControllerBase
    {
        private readonly IPasswordHasher _passwordHasher;

        public DebugController(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Debug endpoint para hashear e verificar senhas.
        /// </summary>
        /// <response code="200">Hash realizado com sucesso</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("hash")]
        public IActionResult Hash([FromBody] HashRequest request)
        {
            try
            {
                var hash = _passwordHasher.Hash(request.Password);
                var verified = _passwordHasher.Verify(request.Password, hash);

                return Ok(new
                {
                    password = request.Password,
                    hash,
                    verified
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
