/**
 * Composite
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
using System.Collections.Generic;
namespace Behavior3CSharp.Core
{

    /**
     * Composite is the base class for all composite nodes. Thus, if you want to 
     * create new custom composite nodes, you need to inherit from this class. 
     * 
     * When creating composite nodes, you will need to propagate the tick signal to
     * the children nodes manually. To do that, override the `tick` method and call 
     * the `_execute` method on all nodes. For instance, take a look at how the 
     * Sequence node inherit this class and how it call its children:
     *
     *
     *     // Inherit from Composite, using the util function Class.
     *     var Sequence = b3.Class(b3.Composite);
     *     var p = Sequence.prototype;
     *
     *         // Remember to set the name of the node. 
     *         p.name = 'Sequence';
     *         
     *         // Override the tick function
     *         p.tick = function(tick) {
     *
     *             // Iterates over the children
     *             for (var i=0; i<this.children.length; i++) {
     *
     *                 // Propagate the tick
     *                 var status = this.children[i]._execute(tick);
     * 
     *                 if (status !== b3.SUCCESS) {
     *                     return status;
     *                 }
     *             }
     *
     *             return b3.SUCCESS;
     *         }
     *
     * @class Composite
     * @extends BaseNode
    **/
    public class Composite : BaseNode
    {

        /**
         * Node category. Default to `b3.COMPOSITE`.
         *
         * @property category
         * @type {String}
         * @readonly
        **/
        public readonly string category = B3.COMPOSITE;

        protected List<BaseNode> _children;

        /**
         * Initialization method.
         *
         * @method initialize
         * @constructor
        **/
        public Composite(B3Settings settings):base(settings)
        {
            _category = B3.COMPOSITE;
            _children = new List<BaseNode>();
        }

        public int GetChildCount()
        {
            return _children.Count;
        }

        //GetChild
        public BaseNode GetChild(int index)
        {
            return this._children[index];
        }

        //AddChild
        public void AddChild(BaseNode node)
        {
            _children.Add(node);
        }
    }
}