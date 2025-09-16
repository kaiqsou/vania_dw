using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjetoMongoDB.Models;
using ProjetoMongoDB.ViewModels;

namespace ProjetoMongoDB.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager; // gerenciar o Login

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous] // Permite que qualquer um possa fazer o Login, sem estar Autenticado (dentro de uma conta)
        public async Task<IActionResult> Login
        (
            // Por parâmetro será recebido o e-mail e senha de forma obrigatória
            [Required][EmailAddress] string email,
            [Required] string password
        )
        {
            // Se estiver tudo certo com os parâmetros, entra aqui para validação do Login
            if (ModelState.IsValid)
            {
                ApplicationUser appuser = await _userManager.FindByEmailAsync(email);

                if (appuser != null)
                {
                    // Faz a verificação da senha
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(appuser, password, false, false);

                    // Se der tudo certo, redireciona para o Index
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    // Se não, retorna mensagem de erro abaixo do e-mail
                    ModelState.AddModelError(nameof(email), "Verifique suas credenciais");
                }
            }

            return View();
        }

        [Authorize] // Permite utilizar o método apenas se estiver Autorizado (logado), caso contrário, redireciona para o Login
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        
        // Métodos GET de ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPassword(string token, string userId)
        {
            if (token == null || userId == null)
            {
                ModelState.AddModelError("", "Token Inválido");
            }

            var model = new ResetPasswordViewModel
            {
                token = token,
                UserId = userId
            };

            return View(model);
        }

        // Métodos POST
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Se o e-mail for null, entra aqui 
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Informe o e-mail");
                return View();
            }

            // Instância de usuário, para procurá-lo por e-mail
            ApplicationUser user = await _userManager.FindByNameAsync(email);

            // Se o usuário for null, entra aqui
            if (user == null) 
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            // Se o usuário for encontrado, continua o processo, criando um Token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = encodedToken }, Request.Scheme);

            // Montar os elementos do e-mail
            string assunto = "Redefinição de Senha";
            string corpo = $"Clique no link para redefinir sua senha: <a href='{callbackUrl}'>Redefinir senha</a>";

            await _emailService.SendEmailAsync(email, assunto, corpo);

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // Verifica se a Model é inválida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Procura o usuário pelo Id na Model
            var user = await _userManager.FindByIdAsync(model.UserId);

            // Verifica se o usuário é null
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            // Decodifica o token
            var decodedToken = HttpUtility.UrlDecode(model.Token);

            // Passa as informações para o Identity, que faz a troca para a nova senha
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            // Se der tudo certo, entra aqui
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            // Se tiver erros, todos eles serão coletados no ForEach
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
