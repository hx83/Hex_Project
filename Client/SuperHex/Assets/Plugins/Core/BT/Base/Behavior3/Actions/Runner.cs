/**
 * Runner
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
namespace Behavior3CSharp.Actions
{
    /**
     * This action node returns RUNNING always.
     *
     * @class Runner
     * @extends Action
    **/
    public class Runner : Action
    {

        /**
         * Node name. Default to `Runner`.
         *
         * @property name
         * @type {String}
         * @readonly
        **/
        public const string name = "Runner";

        public Runner(B3Settings settings)
            : base(settings)
        {

        }

        /**
         * Tick method.
         *
         * @method tick
         * @param {b3.Tick} tick A tick instance.
         * @returns {Constant} Always return `b3.RUNNING`.
        **/
        protected override B3Status OnTick(Tick tick)
        {
            return B3Status.RUNNING;
        }

    }
}