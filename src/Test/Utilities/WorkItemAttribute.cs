// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Roslyn.Diagnostics.Test.Utilities
{
    public class WorkItemAttribute : Attribute
    {
        private int _id;
        private string _source;

        public WorkItemAttribute(int id, string source)
        {
            this._id = id;
            this._source = source;
        }
    }
}