namespace StyleCop.Analyzers.Status.Web.Controllers
{
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using StyleCop.Analyzers.Status.Generator;
    using StyleCop.Analyzers.Status.Web.Models;

    public class HomeController : Controller
    {
        public ActionResult Index(string category = null,
            bool? hasImplementation = null,
            string status = null,
            CodeFixStatus? codeFixStatus = null)
        {
            MainViewModel viewModel = JsonConvert.DeserializeObject<MainViewModel>(System.IO.File.ReadAllText(this.Server.MapPath("~/foo.json")));

            var diagnostics = (from x in viewModel.Diagnostics
                               where category == null || x.Category == category
                               where hasImplementation == null || x.HasImplementation == hasImplementation
                               where status == null || x.Status == status
                               where codeFixStatus == null || x.CodeFixStatus == codeFixStatus
                               select x).ToArray();

            if (diagnostics.Length == 0)
            {
                // No entries found
                return this.HttpNotFound();
            }

            viewModel.Diagnostics = diagnostics;

            return this.View(viewModel);
        }
    }
}