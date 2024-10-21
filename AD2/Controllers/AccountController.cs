using System;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Web.Mvc;
using ADLoginApp.Models;

namespace ADLoginApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            // 從 web.config 讀取預設 AD 伺服器 IP
            var model = new LoginViewModel
            {
                ADServer = ConfigurationManager.AppSettings["ADServerIP"]
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.ADServer))
                {
                    ModelState.AddModelError("", "AD Server IP is required.");
                    return View(model);
                }

                try
                {
                    // 使用指定的 AD 伺服器 IP 進行驗證
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, model.ADServer))
                    {
                        bool isValid = pc.ValidateCredentials(model.Username, model.Password);
                        if (isValid)
                        {
                            ViewBag.Message = $"Welcome, {model.Username}! Connected to AD server {model.ADServer}.";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid credentials.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error connecting to AD server: {ex.Message}");
                }
            }

            return View(model);
        }
    }
}
