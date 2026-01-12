using Apolon.Core.Models;
using Apolon.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apolon.Web.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientService _patientService;
        private readonly IMedicationService _medicationService;

        public PrescriptionController(IPrescriptionService prescriptionService, IPatientService patientService, IMedicationService medicationService)
        {
            _prescriptionService = prescriptionService;
            _patientService = patientService;
            _medicationService = medicationService;
        }

        public async Task<IActionResult> Index()
        {
            var prescriptions = await _prescriptionService.GetAllWithDetailsAsync();

            return View(prescriptions);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _patientService.GetAllPatientsForSelectListAsync();
            ViewBag.Medications = await _medicationService.GetAllMedicationsAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,MedicationId,Dosage,StartDate,EndDate")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                await _prescriptionService.CreatePrescriptionAsync(prescription);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = await _patientService.GetAllPatientsForSelectListAsync();
            ViewBag.Medications = await _medicationService.GetAllMedicationsAsync();
            return View(prescription);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _prescriptionService.DeletePrescriptionAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
