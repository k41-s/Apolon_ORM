using Apolon.Core.Models;
using Apolon.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apolon.Web.Controllers
{
    public class MedicationController : Controller
    {
        private readonly IMedicationService _service;

        public MedicationController(IMedicationService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var medications = await _service.GetAllMedicationsAsync();
            return View(medications);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Manufacturer")] Medication medication)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateMedicationAsync(medication);
                return RedirectToAction(nameof(Index));
            }
            return View(medication);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteMedicationAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
