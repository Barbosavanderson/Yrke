using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;
using Yrke.Data;
using Yrke.Models;
using Yrke.ViewModels;
using Microsoft.AspNetCore.Identity;


namespace Yrke.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Buscar usuário apenas pelo email
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user != null)
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Senha, model.Senha);

                if (result == PasswordVerificationResult.Success)
                {
                    await SignInUserAsync(user);
                    TempData["WelcomeMessage"] = $"Bem-Vindo ao Yrke, {user.Nome}!";
                    return RedirectToAction("Perfil", "Account");
                }
            }

            ModelState.AddModelError("", "Credenciais inválidas");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Cadastrar()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email já cadastrado");
                return View(model);
            }
            var newUser = new User
            {
                Nome = model.Nome,
                Email = model.Email,
                Telefone = model.Telefone,
                TipoEscala = model.TipoEscala,
                Funcao = model.Funcao,

            };
            // deixando a senha mais segura 

            var hasher = new PasswordHasher<User>();
            newUser.Senha = hasher.HashPassword(newUser, model.Senha);

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecuperarSenha()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarSenha(RecuperarSenhaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email não encontrado");
                return View(model);
            }

            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.TokenExpiration = DateTime.Now.AddMinutes(15);

            _context.Users.Update(user);
            _context.SaveChanges();

            var resetLink = Url.Action("RedefinirSenha", "Account", new { token = token }, Request.Scheme);

            // Para testes
            await _emailService.SendEmailAsync(user.Email, "Redefinição de Senha", $"Clique no link para redefinir sua senha: {resetLink}");

            ViewBag.Message = "Um link de redefinição foi gerado. Verifique seu email.";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RedefinirSenha(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetToken == token && u.TokenExpiration > DateTime.Now);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new RedefinirSenhaViewModel { Token = token };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.ResetToken == model.Token && u.TokenExpiration > DateTime.Now);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var hasher = new PasswordHasher<User>();
            user.Senha = hasher.HashPassword(user, model.NovaSenha);

            user.ResetToken = null;
            user.TokenExpiration = null;

            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Perfil()
        {
            var user = GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View(CreatePerfilViewModel(user));
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditarPerfil()
        {
            var user = GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View("Perfil", CreatePerfilViewModel(user));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(PerfilViewModel model)
        {
            var user = GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (model.Id != user.Id)
                ModelState.AddModelError("", "Perfil inválido.");

            var emailAlreadyUsed = _context.Users.Any(u => u.Email == model.Email && u.Id != user.Id);
            if (emailAlreadyUsed)
                ModelState.AddModelError(nameof(model.Email), "Email já cadastrado");

            if (!ModelState.IsValid)
                return View("Perfil", model);

            user.Nome = model.Nome;
            user.Email = model.Email;
            user.Telefone = model.Telefone;
            user.Funcao = model.Funcao;
            user.TipoEscala = model.TipoEscala;

            // Processar upload de foto, se fornecido
            if (model.FotoFile != null && model.FotoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "perfil");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileExt = Path.GetExtension(model.FotoFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FotoFile.CopyToAsync(stream);
                }

                user.UrlFoto = $"/uploads/perfil/{fileName}";
            }

            _context.Users.Update(user);
            _context.SaveChanges();

            await SignInUserAsync(user);

            TempData["WelcomeMessage"] = "Perfil atualizado com sucesso.";
            return RedirectToAction("Perfil", "Account");
        }

        private User? GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out var id))
                return _context.Users.FirstOrDefault(u => u.Id == id);

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrWhiteSpace(email))
                return _context.Users.FirstOrDefault(u => u.Email == email);

            return null;
        }

        private static PerfilViewModel CreatePerfilViewModel(User user)
        {
            return new PerfilViewModel
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                Telefone = user.Telefone,
                Funcao = user.Funcao,
                TipoEscala = user.TipoEscala,
                UrlFoto = user.UrlFoto
            };
        }

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }
    }
}
