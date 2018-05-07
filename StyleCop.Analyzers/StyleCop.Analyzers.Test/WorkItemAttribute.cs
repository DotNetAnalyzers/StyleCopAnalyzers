// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test
{
    using System;

    /// <summary>
    /// Used to tag test methods or types which are created for a given WorkItem
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class WorkItemAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemAttribute"/> class.
        /// </summary>
        /// <param name="id">The ID of the issue in the original tracker where the work item was first reported. This
        /// could be a GitHub issue or pull request number, or the number of a Microsoft-internal bug.</param>
        /// <param name="issueUri">The URI where the work item can be viewed. This is a link to work item
        /// <paramref name="id"/> in the original source.</param>
        public WorkItemAttribute(int id, string issueUri)
        {
            this.Id = id;
            this.Location = issueUri;
        }

        public int Id
        {
            get;
        }

        public string Location
        {
            get;
        }
    }
}
