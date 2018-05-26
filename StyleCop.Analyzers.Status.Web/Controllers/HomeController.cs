// Copyright (c) Dennis Fischer. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using StyleCop.Analyzers.Status.Generator;
    using StyleCop.Analyzers.Status.Web.Models;

    /// <summary>
    /// Main controller for the status page.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// This is the status page.
        /// </summary>
        /// <param name="category">The category that is filtered by.</param>
        /// <param name="hasImplementation">Has implementation filter.</param>
        /// <param name="status">Status filter.</param>
        /// <param name="codeFixStatus">Code fix status filter.</param>
        /// <returns>An <see cref="ActionResult"/> decribing the resulting page.</returns>
        public ActionResult Index(
            string category = null,
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