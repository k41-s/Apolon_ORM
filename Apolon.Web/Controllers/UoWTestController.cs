using Apolon.Core.Models;
using Apolon.Core.ORM.Data;
using Microsoft.AspNetCore.Mvc;

namespace Apolon.Web.Controllers
{
    public class UoWTestController : Controller
    {
        private readonly IUnitOfWork _uow;

        public UoWTestController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RunCommitTest()
        {
            try
            {
                await _uow.BeginTransactionAsync();

                var newMedication = new Medication
                {
                    Name = "UoW Test Drug " + DateTime.Now.Ticks,
                    Manufacturer = "Test Pharma"
                };

                await _uow.Repository<Medication>().AddAsync(newMedication);

                var patientLastName = "Committed_" + DateTime.Now.Ticks;
                var newPatient = new Patient
                {
                    FirstName = "Alice",
                    LastName = patientLastName,
                    DateOfBirth = new DateTime(1980, 1, 1)
                };
                await _uow.Repository<Patient>().AddAsync(newPatient);

                var newPrescription = new Prescription
                {
                    PatientId = newPatient.Id,
                    MedicationId = newMedication.Id,
                    Dosage = "5mg once daily",
                    StartDate = DateTime.UtcNow
                };
                await _uow.Repository<Prescription>().AddAsync(newPrescription);

                await _uow.CommitAsync();

                TempData["Status"] = $"SUCCESS: Commit Test Passed. Patient '{newPatient.LastName}' (ID: {newPatient.Id}) and Prescription committed successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                TempData["Error"] = $"COMMIT FAILURE: Transaction rolled back. Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RunRollbackTest()
        {
            var patientLastName = $"Rollback_{Guid.NewGuid()}";

            try
            {
                await _uow.BeginTransactionAsync();

                var patientToRollback = new Patient
                {
                    FirstName = "Bob",
                    LastName = patientLastName,
                    DateOfBirth = new DateTime(1990, 1, 1)
                };
                await _uow.Repository<Patient>().AddAsync(patientToRollback);

                throw new InvalidOperationException("Simulated critical failure during transaction.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();

                TempData["Status"] = $"ROLLBACK SUCCESS: Failure caught. Transaction was rolled back. Patient with LastName '{patientLastName}' should NOT exist in DB.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
