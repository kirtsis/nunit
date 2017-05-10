// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// RangeConstraint tests whether two _values are within a 
    /// specified range.
    /// </summary>
    public class RangeConstraint : Constraint
    {
        private readonly object from;
        private readonly object to;

        private ComparisonAdapter comparer = ComparisonAdapter.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeConstraint"/> class.
        /// </summary>
        /// <remarks>The <paramref name="from"/> value must be less than or equal to the <paramref name="to"/> value.</remarks> 
        /// <param name="from">Must be less than or equal to the <paramref name="to"/> value.</param>
        /// <param name="to">Must be greater than or equal to the <paramref name="from"/> value.</param>
        public RangeConstraint(IComparable from, IComparable to) : base( from, to )
        {
            // Issue #21 - https://github.com/nunit/nunit-framework/issues/21
            // from must be less than or equal to to

            this.from = from;
            this.to = to;
            CompareFromAndTo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeConstraint"/> class for objects that did not implement <see cref="IComparable"/>.
        /// </summary>
        /// <remarks>The <paramref name="from"/> value must be less than or equal to the <paramref name="to"/> value.</remarks> 
        /// <param name="from">Must be less than or equal to the <paramref name="to"/> value.</param>
        /// <param name="to">Must be greater than or equal to the <paramref name="from"/> value.</param>
        /// <param name="comparer">Class that implements <seealso cref="IComparer"/> used to compare the objects.</param>
        public RangeConstraint(object from, object to, IComparer comparer) : base(from, to)
        {
            // from must be less than or equal to to
            this.comparer = ComparisonAdapter.For(comparer);
            this.from = from;
            this.to = to;
            CompareFromAndTo();
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get { return string.Format("in range ({0},{1})", from, to); }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if ( from == null || to == null || actual == null)
                throw new ArgumentException( "Cannot compare using a null reference", "actual" );
            CompareFromAndTo();
            bool isInsideRange = comparer.Compare(from, actual) <= 0 && comparer.Compare(to, actual) >= 0;
            return new ConstraintResult(this, actual, isInsideRange);
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer"/> and returns self.
        /// </summary>
        public RangeConstraint Using(IComparer comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use an <see cref="IComparer{T}"/> and returns self.
        /// </summary>
        public RangeConstraint Using<T>(IComparer<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        /// <summary>
        /// Modifies the constraint to use a <see cref="Comparison{T}"/> and returns self.
        /// </summary>
        public RangeConstraint Using<T>(Comparison<T> comparer)
        {
            this.comparer = ComparisonAdapter.For(comparer);
            return this;
        }

        private void CompareFromAndTo()
        {
            if (comparer.Compare(from, to) > 0)
                throw new ArgumentException("from must be less than to");
        }
    }
}