/**
 * MemPriority
 *
 * Copyright (c) 2014 Renato de Pontes Pereira.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
**/

/**
 * @module Behavior3JS
 **/

// namespace:
using Behavior3CSharp.Core;
namespace Behavior3CSharp.Composites
{

    /**
     * MemPriority is similar to Priority node, but when a child returns a 
     * `RUNNING` state, its index is recorded and in the next tick the, MemPriority 
     * calls the child recorded directly, without calling previous children again.
     *
     * @class MemPriority
     * @extends Composite
    **/
    public class MemPriority : Composite
    {
        /**
         * Node name. Default to `MemPriority`.
         *
         * @property name
         * @type {String}
         * @readonly
        **/
        public const string name = "MemPriority";

        protected int _runningChild = 0;

        public MemPriority(B3Settings settings)
            : base(settings)
        {

        }

        /**
         * Open method.
         *
         * @method open
         * @param {b3.Tick} tick A tick instance.
        **/
        protected override void OnOpen(Tick tick)
        {
            _runningChild = 0;
        }

        /**
         * Tick method.
         *
         * @method tick
         * @param {b3.Tick} tick A tick instance.
         * @returns {Constant} A state constant.
        **/
        protected override B3Status OnTick(Tick tick)
        {
            for (var i = _runningChild; i < this._children.Count; i++)
            {
                B3Status status = this._children[i].Execute(tick);

                if (status != B3Status.FAILURE)
                {
                    if (status == B3Status.RUNNING)
                    {
                        _runningChild = i;
                    }
                    return status;
                }
            }

            return B3Status.FAILURE;
        }
    }
}