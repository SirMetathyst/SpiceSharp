﻿using System;
using SpiceSharp.Behaviors;
using SpiceSharp.Circuits;
using SpiceSharp.Diagnostics;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A class that represents a circuit component/device.
    /// It can be connected in a circuit and it also has parameters.
    /// </summary>
    public abstract class Component : Entity
    {
        /// <summary>
        /// Private variables
        /// </summary>
        private Identifier[] _connections;
        private int[] _indices;

        /// <summary>
        /// Gets the number of nodes
        /// </summary>
        public virtual int PinCount => _connections.Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the component</param>
        protected Component(Identifier name, int nodeCount)
            : base(name)
        {
            // Initialize
            if (nodeCount > 0)
            {
                _connections = new Identifier[nodeCount];
                _indices = new int[nodeCount];
            }
            else
            {
                _connections = null;
                _indices = null;
            }
        }

        /// <summary>
        /// Gets the model of the circuit component (if any)
        /// </summary>
        public Entity Model { get; protected set; } = null;

        /// <summary>
        /// Connect the component in the circuit
        /// </summary>
        /// <param name="nodes"></param>
        public void Connect(params Identifier[] nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (nodes.Length != _connections.Length)
                throw new CircuitException("{0}: Node count mismatch. {1} given, {2} expected.".FormatString(Name, nodes.Length, _connections.Length));
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null)
                    throw new ArgumentNullException("node " + (i + 1));
                _connections[i] = nodes[i];
            }
        }

        /// <summary>
        /// Gets a behavior
        /// </summary>
        /// <typeparam name="T">Base behavior</typeparam>
        /// <param name="pool">Pool of all behaviors</param>
        /// <returns></returns>
        public override T GetBehavior<T>(BehaviorPool pool)
        {
            T behavior = base.GetBehavior<T>(pool);

            // Extra functionality for behaviors that can be connected
            if (behavior is IConnectedBehavior cb)
            {
                cb.Connect(_indices);
            }
            return behavior;
        }

        /// <summary>
        /// Build the data provider for setting up behaviors
        /// </summary>
        /// <param name="pool">All behaviors</param>
        /// <returns></returns>
        protected override SetupDataProvider BuildSetupDataProvider(BehaviorPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));
            var provider = base.BuildSetupDataProvider(pool);

            // Add our model parameters and behaviors
            if (Model != null)
            {
                provider.Add("model", Model.ParameterSets);
                provider.Add("model", pool.GetEntityBehaviors(Model.Name));
            }

            return provider;
        }

        /// <summary>
        /// Gets the connection of the component
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public virtual Identifier GetNode(int index)
        {
            if (index < 0 || index >= _connections.Length)
                throw new CircuitException("Invalid node {0}".FormatString(index));
            return _connections[index];
        }

        /// <summary>
        /// Gets the node index of the component
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public virtual int GetNodeIndex(int index)
        {
            if (index < 0 || index >= _connections.Length)
                throw new CircuitException("Invalid node {0}".FormatString(index));
            return _indices[index];
        }

        /// <summary>
        /// Helper function for binding nodes to the circuit
        /// </summary>
        /// <param name="circuit"></param>
        /// <returns></returns>
        protected Node[] BindNodes(Circuit circuit)
        {
            if (circuit == null)
                throw new ArgumentNullException(nameof(circuit));

            // Map connected nodes
            Node[] nodes = new Node[_connections.Length];
            for (int i = 0; i < _connections.Length; i++)
            {
                nodes[i] = circuit.Nodes.Map(_connections[i]);
                _indices[i] = nodes[i].Index;
            }

            // Return all nodes
            return nodes;
        }
    }
}
