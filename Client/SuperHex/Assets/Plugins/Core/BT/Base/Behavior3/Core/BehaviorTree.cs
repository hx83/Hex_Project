/**
 * BehaviorTree
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
namespace Behavior3CSharp.Core
{


    using LitJson;
    /**
     * The BehaviorTree class, as the name implies, represents the Behavior Tree 
     * structure.
     * 
     * There are two ways to construct a Behavior Tree: by manually setting the 
     * root node, or by loading it from a data structure (which can be loaded from 
     * a JSON). Both methods are shown in the examples below and better explained 
     * in the user guide.
     *
     * The tick method must be called periodically, in order to send the tick 
     * signal to all nodes in the tree, starting from the root. The method 
     * `BehaviorTree.tick` receives a target object and a blackboard as parameters.
     * The target object can be anything: a game agent, a system, a DOM object, 
     * etc. This target is not used by any piece of Behavior3JS, i.e., the target
     * object will only be used by custom nodes.
     * 
     * The blackboard is obligatory and must be an instance of `Blackboard`. This 
     * requirement is necessary due to the fact that neither `BehaviorTree` or any 
     * node will store the execution variables in its own object (e.g., the BT does
     * not store the target, information about opened nodes or number of times the 
     * tree was called). But because of this, you only need a single tree instance 
     * to control multiple (maybe hundreds) objects.
     * 
     * Manual construction of a Behavior Tree
     * --------------------------------------
     * 
     *     var tree = new b3.BehaviorTree();
     *  
     *     tree.root = new b3.Sequence({children:[
     *         new b3.Priority({children:[
     *             new MyCustomNode(),
     *             new MyCustomNode()
     *         ]}),
     *         ...
     *     ]});
     *     
     * 
     * Loading a Behavior Tree from data structure
     * -------------------------------------------
     * 
     *     var tree = new b3.BehaviorTree();
     *
     *     tree.load({
     *         'title'       : 'Behavior Tree title'
     *         'description' : 'My description'
     *         'root'        : 'node-id-1'
     *         'nodes'       : {
     *             'node-id-1' : {
     *                 'name'        : 'Priority', // this is the node type
     *                 'title'       : 'Root Node', 
     *                 'description' : 'Description', 
     *                 'children'    : ['node-id-2', 'node-id-3'], 
     *             },
     *             ...
     *         }
     *     })
     *     
     *
     * @class BehaviorTree
    **/
    using System;
    using System.Collections.Generic;
    public class BehaviorTree
    {
        /**
         * The tree id, must be unique. By default, created with `b3.createUUID`.
         * 
         * @property id
         * @type {String}
         * @readOnly
         */
        private string _id;

        public string Id
        {
            get { return _id; }
        }

        /**
         * The tree title.
         *
         * @property title
         * @type {String}
         */
        public string title;

        /**
         * Description of the tree.
         *
         * @property description
         * @type {String}
         */
        public string description;

        /**
         * A dictionary with (key-value) properties. Useful to define custom 
         * variables in the visual editor.
         *
         * @property properties
         * @type {Object}
         */
        public Dictionary<string, string> properties;

        /**
         * The reference to the root node. Must be an instance of `b3.BaseNode`.
         *
         * @property root
         * @type {BaseNode}
         */
        private BaseNode _root;

        private Dictionary<string, BaseNode> _nodeDic;

        private object _target;

        private Tick _tick;
        /**
         * Initialization method.
         *
         * @method initialize
         * @constructor
        **/
        public BehaviorTree(object target = null)
        {
            _target = target;
            this._id = B3.CreateUUID();
            this.title = "The behavior tree";
            this.description = "Default description";
            this.properties = new Dictionary<string, string>();
            _nodeDic = new Dictionary<string, BaseNode>();
            _tick = new Tick();
            _tick.Tree = this;
            _tick.Target = _target;
        }

        /**
         * This method loads a Behavior Tree from a data structure, populating this
         * object with the provided data. Notice that, the data structure must 
         * follow the format specified by Behavior3JS. Consult the guide to know 
         * more about this format.
         *
         * You probably want to use custom nodes in your BTs, thus, you need to 
         * provide the `names` object, in which this method can find the nodes by 
         * `names[NODE_NAME]`. This variable can be a namespace or a dictionary, 
         * as long as this method can find the node by its name, for example:
         *
         *     //json
         *     ...
         *     'node1': {
         *       'name': MyCustomNode,
         *       'title': ...
         *     }
         *     ...
         *     
         *     //code
         *     var bt = new b3.BehaviorTree();
         *     bt.load(data, {'MyCustomNode':MyCustomNode})
         *     
         * 
         * @method load
         * @param {Object} data The data structure representing a Behavior Tree.
         * @param {Object} [names] A namespace or dict containing custom nodes.
        **/
        public void Load(JsonData data)
        {
            this._id = (string)data["id"];
            this.title = (string)data["title"];
            this.description = (string)data["description"];
            string rootID = (string)data["root"];
            foreach (string treePropKey in data["properties"].Keys)
            {
                this.properties.Add(treePropKey, (string)data["properties"][treePropKey]);
            }

            // Create the node list (without connection between them)
            foreach (string nodeKey in data["nodes"].Keys)
            {
                JsonData nodeData = data["nodes"][nodeKey];
                B3Settings settings = new B3Settings();
                settings.Parse(nodeData);
                BaseNode node = Activator.CreateInstance(B3Config.GetClassType(settings.Name), settings) as BaseNode;
                _nodeDic.Add(node.Id, node);
            }
            // Connect the nodes
            _root = _nodeDic[rootID];

            foreach (BaseNode node in _nodeDic.Values)
            {
                if (node.Category == B3.COMPOSITE)
                {
                    int childCount = node.ChildKeys.Count;
                    for (int i = 0; i < childCount; i++)
                    {
                        (node as Composite).AddChild(_nodeDic[node.ChildKeys[i]]);
                    }
                }
                else if (node.Category == B3.DECORATOR)
                {
                    if (node.ChildKeys.Count > 0)
                    {
                        (node as Decorator).SetChild(_nodeDic[node.ChildKeys[0]]);
                    }
                }

            }
        }

        /**
         * This method dump the current BT into a data structure.
         *
         * Note: This method does not record the current node parameters. Thus, 
         * it may not be compatible with load for now.
         * 
         * @method dump
         * @returns {Object} A data object representing this tree.
        **/
        public BehaviorTree Dumplicate()
        {
            //var data = {};
            //var customNames = [];

            //data.title        = this.title;
            //data.description  = this.description;
            //data.root         = (this.root)? this.root.id:null;
            //data.properties   = this.properties;
            //data.nodes        = {};
            //data.custom_nodes = [];

            //if (!this.root) return data;

            //var stack = [this.root];
            //while (stack.length > 0) {
            //    var node = stack.pop();

            //    var spec = {};
            //    spec.id = node.id;
            //    spec.name = node.name;
            //    spec.title = node.title;
            //    spec.description = node.description;
            //    spec.properties = node.properties;
            //    spec.parameters = node.parameters;

            //    // verify custom node
            //    var nodeName = node.__proto__.name || node.name;
            //    if (!b3[nodeName] && customNames.indexOf(nodeName) < 0) {
            //        var subdata = {}
            //        subdata.name = nodeName;
            //        subdata.title = node.__proto__.title || node.title;
            //        subdata.category = node.category;

            //        customNames.push(nodeName);
            //        data.custom_nodes.push(subdata);
            //    }

            //    // store children/child
            //    if (node.category === b3.COMPOSITE && node.children) {
            //        var children = []
            //        for (var i=node.children.length-1; i>=0; i--) {
            //            children.push(node.children[i].id);
            //            stack.push(node.children[i]);
            //        }
            //        spec.children = children;
            //    } else if (node.category === b3.DECORATOR && node.child) {
            //        stack.push(node.child);
            //        spec.child = node.child.id;
            //    }

            //    data.nodes[node.id] = spec;
            //}

            return null;
        }

        /**
         * Propagates the tick signal through the tree, starting from the root.
         * 
         * This method receives a target object of any type (Object, Array, 
         * DOMElement, whatever) and a `Blackboard` instance. The target object has
         * no use at all for all Behavior3JS components, but surely is important 
         * for custom nodes. The blackboard instance is used by the tree and nodes 
         * to store execution variables (e.g., last node running) and is obligatory
         * to be a `Blackboard` instance (or an object with the same interface).
         * 
         * Internally, this method creates a Tick object, which will store the 
         * target and the blackboard objects.
         * 
         * Note: BehaviorTree stores a list of open nodes from last tick, if these 
         * nodes weren't called after the current tick, this method will close them 
         * automatically.
         * 
         * @method tick
         * @param {Object} target A target object.
         * @param {Blackboard} blackboard An instance of blackboard object.
         * @returns {Constant} The tick signal state.
        **/
        public B3Status Tick()
        {

            /* TICK NODE */
            B3Status state = this._root.Execute(_tick);

            /* CLOSE NODES FROM LAST TICK, IF NEEDED */
            //var lastOpenNodes = blackboard.get('openNodes', this.id);
            //var currOpenNodes = tick._openNodes.slice(0);

            //// does not close if it is still open in this tick
            //var start = 0;
            //for (var i=0; i<Math.min(lastOpenNodes.length, currOpenNodes.length); i++) {
            //    start = i+1;
            //    if (lastOpenNodes[i] !== currOpenNodes[i]) {
            //        break;
            //    } 
            //}

            //// close the nodes
            //for (var i=lastOpenNodes.length-1; i>=start; i--) {
            //    lastOpenNodes[i]._close(tick);
            //}
            return state;
        }


    }
}