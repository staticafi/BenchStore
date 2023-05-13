using BenchStoreBL.Models;
using BenchStoreBL.Services.Results;

using Microsoft.AspNetCore.Mvc;

namespace BenchStoreMVC.ViewComponents
{
    public class ResultDetails : ViewComponent
    {
        private readonly IResultsService _resultService;

        public ResultDetails(IResultsService resultsService)
        {
            _resultService = resultsService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int resultEntryID)
        {
            Result? result = await _resultService.GetResultByResultEntryID(resultEntryID);

            return View(result);
        }
    }
}

