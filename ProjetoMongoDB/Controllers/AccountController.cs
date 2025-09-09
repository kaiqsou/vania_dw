using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoMongoDB.Models;

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
    }
}
