using curso.api.Business.Entities;
using curso.api.Filters;
using curso.api.Infra.Data;
using curso.api.Models;
using curso.api.Models.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace curso.api.Controllers
{
    [Route("api/v1/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [SwaggerResponse(statusCode: 200, description: "Sucesso ao autenticar", Type = typeof(LoginViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campo Obrigatório", Type = typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro Interno", Type = typeof(ErroGenericoViewModel))]
        [HttpPost]
        [Route("logar")]
        [ValidacaoModelStateCustomizado]
        public IActionResult Logar(LoginViewModelInput loginViewModelInput)
        {

            var usuarioViewModelOutput = new UsuarioViewModelOutput()
            {
                Codigo = 1,
                Login = "teste",
                Email = "teste@email.com"
            };

            var secret = Encoding.ASCII.GetBytes("MzfsT&d9gorP>!9$Es(X!5g@;ef!5sbk:jH\\2.}8ZP'qY#7");
            var symmetricSecurityKey = new SymmetricSecurityKey(secret);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                   new Claim(ClaimTypes.NameIdentifier, usuarioViewModelOutput.Codigo.ToString()),
                   new Claim(ClaimTypes.Name, usuarioViewModelOutput.Login.ToString()),
                   new Claim(ClaimTypes.Email, usuarioViewModelOutput.Email.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtSeecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenGenerated = jwtSeecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSeecurityTokenHandler.WriteToken(tokenGenerated);

            return Ok(new 
            {
                Token = token,
                Usuario = usuarioViewModelOutput
            });
        }

        [HttpPost]
        [Route("registrar")]
        [ValidacaoModelStateCustomizado]
        public IActionResult Registrar(RegristroViewModelInput loginViewModelInput)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CursoDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=CURSO;user=sa;password=senha-qualquer");
            CursoDbContext contexto = new CursoDbContext(optionsBuilder.Options);

            var usuario = new Usuario();
            usuario.Login = loginViewModelInput.Login;
            usuario.Senha = loginViewModelInput.Senha;
            usuario.Email = loginViewModelInput.Email;
            contexto.Usuarios.Add(usuario);
            contexto.SaveChanges();

            return Created("", loginViewModelInput);
        }
    }
}
