using CadastroApi.Data;
using CadastroApi.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configura o CORS para permitir solicitações de qualquer origem
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Configura o DbContext para usar o MySQL
builder.Services.AddDbContext<CadastroContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));  // Atualize para sua versão do MySQL

// Adiciona o Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAllOrigins");

// Configura o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty; // Para acessar a UI do Swagger na raiz
    });
}

app.UseHttpsRedirection();

// Rota para cadastro
app.MapPost("/api/usuarios", async (CadastroRequest request, CadastroContext db) =>
{
    // Lógica para cadastro
    // Verifique se o email já está cadastrado
    if (await db.Usuarios.AnyAsync(u => u.email == request.email))
    {
        return Results.BadRequest("Email já cadastrado.");
    }

    // Verifique se o curso existe
    if (!await db.Cursos.AnyAsync(c => c.id_curso == request.curso))
    {
        return Results.BadRequest("Curso não encontrado.");
    }

    var usuario = new Usuario
    {
        nome = request.nome,
        email = request.email,
        curso = request.curso, // Usar o ID do curso fornecido
        matricula = request.matricula,
        admin = request.admin,
        senha = request.senha // Considere hash para senhas
    };

    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();

    return Results.Created($"/api/usuarios/{usuario.aluno_id}", usuario); // Retorna o usuário criado
})
.WithName("CadastrarUsuario");

// Rota para login
app.MapPost("/api/login", async (LoginRequest request, CadastroContext db) => 
{
    // Verifica se o usuário existe pelo nome de usuário
var usuario = await db.Usuarios.SingleOrDefaultAsync(u => u.nome == request.nome && u.senha == request.senha);
    
    // Se não existir ou a senha não coincidir, retorna erro
    if (usuario == null || usuario.senha != request.senha) // Use hash para senhas em produção
    {
        return Results.BadRequest("Usuário ou senha inválidos.");
    }

    // Se o login for bem-sucedido, você pode retornar um token ou apenas o usuário
    return Results.Ok(usuario);
})
.WithName("LoginUsuario");

app.MapGet("/api/palestras", async (CadastroContext db) =>
{
    // Busca a lista de palestras do banco de dados
    var palestras = await db.Palestras
        .Select(p => new
        {
            palestra_id = p.palestra_id,
            titulo = p.titulo ?? string.Empty, // Tratando nulos para Titulo
            descricao = p.descricao ?? string.Empty, // Tratando nulos para Descricao
            local = p.local ?? string.Empty, // Tratando nulos para Local
            data_palestra = p.data_palestra.ToString("yyyy-MM-dd") ?? "Data não informada", // Tratando nulos para Data
            hora = p.hora != null ? p.hora.ToString(@"hh\:mm") : "Horário não informado",
            imagem = p.imagem ?? string.Empty, // Tratando nulos para Imagem
            categoria = p.categoria ?? "Sem categoria" // Tratando nulos para Categoria
        })
        .ToListAsync();

    // Retorna a lista de palestras
    return Results.Ok(palestras);
});

app.MapPost("/api/inscricao", async (InscricaoRequest request, CadastroContext db) =>
{
    // Verifica se o usuário existe pelo username
    var usuarioExistente = await db.Usuarios.FirstOrDefaultAsync(u => u.nome == request.nome);
    if (usuarioExistente == null)
    {
        return Results.BadRequest("Usuário não encontrado.");
    }

    // Agora usamos o id do usuário encontrado
    var usuarioId = usuarioExistente.aluno_id; // ou o nome do campo que armazena o ID do usuário

    // Verifica se a palestra existe
    var palestraExistente = await db.Palestras.FindAsync(request.palestra_id);
    if (palestraExistente == null)
    {
        return Results.BadRequest("Palestra não encontrada.");
    }

    // Verifica se a inscrição já existe
    var inscricaoExistente = await db.Inscricao
        .AnyAsync(i => i.aluno_id == usuarioId && i.palestra_id == request.palestra_id);

    if (inscricaoExistente)
    {
        return Results.BadRequest("Usuário já inscrito neste evento.");
    }

    // Cria nova inscrição
    var inscricao = new Inscricao
    {
        aluno_id = usuarioId, // usa o ID encontrado
        palestra_id = request.palestra_id,
        data_inscricao = DateTime.Now
    };

    db.Inscricao.Add(inscricao);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Inscrição realizada com sucesso!" });
})
.WithName("RegistrarInscricao");

app.MapGet("/api/cursos", async (CadastroContext db) =>
{
    var cursos = await db.Cursos.ToListAsync();

    if (cursos == null || !cursos.Any())
    {
        return Results.NotFound("Nenhum curso encontrado.");
    }

    return Results.Ok(cursos);
});


// Execute a aplicação
app.Run();

// Classe para o cadastro
public class CadastroRequest
{
    public string nome { get; set; }
    public string email { get; set; }
    public string senha { get; set; }
    public int curso { get; set; } // ID do curso
    public int matricula { get; set; }
    public int admin { get; set; } // Permite indicar se o usuário é admin
}

// Classe para login
public class LoginRequest
{
    public string nome { get; set; }
    public string senha { get; set; }
}

// Classe para a inscrição
public class InscricaoRequest
{
    public string nome { get; set; }
    public int palestra_id { get; set; }
}