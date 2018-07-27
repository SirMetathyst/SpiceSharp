﻿using System;
using SpiceSharp.Algebra.Solve;

// ReSharper disable once CheckNamespace
namespace SpiceSharp.Algebra
{
    /// <summary>
    /// Template for a solver
    /// </summary>
    /// <typeparam name="T">Base type</typeparam>
    public abstract class Solver<T> : SparseLinearSystem<T> where T : IFormattable, IEquatable<T>
    {
        /// <summary>
        /// Number of fill-ins in the matrix generated by the solver
        /// </summary>
        public int Fillins { get; private set; }

        /// <summary>
        /// Gets or sets a flag that reordering is required
        /// </summary>
        public bool NeedsReordering { get; set; }

        /// <summary>
        /// Gets whether or not the solver is factored
        /// </summary>
        public bool IsFactored { get; protected set; }

        /// <summary>
        /// Gets the pivot strategy used
        /// </summary>
        public PivotStrategy<T> Strategy { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strategy">Strategy</param>
        protected Solver(PivotStrategy<T> strategy)
        {
            NeedsReordering = true;
            Strategy = strategy;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strategy">Strategy</param>
        /// <param name="size">Size</param>
        protected Solver(PivotStrategy<T> strategy, int size)
            : base(size)
        {
            NeedsReordering = true;
            Strategy = strategy;
        }

        /// <summary>
        /// Solve
        /// </summary>
        /// <param name="solution">Solution vector</param>
        public abstract void Solve(Vector<T> solution);

        /// <summary>
        /// Solve the transposed problem
        /// </summary>
        /// <param name="solution">Solution vector</param>
        public abstract void SolveTransposed(Vector<T> solution);

        /// <summary>
        /// Factor the matrix
        /// </summary>
        /// <returns>True if factoring was successful</returns>
        public abstract bool Factor();

        /// <summary>
        /// Order and factor the matrix
        /// </summary>
        public abstract void OrderAndFactor();

        /// <summary>
        /// Move a chosen pivot to the diagonal
        /// </summary>
        /// <param name="pivot">Pivot</param>
        /// <param name="step">Step</param>
        public void MovePivot(MatrixElement<T> pivot, int step)
        {
            if (pivot == null)
                throw new ArgumentNullException(nameof(pivot));
            Strategy.MovePivot(Matrix, Rhs, pivot, step);

            // Move the pivot in the matrix
            var row = pivot.Row;
            var column = pivot.Column;
            if (row != step)
                SwapRows(row, step);
            if (column != step)
                SwapColumns(column, step);

            Strategy.Update(Matrix, pivot, step);
        }

        /// <summary>
        /// Create a fillin
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        /// <returns></returns>
        protected virtual MatrixElement<T> CreateFillin(int row, int column)
        {
            var result = Matrix.GetElement(row, column);
            Fillins++;
            return result;
        }

        /// <summary>
        /// Clear the linear equations
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            IsFactored = false;
        }
    }
}
