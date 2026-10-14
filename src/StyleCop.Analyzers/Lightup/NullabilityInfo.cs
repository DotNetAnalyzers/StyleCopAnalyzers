// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;

    internal readonly struct NullabilityInfo : IEquatable<NullabilityInfo>
    {
        public NullabilityInfo(NullableAnnotation annotation, NullableFlowState flowState)
        {
            this.Annotation = annotation;
            this.FlowState = flowState;
        }

        public NullableAnnotation Annotation { get; }

        public NullableFlowState FlowState { get; }

        public bool Equals(NullabilityInfo other)
        {
            return this.Annotation == other.Annotation
                && this.FlowState == other.FlowState;
        }

        public override bool Equals(object obj)
        {
            return obj is NullabilityInfo other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return unchecked(((int)this.Annotation * 397) ^ (int)this.FlowState);
        }
    }
}
