using ConfigureMS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SenderEmail.Controllers
{
    public class StartConfigureController : Controller
    {
        private readonly IRepoMS data;
        private readonly IStartConfigurationMS config;
        private readonly ILogger<StartConfigureController> _logger;

        public StartConfigureController(IRepoMS data, IStartConfigurationMS config, ILogger<StartConfigureController> logger)
        {
            this.data = data;
            this.config = config;
            _logger = logger;

        }
        public async Task<IActionResult> Index( [FromServices]IWebHostEnvironment webHostEnvironment, bool StartOver = false)
        {
            var pluginsFolder = Path.Combine(webHostEnvironment.WebRootPath, "plugins");
            await foreach (var item in config.StartFinding(pluginsFolder))
            {
                ModelState.AddModelError(item.MemberNames.FirstOrDefault() ?? "error", item.ErrorMessage);
            }
            if(!StartOver)
            try
            {
                await config.LoadData(data);
                if (config.IsConfigured()) 
                {
                    return RedirectToAction("ConfigurationSaved");
                }
            }
            catch
            {
                //do nothing 
            }
            var model = new SHIM_StartConfigurationMS(config);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SaveProvider(SHIM_StartConfigurationMS configUser)
        {
            config.ChoosenMainProvider = configUser.ChoosenMainProvider?.Trim();
            if (string.IsNullOrWhiteSpace(config.ChoosenMainProvider))
            {
                return View("Index");
            }
            await config.ChooseConfiguration("smtpProviders", config.ChoosenMainProvider);
            ;
            var shim = new SHIM_StartConfigurationMS(config);
            return View("Index", shim);
        }

        [HttpPost]
        public async Task<IActionResult> Test(Dictionary<string,string> myValues)
        {
            var shim = new SHIM_StartConfigurationMS(config);
            try
            {
                var d = myValues?.ToDictionary(it => it.Key, it => (object)it.Value);
                config.ChoosenProviderData.SetProperties(d);
                await config.ChoosenProviderData.Test();
                config.ConfiguredAt = DateTime.Now;
                await config.SaveData(data);
                return RedirectToAction("ConfigurationSaved");
            }
            catch(Exception ex)
            {
                config.ConfiguredAt = null;//restore false configuration
                ModelState.AddModelError("Test", ex.Message);
            }
            return View("Index", shim);
        }

        public IActionResult ConfigurationSaved()
        {
            if (!config.IsConfigured())
                return RedirectToAction("Index");

            var shim = new SHIM_StartConfigurationMS(config);
            return View(shim);
        }

    }
}
