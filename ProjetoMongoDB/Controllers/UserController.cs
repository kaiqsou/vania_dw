using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoMongoDB.Models;

namespace ProjetoMongoDB.Controllers
{
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET do CREATE
        public IActionResult Create(string role)   // IActionResult permite várias coisas, como views
        {
            ViewBag.Role = role;
            return View();
        }

        // POST do CREATE
        [HttpPost]
        public async Task<IActionResult> Create(User user, string role)
        {
            // Início do IF
            // Se estiver tudo ok com o modelo da classe, cria o usuário
            if (ModelState.IsValid)
            {
                // Atribuindo valores do objeto 'user' a uma nova instância de ApplicationUser, do Identity
                ApplicationUser appUser = new ApplicationUser();

                // Lógica para criar o UserName, a partir do nome. OBS: UserName não pode conter espaços ou
                // símbolos especiais
                string userName = user.NomeCompleto.Replace(" ", ""); // Remover o espaço
                var normalizedString = userName.Normalize(NormalizationForm.FormD); // Separa os acentos das letras
                StringBuilder sb = new StringBuilder();

                // Percorrendo as letras da variável para verificar se há caracteres especiais, acentos
                foreach (char letra in normalizedString) 
                { 
                    if (CharUnicodeInfo.GetUnicodeCategory(letra) != UnicodeCategory.NonSpacingMark)
                    {
                        // Se for válido, acrescenta no 'sb' a letra para construir o UserName
                        sb.Append(letra);
                    }
                };

                userName = sb.ToString().Normalize(NormalizationForm.FormC);

                // Retirando tudo que não for letras e números
                userName = Regex.Replace(userName, @"[^a-zA-Z0-9\s]", "");

                appUser.UserName = userName;
                appUser.Email = user.Email;
                appUser.NomeCompleto = user.NomeCompleto;

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, role);
                    // ViewBag 'carrega' coisas pra view
                    ViewBag.Message = "Usuário cadastrado com sucesso"; 
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            // Fim do IF

            return View(user); // retorna os campos preenchidos para não ter a necessidade de reescrever, em caso de erro
        }

        // Inserir perfis no Banco de Dados - apenas Administradores
        [Authorize(Roles = "Administrador")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });

                if (result.Succeeded)
                {
                    ViewBag.Message = "Tipo de usuário cadastrado com sucesso";
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View();
        }
    }
}
