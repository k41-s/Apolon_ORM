using Apolon.Core.Models;
using Apolon.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apolon.Web.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await _service.GetAllPatientsAsync();
            return View(patients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,DateOfBirth")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                await _service.CreatePatientAsync(patient);
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeletePatientAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
