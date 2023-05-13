using BenchStoreBL.Models;
using BenchStoreBL.Services.Labels;
using BenchStoreBL.Services.ResultEntries;

using BenchStoreMVC.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace BenchStoreMVC.ViewComponents
{
    public class ResultEntriesList : ViewComponent
    {
        private readonly IResultEntriesService _resultEntriesService;

        public ResultEntriesList(IResultEntriesService resultEntriesService, ILabelsService labelsService)
        {
            _resultEntriesService = resultEntriesService;
        }

        public async Task<IViewComponentResult> InvokeAsync(OrderResultEntryBy orderResultEntryBy, ResultEntriesFilter? filter = null)
        {
            List<ResultEntry> resultEntries = await _resultEntriesService.GetResultEntries(orderResultEntryBy, filter);

            List<ListResultEntryViewModel> storedResultListViewModels = resultEntries
                .Select(sr => new ListResultEntryViewModel
                {
                    ResultEntry = sr,
                    Result = sr.Result,
                    Labels = sr.Labels,
                    SelectResult = new SelectResultEntryViewModel
                    {
                        ID = sr.ID,
                        IsChecked = false,
                    }
                })
                .ToList();

            return View(storedResultListViewModels);
        }
    }
}

