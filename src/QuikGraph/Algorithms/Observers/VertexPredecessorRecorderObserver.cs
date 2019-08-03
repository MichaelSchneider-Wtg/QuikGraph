using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using static QuikGraph.Utils.DisposableHelpers;

namespace QuikGraph.Algorithms.Observers
{
    /// <summary>
    /// Recorder of vertices predecessors (undirected).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class VertexPredecessorRecorderObserver<TVertex, TEdge> : IObserver<ITreeBuilderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPredecessorRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        public VertexPredecessorRecorderObserver()
            : this(new Dictionary<TVertex, TEdge>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPredecessorRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="vertexPredecessors">Vertex predecessors.</param>
        public VertexPredecessorRecorderObserver(
            [NotNull] IDictionary<TVertex, TEdge> vertexPredecessors)
        {
            if (vertexPredecessors is null)
                throw new ArgumentNullException(nameof(vertexPredecessors));

            VertexPredecessors = vertexPredecessors;
        }

        /// <summary>
        /// Vertex predecessors.
        /// </summary>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [NotNull]
        public IDictionary<TVertex, TEdge> VertexPredecessors { get; }

        #region IObserver<TAlgorithm>

        /// <inheritdoc />
        public IDisposable Attach(ITreeBuilderAlgorithm<TVertex, TEdge> algorithm)
        {
            algorithm.TreeEdge += OnEdgeDiscovered;
            return Finally(() => algorithm.TreeEdge -= OnEdgeDiscovered);
        }

        #endregion

        private void OnEdgeDiscovered([NotNull] TEdge edge)
        {
            VertexPredecessors[edge.Target] = edge;
        }

        /// <summary>
        /// Tries to get the predecessor path, if reachable.
        /// </summary>
        /// <param name="vertex">Path ending vertex.</param>
        /// <param name="path">Path to the ending vertex.</param>
        /// <returns>True if a path was found, false otherwise.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        public bool TryGetPath(TVertex vertex, out IEnumerable<TEdge> path)
        {
            return VertexPredecessors.TryGetPath(vertex, out path);
        }
    }
}
