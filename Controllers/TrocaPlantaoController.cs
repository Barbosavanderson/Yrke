using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Yrke.Data;
using Yrke.Hubs;
using Yrke.Models;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TrocaPlantaoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public TrocaPlantaoController(ApplicationDbContext context, EmailService emailService, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _emailService = emailService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Lista todos os usuários disponíveis para troca (exceto o usuário autenticado)
    /// </summary>
    [HttpGet("usuarios")]
    public IActionResult ListarUsuarios()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var usuarios = _context.Users
            .Where(u => u.Id.ToString() != userId)
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

    /// <summary>
    /// Cria uma solicitação de troca de plantão
    /// </summary>
    [HttpPost("solicitar")]
    public async Task<IActionResult> SolicitarTroca([FromBody] SolicitacaoTrocaDto dto)
    {
        var solicitanteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(solicitanteId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.DestinatarioId) || dto.PlantaoA == default || dto.PlantaoB == default)
            return BadRequest("Preencha todos os campos obrigatórios");

        // Verificar se o destinatário existe
        var destinatario = _context.Users.FirstOrDefault(u => u.Id.ToString() == dto.DestinatarioId);
        if (destinatario == null)
            return NotFound("Destinatário não encontrado");

        // Criar solicitação de troca
        var troca = new TrocaPlantao
        {
            SolicitanteId = solicitanteId,
            DestinatarioId = dto.DestinatarioId,
            PlantaoA = dto.PlantaoA,
            PlantaoB = dto.PlantaoB,
            Status = "Pendente"
        };

        _context.Trocas.Add(troca);
        _context.SaveChanges();

        // tentar criar notificação em banco e enviar SignalR (não bloqueante)
        try
        {
            var notification = new Notification
            {
                UserId = dto.DestinatarioId,
                Title = "Nova solicitação de troca",
                Message = $"Você recebeu uma solicitação de troca de plantão de { ( _context.Users.FirstOrDefault(u => u.Id.ToString() == solicitanteId)?.Nome ?? "um colega") } para {dto.PlantaoB:yyyy-MM-dd}",
                Link = "/Home/Trabalhos",
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
            await _hubContext.Clients.User(dto.DestinatarioId).SendAsync("ReceiveNotification", notificationEvent);
        }
        catch
        {
            // se falhar ao gravar/enviar notificação, não interrompe a criação da troca
        }

        // enviar email ao destinatário (não bloqueante para o retorno)
        var destinatarioEmail = destinatario.Email;
        var emailSubject = "Solicitação de troca de plantão";
        var emailBody = $"<p>Olá {destinatario.Nome},</p><p>Você recebeu uma solicitação de troca de plantão de <strong>{ _context.Users.FirstOrDefault(u => u.Id.ToString() == solicitanteId)?.Nome }</strong>.</p><p>Plantão do solicitante: {dto.PlantaoA:yyyy-MM-dd}<br/>Plantão solicitado: {dto.PlantaoB:yyyy-MM-dd}</p><p><a href=\"{Request.Scheme}://{Request.Host}/Home/Trabalhos\">Ver no sistema</a></p>";
        try
        {
            await _emailService.SendEmailAsync(destinatarioEmail, emailSubject, emailBody);
        }
        catch
        {
            // log opcional, não interrompe o fluxo
        }

        return Ok(new { message = "Solicitação de troca registrada com sucesso", id = troca.Id });
    }

    /// <summary>
    /// Lista trocas pendentes para o usuário autenticado
    /// </summary>
    [HttpGet("pendentes")]
    public IActionResult ListarTrocasPendentes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var trocas = _context.Trocas
            .Where(t => (t.SolicitanteId == userId || t.DestinatarioId == userId) && t.Status == "Pendente")
            .Select(t => new
            {
                id = t.Id,
                solicitante = _context.Users.FirstOrDefault(u => u.Id.ToString() == t.SolicitanteId).Nome,
                destinatario = _context.Users.FirstOrDefault(u => u.Id.ToString() == t.DestinatarioId).Nome,
                plantaoA = t.PlantaoA.ToString("yyyy-MM-dd"),
                plantaoB = t.PlantaoB.ToString("yyyy-MM-dd"),
                status = t.Status
            })
            .ToList();

        return Ok(trocas);
    }

    /// <summary>
    /// Lista todas as trocas (próximas trocas) - todas com qualquer status
    /// </summary>
    [HttpGet("todas")]
    public IActionResult ListarTodasAsTrocas()
    {
        var trocas = _context.Trocas
            .Select(t => new
            {
                id = t.Id,
                solicitante = _context.Users.FirstOrDefault(u => u.Id.ToString() == t.SolicitanteId).Nome,
                destinatario = _context.Users.FirstOrDefault(u => u.Id.ToString() == t.DestinatarioId).Nome,
                plantaoA = t.PlantaoA.ToString("yyyy-MM-dd"),
                plantaoB = t.PlantaoB.ToString("yyyy-MM-dd"),
                status = t.Status
            })
            .OrderByDescending(t => t.id)
            .ToList();

        return Ok(trocas);
    }

    /// <summary>
    /// Aceita uma solicitação de troca
    /// </summary>
    [HttpPost("{id}/aceitar")]
    public IActionResult AceitarTroca(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var troca = _context.Trocas.FirstOrDefault(t => t.Id == id);
        if (troca == null)
            return NotFound("Troca não encontrada");

        // Apenas o destinatário pode aceitar
        if (troca.DestinatarioId != userId)
            return Forbid();

        troca.Status = "Aceita";
        _context.SaveChanges();

        return Ok(new { message = "Troca aceita com sucesso" });
    }

    /// <summary>
    /// Rejeita uma solicitação de troca
    /// </summary>
    [HttpPost("{id}/rejeitar")]
    public IActionResult RejeitarTroca(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var troca = _context.Trocas.FirstOrDefault(t => t.Id == id);
        if (troca == null)
            return NotFound("Troca não encontrada");

        // Apenas o destinatário pode rejeitar
        if (troca.DestinatarioId != userId)
            return Forbid();

        troca.Status = "Negada";
        _context.SaveChanges();

        return Ok(new { message = "Troca rejeitada" });
    }
}

public class SolicitacaoTrocaDto
{
    public string DestinatarioId { get; set; }
    public DateTime PlantaoA { get; set; }
    public DateTime PlantaoB { get; set; }
}
