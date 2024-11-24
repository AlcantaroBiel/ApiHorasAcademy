using Microsoft.AspNetCore.Mvc;
using CadastroApi.Models;
using CadastroApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CadastroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly CadastroContext _context;

        public UsuariosController(CadastroContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> Register(Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest();
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário cadastrado com sucesso!" });
        }

            // Método GET para buscar todos os cursos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Curso>>> GetCursos()
        {
            var cursos = await _context.Cursos.ToListAsync();  // Busca todos os cursos do banco de dados
            if (cursos == null || !cursos.Any())
            {
                return NotFound("Nenhum curso encontrado.");
            }
            
            return Ok(cursos);  // Retorna os cursos
        }
    }
}