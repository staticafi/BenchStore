using BenchStoreBL.Models;
using BenchStoreBL.Services.Labels;
using BenchStoreBL.Services.ResultEntries;

using BenchStoreMVC.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace BenchStoreMVC.ViewComponents
{
    public class ResultEntryDetails : ViewComponent
    {
        private readonly IResultEntriesService _resultEntriesService;
        private readonly ILabelsService _labelsService;

        public ResultEntryDetails(IResultEntriesService resultEntriesService, ILabelsService labelsService)
        {
            _resultEntriesService = resultEntriesService;
            _labelsService = labelsService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            ResultEntry? resultEntry = await _resultEntriesService.GetResultEntryByID(id);

            if (resultEntry == null)
            {
                throw new Exception("not found result");
            }

            IEnumerable<Label> labels = await _labelsService.GetResultEntryLabels(id);

            DetailsResultEntryViewModel storedResultDetailsViewModel = new DetailsResultEntryViewModel
            {
                ResultEntry = resultEntry,
                Labels = labels
            };

            return View(storedResultDetailsViewModel);
        }
    }
}

