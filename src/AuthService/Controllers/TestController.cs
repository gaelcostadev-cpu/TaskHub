using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("auth")]
    public class TestController : BaseAuthController
    {
        /// <summary>
        /// Validação do token JWT e informações do usuário autenticado
        /// </summary>
        /// <response code="200">Teste de validação realizado com sucesso</response>
        /// <response code="401">Token inválido</response>
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId,
                Email,
                Username
            });
        }
    }
}
