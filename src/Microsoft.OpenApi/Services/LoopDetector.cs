using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Services
{
    internal class LoopDetector
    {
        private readonly Dictionary<Type, Stack<object>> _loopStacks = new Dictionary<Type, Stack<object>>();

        /// <summary>
        /// Maintain history of traversals to avoid stack overflows from cycles
        /// </summary>
        /// <param name="loopId">Any unique identifier for a stack.</param>
        /// <param name="key">Identifier used for current context.</param>
        /// <returns>If method returns false a loop was detected and the key is not added.</returns>
        public bool PushLoop<T>(T key)
        {
            Stack<object> stack;
            if (!_loopStacks.TryGetValue(typeof(T), out stack))
            {
                stack = new Stack<object>();
                _loopStacks.Add(typeof(T), stack);
            }

            if (!stack.Contains(key))
            {
                stack.Push(key);
                return true;
            }
            else
            {
                return false;  // Loop detected
            }
        }

        /// <summary>
        /// Exit from the context in cycle detection
        /// </summary>
        /// <param name="loopid">Identifier of loop</param>
        public void PopLoop<T>()
        {
            if (_loopStacks[typeof(T)].Count > 0)
            {
                _loopStacks[typeof(T)].Pop();
            }
        }

        public void SaveLoop<T>(T loop)
        {
            if (!Loops.ContainsKey(typeof(T)))
            {
                Loops[typeof(T)] = new List<object>();
            }
            Loops[typeof(T)].Add(loop);
        }

        /// <summary>
        /// List of Loops detected
        /// </summary>
        public Dictionary<Type, List<object>> Loops { get; } = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Reset loop tracking stack
        /// </summary>
        /// <param name="loopid">Identifier of loop to clear</param>
        internal void ClearLoop<T>()
        {
            _loopStacks[typeof(T)].Clear();
        }
    }
}
