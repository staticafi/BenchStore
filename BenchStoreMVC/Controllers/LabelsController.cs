using BenchStoreBL.Models;
using BenchStoreBL.Services.Labels;

using Microsoft.AspNetCore.Mvc;

namespace BenchStoreMVC.Controllers
{
    public class LabelsController : Controller
    {
        private readonly ILabelsService _labelsService;
        public LabelsController(ILabelsService labelsService)
        {
            _labelsService = labelsService;
        }

        // GET: Labels
        public async Task<IActionResult> Index()
        {
            IEnumerable<Label> labels = await _labelsService.GetLabels();
            return View(labels);
        }

        // GET: Labels/Edit/id
        public async Task<IActionResult> Edit(int id)
        {
            Label? label = await _labelsService.GetLabelByID(id);

            if (label == null)
            {
                return NotFound();
            }

            return View(label);
        }

        // POST: Labels/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Label label)
        {
            if (label.ID != id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(label);
            }

            try
            {
                await _labelsService.EditLabel(label);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

