using Apolon.Core.Models;
using Apolon.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apolon.Web.Controllers
{
    public class CheckupController : Controller
    {
        private readonly ICheckupService _checkupService;
        private readonly IPatientService _patientService;

        public CheckupController(ICheckupService checkupService, IPatientService patientService)
        {
            _checkupService = checkupService;
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var checkups = await _checkupService.GetAllCheckupsAsync();
            return View(checkups);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _patientService.GetAllPatientsForSelectListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Date,CheckupType,Finding")] Checkup checkup)
        {
            if (ModelState.IsValid)
            {
                await _checkupService.CreateCheckupAsync(checkup);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = await _patientService.GetAllPatientsForSelectListAsync();
            return View(checkup);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _checkupService.DeleteCheckupAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
