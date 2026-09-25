using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Yrke.Data;
using Yrke.Hubs;
using Yrke.Models;
using Yrke.Services;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TrocaPlantaoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly TermoTrocaService _termoService;
    private readonly ILogger<TrocaPlantaoController> _logger;

    public TrocaPlantaoController(
        ApplicationDbContext context,
        EmailService emailService,
        IHubContext<NotificationHub> hubContext,
        TermoTrocaService termoService,
        ILogger<TrocaPlantaoController> logger)
    {
        _context = context;
        _emailService = emailService;
        _hubContext = hubContext;
        _termoService = termoService;
        _logger = logger;
    }

    [HttpGet("usuarios")]
    public IActionResult ListarUsuarios()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var usuarios = _context.Users
            .AsEnumerable()
            .Where(u => !IdsIguais(u.Id.ToString(), userId))
            .Select(u => new
            {
                id = u.Id.ToString(),
                nome = u.Nome,
                funcao = u.Funcao,
                email = u.Email
            })
            .OrderBy(u => u.nome)
            .ToList();

        return Ok(usuarios);
    }

    [HttpPost("solicitar")]
    public async Task<IActionResult> SolicitarTroca([FromBody] SolicitacaoTrocaDto dto)
    {
        var solicitanteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(solicitanteId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.DestinatarioId) || dto.PlantaoA == default || dto.PlantaoB == default)
            return BadRequest("Preencha todos os campos obrigatórios.");

        if (IdsIguais(dto.DestinatarioId, solicitanteId))
            return BadRequest("Não é possível solicitar troca para si mesmo.");

        var destinatario = _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), dto.DestinatarioId));
        if (destinatario == null)
            return NotFound("Destinatário não encontrado.");

        var solicitante = _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), solicitanteId));
        if (solicitante == null)
            return Unauthorized();

        var troca = new TrocaPlantao
        {
            SolicitanteId = solicitante.Id.ToString(),
            DestinatarioId = destinatario.Id.ToString(),
            PlantaoA = dto.PlantaoA,
            PlantaoB = dto.PlantaoB,
            Status = "Pendente"
        };

        _context.Trocas.Add(troca);
        _context.SaveChanges();

        var plantaoATexto = FormatarDataHora(dto.PlantaoA);
        var plantaoBTexto = FormatarDataHora(dto.PlantaoB);

        await EnviarNotificacaoAsync(
            destinatario.Id.ToString(),
            "Nova solicitação de troca",
            $"{solicitante.Nome} solicitou troca: plantão dele em {plantaoATexto} pelo seu em {plantaoBTexto}.",
            "/Home/Trabalhos");

        try
        {
            var emailBody = $"<p>Olá {destinatario.Nome},</p>" +
                $"<p>Você recebeu uma solicitação de troca de <strong>{solicitante.Nome}</strong>.</p>" +
                $"<p>Plantão do solicitante: {plantaoATexto}<br/>Plantão solicitado (seu): {plantaoBTexto}</p>" +
                $"<p><a href=\"{Request.Scheme}://{Request.Host}/Home/Trabalhos\">Ver no sistema</a></p>";
            await _emailService.SendEmailAsync(destinatario.Email, "Solicitação de troca de plantão", emailBody);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao enviar e-mail de solicitação de troca {TrocaId}", troca.Id);
        }

        return Ok(new { message = "Solicitação de troca registrada com sucesso", id = troca.Id });
    }

    [HttpGet("pendentes")]
    public IActionResult ListarTrocasPendentes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var trocas = _context.Trocas
            .Where(t => t.Status == "Pendente")
            .AsEnumerable()
            .Where(t => IdsIguais(t.SolicitanteId, userId) || IdsIguais(t.DestinatarioId, userId))
            .OrderByDescending(t => t.Id)
            .AsEnumerable()
            .Select(t => new
            {
                id = t.Id,
                solicitante = ObterNomeUsuario(t.SolicitanteId),
                destinatario = ObterNomeUsuario(t.DestinatarioId),
                plantaoA = FormatarDataHora(t.PlantaoA),
                plantaoB = FormatarDataHora(t.PlantaoB),
                status = t.Status,
                podeResponder = IdsIguais(t.DestinatarioId, userId)
            })
            .ToList();

        return Ok(trocas);
    }

    [HttpGet("todas")]
    public IActionResult ListarTodasAsTrocas()
    {
        var trocas = _context.Trocas
            .OrderByDescending(t => t.Id)
            .AsEnumerable()
            .Select(t => new
            {
                id = t.Id,
                solicitante = ObterNomeUsuario(t.SolicitanteId),
                destinatario = ObterNomeUsuario(t.DestinatarioId),
                plantaoA = FormatarDataHora(t.PlantaoA),
                plantaoB = FormatarDataHora(t.PlantaoB),
                status = t.Status
            })
            .ToList();

        return Ok(trocas);
    }

    [HttpGet("{id}/termo")]
    public IActionResult ObterTermo(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var troca = _context.Trocas.FirstOrDefault(t => t.Id == id);
        if (troca == null)
            return NotFound("Troca não encontrada.");

        if (!IdsIguais(troca.DestinatarioId, userId))
            return Forbid();

        if (troca.Status != "Pendente")
            return BadRequest("Esta troca não está pendente.");

        var usuario = _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), userId));
        if (usuario == null)
            return Unauthorized();

        try
        {
            var pdf = _termoService.GerarTermo(usuario.Nome, DateTime.Now);
            return File(pdf, "application/pdf", "termo_troca_plantao.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar termo da troca {TrocaId}", id);
            return StatusCode(500, "Não foi possível gerar o termo de ciência.");
        }
    }

    [HttpPost("{id}/aceitar")]
    public async Task<IActionResult> AceitarTroca(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var troca = _context.Trocas.FirstOrDefault(t => t.Id == id);
        if (troca == null)
            return NotFound("Troca não encontrada.");

        if (!IdsIguais(troca.DestinatarioId, userId))
            return Forbid();

        if (troca.Status != "Pendente")
            return BadRequest("Esta troca já foi processada.");

        EfetivarTrocaPlantoes(troca);

        troca.Status = "Aceita";
        _context.SaveChanges();

        var destinatario = _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), userId));
        var solicitante = _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), troca.SolicitanteId));

        if (solicitante != null)
        {
            await EnviarNotificacaoAsync(
                troca.SolicitanteId,
                "Troca aceita",
                $"{destinatario?.Nome ?? "O colega"} aceitou sua solicitação de troca ({FormatarDataHora(troca.PlantaoA)} ↔ {FormatarDataHora(troca.PlantaoB)}).",
                "/Home/Trabalhos");

            try
            {
                if (!string.IsNullOrWhiteSpace(solicitante.Email))
                {
                    var emailBody = $"<p>Olá {solicitante.Nome},</p>" +
                        $"<p>Sua solicitação de troca foi <strong>aceita</strong> por {destinatario?.Nome}.</p>" +
                        $"<p><a href=\"{Request.Scheme}://{Request.Host}/Home/Trabalhos\">Ver no sistema</a></p>";
                    await _emailService.SendEmailAsync(solicitante.Email, "Troca de plantão aceita", emailBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao enviar e-mail de aceite da troca {TrocaId}", id);
            }
        }

        return Ok(new { message = "Troca aceita com sucesso" });
    }

    [HttpPost("{id}/rejeitar")]
    public async Task<IActionResult> RejeitarTroca(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var troca = _context.Trocas.FirstOrDefault(t => t.Id == id);
        if (troca == null)
            return NotFound("Troca não encontrada.");

        if (!IdsIguais(troca.DestinatarioId, userId))
            return Forbid();

        if (troca.Status != "Pendente")
            return BadRequest("Esta troca já foi processada.");

        troca.Status = "Negada";
        _context.SaveChanges();

        var destinatario = _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), userId));
        var solicitante = _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), troca.SolicitanteId));

        if (solicitante != null)
        {
            await EnviarNotificacaoAsync(
                troca.SolicitanteId,
                "Troca rejeitada",
                $"{destinatario?.Nome ?? "O colega"} rejeitou sua solicitação de troca ({FormatarDataHora(troca.PlantaoA)} ↔ {FormatarDataHora(troca.PlantaoB)}).",
                "/Home/Trabalhos");

            try
            {
                if (!string.IsNullOrWhiteSpace(solicitante.Email))
                {
                    var emailBody = $"<p>Olá {solicitante.Nome},</p>" +
                        $"<p>Sua solicitação de troca foi <strong>rejeitada</strong> por {destinatario?.Nome}.</p>" +
                        $"<p><a href=\"{Request.Scheme}://{Request.Host}/Home/Trabalhos\">Ver no sistema</a></p>";
                    await _emailService.SendEmailAsync(solicitante.Email, "Troca de plantão rejeitada", emailBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao enviar e-mail de rejeição da troca {TrocaId}", id);
            }
        }

        return Ok(new { message = "Troca rejeitada" });
    }

    private void EfetivarTrocaPlantoes(TrocaPlantao troca)
    {
        var plantaoSolicitante = ObterOuCriarPlantao(troca.SolicitanteId, troca.PlantaoA);
        var plantaoDestinatario = ObterOuCriarPlantao(troca.DestinatarioId, troca.PlantaoB);

        plantaoSolicitante.UsuarioId = troca.DestinatarioId;
        plantaoDestinatario.UsuarioId = troca.SolicitanteId;
    }

    private Plantao ObterOuCriarPlantao(string usuarioId, DateTime dataHora)
    {
        var inicioDia = dataHora.Date;
        var fimDia = inicioDia.AddDays(1);

        var plantao = _context.Plantoes
            .AsEnumerable()
            .FirstOrDefault(p => IdsIguais(p.UsuarioId, usuarioId) && p.Data >= inicioDia && p.Data < fimDia);

        if (plantao != null)
        {
            plantao.Data = dataHora;
            return plantao;
        }

        plantao = new Plantao
        {
            UsuarioId = usuarioId,
            Data = dataHora,
            Turno = InferirTurno(dataHora)
        };
        _context.Plantoes.Add(plantao);
        return plantao;
    }

    private static string InferirTurno(DateTime dataHora)
    {
        return dataHora.Hour switch
        {
            >= 5 and < 12 => "Manhã",
            >= 12 and < 18 => "Tarde",
            _ => "Noite"
        };
    }

    private async Task EnviarNotificacaoAsync(string userId, string title, string message, string link)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();

            var notificationEvent = new NotificationEventDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Link = notification.Link,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            };

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notificationEvent);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao enviar notificação para usuário {UserId}", userId);
        }
    }

    private string ObterNomeUsuario(string userId)
    {
        return _context.Users.AsEnumerable().FirstOrDefault(u => IdsIguais(u.Id.ToString(), userId))?.Nome ?? "Usuário";
    }

    private static bool IdsIguais(string? idA, string? idB)
    {
        if (string.IsNullOrWhiteSpace(idA) || string.IsNullOrWhiteSpace(idB))
            return false;

        return Guid.TryParse(idA, out var guidA)
            && Guid.TryParse(idB, out var guidB)
            && guidA == guidB;
    }

    private static string FormatarDataHora(DateTime data)
    {
        return data.ToString("dd/MM/yyyy HH:mm");
    }
}

public class SolicitacaoTrocaDto
{
    public string DestinatarioId { get; set; } = string.Empty;
    public DateTime PlantaoA { get; set; }
    public DateTime PlantaoB { get; set; }
}
