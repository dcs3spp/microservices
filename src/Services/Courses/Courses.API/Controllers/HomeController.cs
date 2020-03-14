using Microsoft.AspNetCore.Mvc;

namespace dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
