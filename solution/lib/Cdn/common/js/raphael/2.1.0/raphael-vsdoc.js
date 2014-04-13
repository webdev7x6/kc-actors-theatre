// ┌─────────────────────────────────────────────────────────────────────┐ \\
// │ Raphaël 2.0.1 - JavaScript Vector Library - VSDOC                   │ \\
// ├─────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright (c) 2012 Forbes Lindesay (http://www.jepso.com)           │ \\
// │ Licensed under the MIT (http://raphaeljs.com/license.html) license. │ \\
// └─────────────────────────────────────────────────────────────────────┘ \\


// ┌─────────────────────────────────────────────────────────────────────┐ \\
// │ Raphaël 2.0.1 - JavaScript Vector Library                           │ \\
// ├─────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright (c) 2008-2011 Dmitry Baranovskiy (http://raphaeljs.com)   │ \\
// │ Copyright (c) 2008-2011 Sencha Labs (http://sencha.com)             │ \\
// │ Licensed under the MIT (http://raphaeljs.com/license.html) license. │ \\
// └─────────────────────────────────────────────────────────────────────┘ \\

// ┌──────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Eve 0.4.0 - JavaScript Events Library                                                │ \\
// ├──────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright (c) 2008-2011 Dmitry Baranovskiy (http://dmitry.baranovskiy.com/)          │ \\
// │ Licensed under the MIT (http://www.opensource.org/licenses/mit-license.php) license. │ \\
// └──────────────────────────────────────────────────────────────────────────────────────┘ \\


var eve = (function () {
    /// <field name="version" type="String">Current version of the library</field>
    var eve = function (name, scope, varargs) {
        /// <summary>Fires event with given name, given scope and other parameters.  Returns an array of all values returned by event handlers.</summary>
        /// <param name="name" type="String">name of the event, dot or slash separated.</param>
        /// <param name="scope" type="Object">context for the event handlers</param>
        /// <param name="varargs" type="Objects">The rest of arguments will be sent to event handlers</param>
        /// <returns type="Array" />
        return[];
    };

    eve.version = "";

    eve.listeners = function (name) {
        /// <summary>Internal method which gives you array of all event handlers that will be triggered by the given name.</summary>
        /// <param name="name" type="String">name of the event, dot or slash separated.</param>
        /// <returns type="Array" />
        return[];
    };

    eve.nt = function (subname) {
        /// <summary>Could be used inside event handler to figure out actual name of the event.</summary>
        /// <param name="subname" type="String" optional="true">subname of the event</param>
        /// <returns type="String/Boolean" />
        if (arguments.length == 1) {
            return false;
        }
        return "";
    };

    eve.on = function (name, f) {
        /// <summary>Binds given event handler with a given name. You can use wildcards “*” for the names.
        /// Returned function accepts a single numeric parameter that represents z-index of the handler.
        /// It is an optional feature and only used when you need to ensure that some subset of handlers will be invoked in a given order, despite of the order of assignment.</summary>
        /// <param name="name" type="String">name of the event, dot or slash separated.</param>
        /// <param name="f" type="Function">event handler function</param>
        /// <returns type="Function" />
        return function (zIndex) {
        };
    };

    eve.once = function (name, f) {
        /// <summary>Binds given event handler with a given name to only run once then unbind itself.
        /// Returned function accepts a single numeric parameter that represents z-index of the handler.
        /// It is an optional feature and only used when you need to ensure that some subset of handlers will be invoked in a given order, despite of the order of assignment.</summary>
        /// <param name="name" type="String">name of the event, dot or slash separated.</param>
        /// <param name="f" type="Function">event handler function</param>
        /// <returns type="Function" />
        return eve.on(name, f);
    };

    eve.stop = function () {
        /// <summary>Is used inside an event handler to stop the event, preventing any subsequent listeners from firing.</summary>
    };

    eve.unbind = function (name, f) {
        /// <summary>Removes given function from the list of event listeners assigned to given name.</summary>
        /// <param name="name" type="String">name of the event, dot or slash separated.</param>
        /// <param name="f" type="Function">event handler function</param>
    };

    return eve;
}());

// ┌─────────────────────────────────────────────────────────────────────┐ \\
// │ "Raphaël 2.0.1" - JavaScript Vector Library                         │ \\
// ├─────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright (c) 2008-2011 Dmitry Baranovskiy (http://raphaeljs.com)   │ \\
// │ Copyright (c) 2008-2011 Sencha Labs (http://sencha.com)             │ \\
// │ Licensed under the MIT (http://raphaeljs.com/license.html) license. │ \\
// └─────────────────────────────────────────────────────────────────────┘ \\


var Raphael;

(function () {
    function Animation(anim, ms) {
    }
    function Element() {
        /// <summary>Represents a raphael element on the screen</summary>
        /// <field name="id" type="Number">
        ///  Unique id of the element.
        ///  Especially usesful when you want to listen to events of the element,
        ///  because all events are fired in format &lt;module&gt;.&lt;action&gt;.&lt;id&gt;. Also useful for Paper.getById method.
        /// </field>
        /// <field name="next" type="Element">Reference to the next element in the hierachy</field>
        /// <field name="node" type="DOMObject">Gives you a reference to the DOM object, so you can assign event handlers or just mess around. Note: Don’t mess with it.</field>
        /// <field name="paper" type="Paper">Internal reference to “paper” where object drawn. Mainly for use in plugins and element extensions.</field>
        /// <field name="prev" type="Element">Reference to the previous element in the hierachy</field>
        /// <field name="raphael" type="Raphael">Internal reference to Raphael object. In case it is not available.</field>
        this.id = 0;
        this.next = this;
        this.node = document.createElement('div');
        this.paper = new Paper();
        this.prev = this;
        this.raphael = R;
    }
    function Matrix(a, b, c, d, e, f) {
        /// <field name="a" type="Number"></param>
        /// <field name="b" type="Number"></param>
        /// <field name="c" type="Number"></param>
        /// <field name="d" type="Number"></param>
        /// <field name="e" type="Number"></param>
        /// <field name="f" type="Number"></param>
        this.a = 1;
        this.b = 0;
        this.c = 0;
        this.d = 1;
        this.e = 0;
        this.f = 0;
    }
    function Paper() {
        ///<field name="ca" type="Object">Shortcut for Paper.customAttributes</field>
        ///<field name="customAttributes" type="Object">If you have a set of attributes that you would like to represent as a function of some number you can do it easily with custom attributes.</field>
        ///<field name="bottom" type="Element">Points to the bottom element on the paper</field>
        ///<field name="top" type="Element">Points to the top element on the paper</field>
        ///<field name="raphael" type="Raphael">Points to the Raphael object/function</field>
        this.ca = this.customAttributes = {};
        this.top = this.bottom = elproto;
        this.raphael = R;
    };
    var R = (function () {
        /// <field name="type" type="String">Can be SVG, VML or empty, depending on browser support.</field>
        /// <field name="svg" type="Boolean">true if browser supports SVG</field>
        /// <field name="vml" type="Boolean">true if browser supports VML</field>
        var R = function R(first) {
            /// <summary>Creates a canvas object on which to draw. You must do this first, as all future calls to drawing methods from this instance will be bound to this canvas.</summary>
            /// <returns type="Paper" />
            return new Paper();
        }
        R.svg = !(R.vml = false);
        R.type = "";
        R.fn = paperproto = Paper.prototype;
        R.el = Element.prototype;
        R.st = Set.prototype = Object.create(Element.prototype);
        return R;
    }());
    Raphael = R;
    function Set(items) {
        this.items = [];
        this.length = 0;
        this.type = "set";
        if (items) {
            for (var i = 0, ii = items.length; i < ii; i++) {
                if (items[i] && (items[i].constructor == elproto.constructor || items[i].constructor == Set)) {
                    this[this.items.length] = this.items[this.items.length] = items[i];
                    this.length++;
                }
            }
        }
    }
    //rgb is reused multiple times so this saves writing out the field descriptions each time.
    function rgb() {
        /// <field name="r" type="Number">red</field>
        /// <field name="g" type="Number">green</field>
        /// <field name="b" type="Number">blue</field>
        /// <field name="hex" type="String">colour in HTML/CSS format #xxxxxx</field>
        return { r: 0, g: 0, b: 0, hex: "" };
    }


    (function (animproto) {
        animproto.delay = function (delay) {
            /// <summary>Creates a copy of existing animation object with given delay.</summary>
            /// <param name="delay" type="Number">number of ms to pass between animation start and actual animation</param>
            /// <returns type="Object" />
            return new Animation(this.anim, this.ms);;
        };
        animproto.repeat = function (times) {
            /// <summary>Creates a copy of existing animation object with given repetition.</summary>
            /// <param name="times" type="Number">number iterations of animation. For infinite animation pass Infinity</param>
            /// <returns type="Object" />
            return new Animation(this.anim, this.ms);;
        };
    }(Animation.prototype));
    (function (elproto) {
        elproto.animate = function (paramsOrAnimation, ms, easing, callback) {
            /// <summary>Creates and starts animation for given element.</summary>
            /// <param name="params" type="Object">If an animation, skip the rest of the parameters.  If paramaters this should represent the final attributes of the element.</param>
            /// <param name="ms" type="Number">number of milliseconds for animation to run</param>
            /// <param name="easing" type="String">easing type. Accept one of Raphael.easing_formulas or CSS format: cubic‐bezier(XX, XX, XX, XX)</param>
            /// <param name="callback" type="Function">callback function.  Will be called at the end of animation.</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.animateWith = function (element, anim, params, ms, easing, callback) {
            return this;
        };
        elproto.attr = function (name, value) {
            /// <summary>Sets or gets attributes of the element.
            ///  It accepts either a name/value pair, an object of name/value pairs, an attribute name to get, an array of attribute names to retrieve as an array.</summary>
            /// <param name="name" type="String/Array/Object">Name of attribute, Array of attribute names or object specifying attribute values.</param>
            /// <param name="value" type="Object" optional="true">The value to set the attribute to.</param>
            /// <returns type="Element/Object/Array" />
            if (arguments.length === 1 && typeof name === "string") { return {}; }
            if (arguments.length === 1 && name.constructor == Array) { return[]; }
            return this;
        };
        elproto.click = function (handler) {
            /// <summary>Adds event handler for click to the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.clone = function () {
            /// <summary>Clone the current element</summary>
            /// <returns type="Element" />
            return this;
        };
        elproto.data = function (key, value) {
            /// <summary>Adds or retrieves a given value associated with given key. See also Element.removeData</summary>
            /// <param name="key" type="String">key to store data</param>
            /// <param name="vaue" optional="true">Description</param>
            /// <returns type="Element/Value" />
            if (arguments.length === 1) { return {}; }
            return this;
        };
        elproto.dblclick = function (handler) {
            /// <summary>Adds event handler for double click for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.drag = function (onmove, onstart, onend, move_scope, start_scope, end_scope) {
            /// <summary>Adds event handlers for drag of the element.</summary>
            /// <param name="onmove" type="Function">handler for moving. move(Number dx, Number dy, Number x, Number y, DOMevent event)</param>
            /// <param name="onstart" type="Function">handler for drag start. start(Number x, Number y, DOMevent event)</param>
            /// <param name="onend" type="Function">handler for drag end. end(DOMevent event)</param>
            /// <param name="move_scope" type="Object" optional="true">context for moving handler</param>
            /// <param name="start_scope" type="Object" optional="true">context for drag start handler</param>
            /// <param name="end_scope" type="Object" optional="true">context for drag end handler</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.getBBox = function (isWithoutTransform) {
            /// <summary>Return bounding box for a given element</summary>
            /// <param name="isWithoutTransform" type="Boolean" optional="true">flag, true if you want to have bounding box before transformations. Default is false.</param>
            /// <returns type="Object" />
            /// <field name="x" type="Number">top left corner x</field>
            /// <field name="y" type="Number">top left corner y</field>
            /// <field name="width" type="Number">width</field>
            /// <field name="height" type="Number">height</field>
            return { x: 0, y: 0, width: 0, height: 0 };
        };
        elproto.getPointAtLength = function (length) {
            /// <summary>Return coordinates of the point located at the given length on the given path. Only works for element of “path” type.</summary>
            /// <param name="length" type="Number"></param>
            /// <returns type="Object" />
            /// <field name="x" type="Number">x coordinate</field>
            /// <field name="y" type="Number">y coordinate</field>
            /// <field name="alpha" type="Number">angle of derivative</field>
            return { x: 0, y: 0, alpha: 0 };
        };
        elproto.getSubpath = function (from, to) {
            /// <summary>Return subpath of a given element from given length to given length. Only works for element of “path” type.</summary>
            /// <param name="from" type="Number">position of the start of the segment</param>
            /// <param name="to" type="Number">position of the end of the segment</param>
            /// <returns type="String" />
            return "";
        };
        elproto.getTotalLength = function () {
            /// <summary>Returns length of the path in pixels. Only works for element of “path” type.</summary>
            /// <returns type="Number" />
            return 0;
        };
        elproto.glow = function (glow) {
            /// <summary>
            ///   Return set of elements that create glow-like effect around given element. See Paper.set.
            ///   Note: Glow is not connected to the element. If you change element attributes it won’t adjust itself.
            /// </summary>
            /// <param name="glow" type="Object" optional="true">{width:10, fill: false, opacity:0.5, offseetx:0, offsety:0, color:black}</param>
            /// <returns type="Set" />
            return new Set();
        };
        elproto.hide = function () {
            /// <summary>Makes the element invisible</summary>
            /// <returns type="Element" />
            return this;
        };
        elproto.hover = function (f_in, f_out, icontext, ocontext) {
            /// <summary>Adds event handlers for hover for the element.</summary>
            /// <param name="f_in" type="Function">Handler for hover in</param>
            /// <param name="f_out" type="Function">Handler for hover out</param>
            /// <param name="icontext" type="Object" optional="true">Context for hover in handler</param>
            /// <param name="ocontext" type="Object" optional="true">Context for hover out handler</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.insertAfter = function (element) {
            /// <summary>Inserts current object after the given one.</summary>
            return this;
        };
        elproto.insertBefore = function (element) {
            /// <summary>Inserts current object before the given one.</summary>
            return this;
        };
        elproto.mousedown = function (handler) {
            /// <summary>Adds event handler for mousedown for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.mousemove = function (handler) {
            /// <summary>Adds event handler for mousemove for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.mouseout = function (handler) {
            /// <summary>Adds event handler for mouseout for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.mouseover = function (handler) {
            /// <summary>Adds event handler for mouseover for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.mouseup = function (handler) {
            /// <summary>Adds event handler for mouseup for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.onDragOver = function (f) {
            /// <summary>Shortcut for assigning event handler for drag.over.<id> event, where id is id of the element (see Element.id).</summary>
            /// <param name="f" type="Function">handler for event, first argument would be the element you are dragging over</param>
        };
        elproto.pause = function (anim) {
            /// <summary>Stops animation of the element with ability to resume it later on.</summary>
            /// <param name="anim" type="Animation" optional="true">Animation object</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.remove = function () {
            /// <summary>Removes element from the paper</summary>
        };
        elproto.removeData = function (key) {
            /// <summary>Removes value associated with an element by given key. If key is not provided, removes all the data of the element.</summary>
            /// <param name="key" type="String" optional="true">key</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.resume = function (anim) {
            /// <summary>Resumes animation if it was paused with Element.pause method.</summary>
            /// <param name="anim" type="Animation" optional="true">Animation object</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.rotate = function (deg, cx, cy) {
            /// <summary>Adds rotation by given angle around given point to the list of transformations of the element.  If centre is not speficied, the centre of the shape is used.</summary>
            /// <param name="deg" type="Number">angle in degrees</param>
            /// <param name="cx" type="Number" optional="true">x coordinate of the centre of rotation</param>
            /// <param name="cy" type="Number" optional="true">y coordinate of the centre of rotation</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.scale = function (sx, sy, cx, cy) {
            /// <summary>Adds scale by given amount relative to given point to the list of transformations of the element.</summary>
            /// <param name="sx" type="Number">horizontal scale amount</param>
            /// <param name="sy" type="Number">vertical scale amount</param>
            /// <param name="cx" type="Number" optional="true">x coordinate of the centre of scale</param>
            /// <param name="cy" type="Number" optional="true">y coordinate of centre of scale</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.setTime = function (anim, value) {
            /// <summary>Sets the status of animation of the element in milliseconds. Similar to Element.status method.</summary>
            /// <param name="anim" type="Animation">Animation object</param>
            /// <param name="value" type="Number">Number of milliseconds from teh beginning of the animation</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.show = function () {
            /// <summary>Makes element visible. See Element.hide.</summary>
            /// <returns type="Element" />
            return this;
        };
        elproto.status = function (anim, value) {
            /// <summary>Gets or sets the status of animation of the element.</summary>
            /// <param name="anim" type="Animation" optional="true">Animation Object</param>
            /// <param name="value" type="Number" optional="true">
            ///   0 – 1. If specified, method works like a setter and sets the status of a given animation to the value.
            ///   This will cause animation to jump to the given position.
            /// </param>
            /// <returns type="Number/Array/Element" />
            if (arguments.length === 0) { return[]; }
            if (arguments.length === 1) { return 0; }
            return this;
        };
        elproto.stop = function (anim) {
            /// <summary>Stops animation of the element.</summary>
            /// <param name="anim" type="Animation" optional="true">Animation object</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.toBack = function () {
            /// <summary>Moves the element so it is the furthest from the viewer’s eyes, behind other elements.</summary>
            /// <returns type="Element" />
            return this;
        };
        elproto.toFront = function () {
            /// <summary>Moves the element so it is the closest to the viewer’s eyes, on top of other elements.</summary>
            /// <returns type="Element" />
            return this;
        };
        elproto.touchcancel = function (handler) {
            /// <summary>Adds event handler for touchcancel for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.touchend = function (handler) {
            /// <summary>Adds event handler for touchend for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.touchmove = function (handler) {
            /// <summary>Adds event handler for touchmove for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.touchstart = function (handler) {
            /// <summary>Adds event handler for touchstart for the element</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.transform = function (tstr) {
            /// <summary>Adds transformation to the element which is separate to other attributes, i.e. translation doesn’t change x or y of the rectange.</summary>
            /// <param name="tstr" type="String" optional="true">transformation string, if not specified, the current transformation string will be returned.</param>
            /// <returns type="Element/String" />
            if (arguments.length === 0){ return ""; }
            return this;
        };
        elproto.translate = function (dx, dy) {
            /// <summary>Adds translation by given amount to the list of transformations of the element.</summary>
            /// <param name="dx" type="Number">horisontal shift</param>
            /// <param name="dy" type="Number">vertial shift</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.unclick = function (handler) {
            /// <summary>Removes event handler for click for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.undblclick = function (handler) {
            /// <summary>Removes event handler for double click for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.undrag = function () {
            /// <summary>Removes all drag event handlers from given element.</summary>
        };
        elproto.unhover = function (f_in, f_out) {
            /// <summary>Removes event handlers for hover for the element.</summary>
            /// <param name="f_in" type="Function">Handler for hover in</param>
            /// <param name="f_out" type="Function">Handler for hover out</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.unmousedown = function (handler) {
            /// <summary>Removes event handler for mousedown for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.unmousemove = function (handler) {
            /// <summary>Removes event handler for mousemove for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.unmouseout = function (handler) {
            /// <summary>Removes event handler for mouseout for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.unmouseover = function (handler) {
            /// <summary>Removes event handler for mouseover for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.unmouseup = function (handler) {
            /// <summary>Removes event handler for mouseup for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.untouchcancel = function (handler) {
            /// <summary>Removes event handler for touchcancel for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.untouchend = function (handler) {
            /// <summary>Removes event handler for touchend for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.untouchmove = function (handler) {
            /// <summary>Removes event handler for touchmove for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
        elproto.untouchstart = function (handler) {
            /// <summary>Removes event handler for touchstart for the element.</summary>
            /// <param name="handler" type="Function">handler for the event</param>
            /// <returns type="Element" />
            return this;
        };
    }(Element.prototype));
    (function (matrixproto) {
        matrixproto.add = function (a, b, c, d, e, f) {
            /// <summary>Adds given matrix to existing one.</summary>
            /// <param name="a" type="Number"></param>
            /// <param name="b" type="Number"></param>
            /// <param name="c" type="Number"></param>
            /// <param name="d" type="Number"></param>
            /// <param name="e" type="Number"></param>
            /// <param name="f" type="Number"></param>
        };
        matrixproto.clone = function () {
            /// <summary>Returns copy of the matrix</summary>
            /// <returns type="Matrix" />
            return new Matrix(this.a, this.b, this.c, this.d, this.e, this.f);
        };
        matrixproto.invert = function () {
            /// <summary>Returns inverted version of the matrix</summary>
            /// <returns type="Matrix" />
            var me = this,
                x = me.a * me.d - me.b * me.c;
            return new Matrix(0, 0, 0, 0, 0, 0);
        };
        matrixproto.rotate = function (a, x, y) {
            /// <summary>Rotates the matrix</summary>
            /// <param name="a" type="Number"></param>
            /// <param name="x" type="Number" optional="true"></param>
            /// <param name="y" type="Number" optional="true"></param>
        };
        matrixproto.scale = function (x, y, cx, cy) {
            /// <summary>Scales the matrix</summary>
            /// <param name="x" type="Number"></param>
            /// <param name="y" type="Number" optional="true"></param>
            /// <param name="cx" type="Number" optional="true"></param>
            /// <param name="cy" type="Number" optional="true"></param>
        };
        matrixproto.split = function () {
            /// <summary>Splits matrix into primitive transformations</summary>
            /// <returns type="Object" />
            /// <field name="dx" type="Number">translation by x</field>
            /// <field name="dy" type="Number">translation by y</field>
            /// <field name="scalex" type="Number">scale by x</field>
            /// <field name="scaley" type="Number">scale by y</field>
            /// <field name="shear" type="Number">shear</field>
            /// <field name="rotate" type="Number">rotation in deg</field>
            /// <field name="isSimple" type="Boolean">could it be represented via simple transformations</field>
            return { dx: 0, dy: 0, scalex: 0, scaley: 0, shear: 0, rotate: 0, isSimple: true };
        };
        matrixproto.toTransformString = function () {
            /// <summary>Return transform string that represents given matrix</summary>
            /// <returns type="String" />
            return "";
        };
        matrixproto.translate = function (x, y) {
            /// <summary>Translate the matrix</summary>
            /// <param name="x" type="Number"></param>
            /// <param name="y" type="Number"></param>
        };
        matrixproto.x = function (x, y) {
            /// <summary>Return x coordinate for given point after transformation described by the matrix.</summary>
            /// <param name="x" type="Number"></param>
            /// <param name="y" type="Number"></param>
            /// <returns type="Number" />
            return 0;
        };
        matrixproto.y = function (x, y) {
            /// <summary>Return y coordinate for given point after transformation described by the matrix.</summary>
            /// <param name="x" type="Number"></param>
            /// <param name="y" type="Number"></param>
            /// <returns type="Number" />
            return 0;
        };
    })(Matrix.prototype);
    (function (paperproto) {
        paperproto.add = function (json) {
            /// <summary>Imports elements in JSON array in format {type: type, &lt;attributes&gt;}</summary>
            /// <param name="json" type="Array"></param>
            /// <returns type="Set" />
            return new Set();
        };
        paperproto.circle = function (x, y, r) {
            return new Element();
        };
        paperproto.clear = function () {
            /// <summary>Clears the paper, i.e. removes all the elements.</summary>
        };
        paperproto.ellipse = function (x, y, rx, ry) {
            /// <summary>Draws an ellipse</summary>
            /// <param name="x" type="Number">x coordinate of the centre</param>
            /// <param name="y" type="Number">y coordinate of the centre</param>
            /// <param name="rx" type="Number">horizontal radius</param>
            /// <param name="ry" type="Number">vertical radius</param>
            /// <returns type="Element" />
            return new Element();
        };
        paperproto.forEach = function (callback, thisArg) {
            /// <summary>
            ///  Executes given function for each element on the paper
            ///
            ///  If callback function returns false it will stop loop running.
            /// </summary>
            /// <param name="callback" type="Function">function to run</param>
            /// <param name="thisArg" type="Object" optional="true">context object for the callback</param>
            /// <returns type="Paper" />
            return this;
        };
        paperproto.getById = function (id) {
            /// <summary>Returns you element by its internal ID.</summary>
            /// <param name="id" type="Number">id</param>
            /// <returns type="Element" />
            return new Element();
        };
        paperproto.getElementByPoint = function (x, y) {
            /// <summary>Returns you topmost element under given point.</summary>
            /// <param name="x" type="Number">x coordinate from the top left corner of the window</param>
            /// <param name="y" type="Number">y coordinate from the top left corner of the window</param>
            /// <returns type="Element" />
            return new Element();
        };
        paperproto.getFont = function (family, weight, style, stretch) {
            /// <summary>Finds font object in the registered fonts by given parameters. You could specify only one word from the font name, like “Myriad” for “Myriad Pro”.</summary>
            /// <param name="family" type="String">font family name or any word from it</param>
            /// <param name="weight" type="String">font weight</param>
            /// <param name="style" type="String">font style</param>
            /// <param name="stretch" type="String">font stretch</param>
            /// <returns type="Object" />
            return { face: { "font-weight": 0, "font-style": "", "font-style": "", "font-stretch": "" } };
        };
        paperproto.image = function (src, x, y, width, height) {
            /// <summary>Embeds an image into the surface</summary>
            /// <param name="src" type="String">URI of the source image</param>
            /// <param name="x" type="Number">x coordinate position</param>
            /// <param name="y" type="Number">y coordinate position</param>
            /// <param name="width" type="Number">Width of the image</param>
            /// <param name="height" type="Number">Height of the image</param>
            /// <returns type="Element" />
            return new Element();
        };
        paperproto.path = function (pathString) {
            /// <summary>Creates a path element by given path data string</summary>
            /// <param name="pathString" type="String">path string in SVG format.</param>
            /// <returns type="Element" />
            //http://raphaeljs.com/reference.html#Paper.path
            return new Element();
        };
        paperproto.print = function (x, y, string, font, size, origin, letter_spacing) {
            /// <summary>
            ///  Creates set of shapes to represent given font at given position with given size.
            ///  Result of the method is set object (see Paper.set) which contains each letter as separate path object.
            /// </summary>
            /// <param name="x" type="Number">x position of the text</param>
            /// <param name="y" type="Number">y position of the text</param>
            /// <param name="text" type="String">text to print</param>
            /// <param name="size" type="Number" optional="true">size of the font, default is 16</param>
            /// <param name="origin" type="String" optional="true">Could be "baseline" or "middle", default is "middle"</param>
            /// <param name="letter_spacing" type="Number" optional="true">number in range -1..1, default is 0</param>
            /// <returns type="Set" />
            return new Set();
        };
        paperproto.rect = function (x, y, width, height, radius) {
            /// <summary>Draws a rectangle</summary>
            /// <param name="x" type="Number">x coordinate of top left corner</param>
            /// <param name="y" type="Number">y coordinate of top left corner</param>
            /// <param name="width" type="Number">width</param>
            /// <param name="height" type="Number">height</param>
            /// <param name="radius" type="Number" optional="true">Radius for rounded corners, defaults to 0</param>
            /// <returns type="Element" />
            return new Element();
        };
        paperproto.remove = function () {
            /// <summary>Removes the paper from the DOM.</summary>
        };
        //Firefox and IE9 bug workaround method
        paperproto.renderfix = function () {
            /// <summary>Fixes the issue of Firefox and IE9 regarding subpixel rendering.
            /// If paper is dependant on other elements after reflow it could shift half pixel which cause for lines to lost their crispness.
            /// This method fixes the issue.</summary>
        };
        // WebKit rendering bug workaround method
        paperproto.safari = function () {
            /// <summary>There is an inconvenient rendering bug in Safari (WebKit): sometimes the rendering should be forced. This method should help with dealing with this bug.</summary>
        };
        paperproto.set = function () {
            /// <summary>
            ///   Creates array-like object to keep and operate several elements at once.
            ///   Warning: it doesn’t create any elements for itself in the page, it just groups existing elements.
            ///   Sets act as pseudo elements — all methods available to an element can be used on a set.
            /// </summary>
            /// <returns type="Set" />
            return new Set();;
        };
        paperproto.setFinish = function () {
            /// <summary>See Paper.setStart. This method finishes catching and returns resulting set.</summary>
            /// <returns type="Set" />
            return new Set();
        };
        paperproto.setSize = function (width, height) {
            /// <summary>If you need to change dimensions of the canvas call this method</summary>
            /// <param name="width" type="Number">new width of the canvas</param>
            /// <param name="height" type="Number">new height of the canvas</param>
        };
        paperproto.setStart = function () {
            /// <summary>Creates Paper.set. All elements that will be created after calling this method and before calling Paper.setFinish will be added to the set.</summary>
        };
        paperproto.setViewBox = function (x, y, w, h, fit) {
            /// <summary>Sets the view box of the paper. Practically it gives you ability to zoom and pan whole paper surface by specifying new boundaries.</summary>
            /// <param name="x" type="Number">new x position, default is 0</param>
            /// <param name="y" type="Number">new y position, default is 0</param>
            /// <param name="w" type="Number">new width of the canvas</param>
            /// <param name="h" type="Number">new height of the canvas</param>
            /// <param name="fit" type="Boolean">true if you want graphics to fit into new boundary box</param>
        };
        paperproto.text = function (x, y, text) {
            /// <summary>Draws a text string.  If you need line breaks, put "\n" in the string.</summary>
            /// <param name="x" type="Number">x coordinate position</param>
            /// <param name="y" type="Number">y coordinate position</param>
            /// <param name="text" type="String">The text to draw</param>
            /// <returns type="Element" />
            return new Element();
        };
    }(Paper.prototype));
    (function (R) {
        R.angle = function (x1, y1, x2, y2, x3, y3) {
            /// <summary>Returns angle between two or three points in degrees</summary>
            /// <param name="x1" type="Number">x coord of first point</param>
            /// <param name="y1" type="Number">y coord of first point</param>
            /// <param name="x2" type="Number">x coord of second point</param>
            /// <param name="y2" type="Number">y coord of second point</param>
            /// <param name="x3" type="Number">x coord of third point</param>
            /// <param name="y3" type="Number">y coord of third point</param>
            /// <returns type="Number" />
            return 180;
        };
        R.animation = function (params, ms, easing, callback) {
            /// <summary>Creates an animation object that can be passed to the Element.animate or Element.animateWith methods. See also Animation.delay and Animation.repeat methods.</summary>
            /// <param name="params" type="Object">final attributes of element, see also Element.attr</param>
            /// <param name="ms" type="Number">number of milliseconds for animation to run</param>
            /// <param name="easing" type="String" optional="true">easing type.  Accept one of Raphael.easing_formulas or css format: cubic-bezier(XX, XX, XX, XX)</param>
            /// <param name="callback" type="Function" optional="true">callback function.  Will be called at the end of animation.</param>
            /// <returns type="Animation" />
            return new Animation();
        };
        R.color = function (clr) {
            /// <summary>Parses the color string and returns object with all values for the given color.</summary>
            /// <param name="clr" type="String">color string in one of the supported formats (see Raphael.getRGB)</param>
            /// <returns type="object" />
            return { r: 0, g: 0, b: 0, hex: 0, h: 0, s: 0, l: 0, v: 0 };
        };
        R.createUUID = function () {
            /// <summary>Returns RFC4122, version 4 ID</summary>
            return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx";
        };
        R.deg = function (rad) {
            /// <summary>Transform angle to degrees</summary>
            /// <param name="rad" type="Number">angle in radians</param>
            /// <returns type="Number" />
            return rad * 180 / PI % 360;
        };
        R.findDotsAtSegment = function (p1x, p1y, c1x, c1y, c2x, c2y, p2x, p2y, t) {
            /// <summary>Utility method Find dot coordinates on the given cubic bezier curve at the given t.</summary>
            /// <param name="p1x" type="Number">x of the first point of the curve</param>
            /// <param name="p1y" type="Number">y of the first point of the curve</param>
            /// <param name="c1x" type="Number">x of the first anchor of the curve</param>
            /// <param name="c1y" type="Number">y of the first anchor of the curve</param>
            /// <param name="c2x" type="Number">x of the second anchor of the curve</param>
            /// <param name="c2y" type="Number">y of the second anchor of the curve</param>
            /// <param name="p2x" type="Number">x of the second point of the curve</param>
            /// <param name="p2y" type="Number">y of the second point of the curve</param>
            /// <param name="t" type="Number">position on the curve (0..1)</param>
            /// <field name="x" type="Number">x coordinate of the point</field>
            /// <field name="y" type="Number">y coordinate of the point</field>
            /// <field name="m" type="Object">Coordinate of the left anchor</field>
            /// <field name="n" type="Object">Coordinate of the right anchor</field>
            /// <field name="start" type="Object">Coordinate of the start of the curve</field>
            /// <field name="end" type="Object">Coordinate of the end of the curve</field>
            /// <field name="alpha" type="Number">angle of the curve derivative at the point</field>
            return {
                x: 0,
                y: 0,
                m: { x: 0, y: 0 },
                n: { x: 0, y: 0 },
                start: { x: 0, y: 0 },
                end: { x: 0, y: 0 },
                alpha: 0
            };
        };
        R.format = function (token, params) {
            /// <summary>Simple format function. Replaces construction of type “{&lt;number&gt;}” to the corresponding argument.</summary>
            /// <param name="token" type="String">string to format</param>
            /// <param name="params" type="String(s)">rest of arguments will be treated as parameters for replacement</param>
            /// <returns type="String" />
            return "";
        };
        R.fullfill = function (token, json) {
            /// <summary>A little bit more advanced format function than Raphael.format. Replaces construction of type “{&lt;name&gt;}” to the corresponding argument.</summary>
            /// <param name="token" type="String">string to format</param>
            /// <param name="json" type="Object">Object which properties will be used as a replacement</param>
            /// <returns type="String" />
            return "";
        };
        R.getColor = function (value) {
            /// <summary>On each call returns next colour in the spectrum. To reset it back to red call Raphael.getColor.reset</summary>
            /// <param name="value" type="Number" optional="true">Brightness, default is 0.75</param>
            /// <returns type="String" />
            return "";
        };
        R.getColor.reset = function () {
            /// <summary>Resets spectrum position for Raphael.getColor back to red.</summary>
        };
        R.getPointAtLength = function (path, length) {
            /// <summary>Return coordinates of the point located at the given length on the given path.</summary>
            /// <param name="path" type="String">SVG path string</param>
            /// <param name="length" type="Number"></param>
            /// <returns type="Object" />
            /// <field name="x" type="Number">x coordinate</field>
            /// <field name="y" type="Number">y coordinate</field>
            /// <field name="alpha" type="Number">angle of derivative</field>
            return { x: 0, y: 0, alpha: 0 };
        };
        R.getRGB = function (colour) {
            /// <summary>Parses colour string as RTB object</summary>
            /// <param name="colour" type="String">colour string in one of the formats listed at http://raphaeljs.com/reference.html#Raphael.getRGB </param>
            /// <returns type="Object" />
            /// <field name="error" type="Boolean">true if string cant be parsed</field>
            var r = rgb();
            r.error = false;
            return r;
        };
        R.getSubpath = function (path, from, to) {
            /// <summary>Return subpath of a given path from given length to given length.</summary>
            /// <param name="path" type="String">SVG path string</param>
            /// <param name="from" type="Number">position of the start of the segment</param>
            /// <param name="to" type="Number">position of the end of the segment</param>
            /// <returns type="String" />
            return "";
        };
        R.getTotalLength = function (path) {
            /// <summary>Returns length of the given path in pixels.</summary>
            /// <param name="path" type="String">SVG path string</param>
            /// <returns type="Number" />
            return 0;
        };
        R.hsb = function (h, s, b) {
            /// <summary>Converts HSB values to hex representation of the colour.</summary>
            /// <param name="h" type="Number">hue</param>
            /// <param name="s" type="Number">saturation</param>
            /// <param name="b" type="Number">value or brightness</param>
            /// <returns type="String" />
            return "";
        };
        R.hsb2rgb = function (h, s, v, o) {
            /// <summary>Converts HSB values to RGB object.</summary>
            /// <param name="h" type="Number">hue</param>
            /// <param name="s" type="Number">saturation</param>
            /// <param name="v" type="Number">value or brightness</param>
            /// <returns type="Object" />
            return rgb();
        };
        R.hsl = function (h, s, l) {
            /// <summary>Converts HSL values to hex representation of the colour.</summary>
            /// <param name="h" type="Number">hue</param>
            /// <param name="s" type="Number">saturation</param>
            /// <param name="l" type="Number">luminosity</param>
            /// <returns type="String" />
            return "";
        };
        R.hsl2rgb = function (h, s, l) {
            /// <summary>Converts HSL values to RGB object</summary>
            /// <param name="h" type="Number">hue</param>
            /// <param name="s" type="Number">saturation</param>
            /// <param name="l" type="Number">luminosity</param>
            /// <returns type="Object" />
            return rgb();
        };
        R.is = function (o, type) {
            /// <summary>Handfull replacement for typeof operator.</summary>
            /// <param name="o" >Any object or primitive</param>
            /// <param name="type" type="String">name of the type, i.e. "string", "function", "number" etc.</param>
            /// <returns type="Boolean" />
            return  false;
        };
        R.matrix = function (a, b, c, d, e, f) {
            /// <summary>Utility method Returns matrix based on given parameters.</summary>
            /// <param name="a" type="Number"></param>
            /// <param name="b" type="Number"></param>
            /// <param name="c" type="Number"></param>
            /// <param name="d" type="Number"></param>
            /// <param name="e" type="Number"></param>
            /// <param name="f" type="Number"></param>
            /// <returns type="Matrix" />
            return new Matrix(a, b, c, d, e, f);
        };
        R.ninja = function () {
            /// <summary>
            ///  If you want to leave no trace of Raphaël (Well, Raphaël creates only one global variable Raphael, but anyway.)
            ///  You can use ninja method. Beware, that in this case plugins could stop working, because they are depending on global variable existance.
            /// </summary>
            /// <returns type="Raphael" />
            return R;
        };
        R.parsePathString = function (pathString) {
            /// <summary>Utility method Parses given path string into an array of arrays of path segments.</summary>
            /// <param name="pathString" type="String/Array">path string or array of segments (in last case it will be returned straight away)</param>
            /// <returns type="Array" />
            return[];
        };
        R.parseTransformString = function (TString) {
            /// <summary>Utility method Parses given path string into an array of transformations.</summary>
            /// <param name="TString" type="String/Array">transform string or array of transformations (in the last case it will be returned straight away)</param>
            /// <returns type="Array" />
            return[];
        };
        R.path2curve = function (pathString) {
            /// <summary>Utility method Converts path to a new path where all segments are cubic bezier curves</summary>
            /// <param name="pathString" type="String/Array">path string or array of segments</param>
            /// <returns type="Array" />
            return[];
        }
        R.pathToRelative = function (pathString) {
            /// <summary>Utility method Converts path to relative form</summary>
            /// <param name="pathString" type="String/Array">path string or array of segments</param>
            /// <returns type="Array" />
            return[];
        }
        R.rad = function (deg) {
            /// <summary>Transform angle to radians</summary>
            /// <param name="deg" type="Number">angle in degrees</param>
            /// <returns type="Number" />
            return deg % 360 * PI / 180;
        };
        R.registerFont = function (font) {
            /// <summary>
            ///  Adds given font to the registered set of fonts for Raphaël.
            ///  Should be used as an internal call from within Cufón’s font file.
            ///  Returns original parameter, so it could be used with chaining.
            /// </summary>
            /// <param name="font" type="Font">the font to register</param>
            /// <returns type="Font" />
            return font;
        };
        R.rgb = function (r, g, b) {
            /// <summary>Converts RGB values to hex representation of the colour.</summary>
            /// <param name="r" type="Number">red</param>
            /// <param name="g" type="Number">green</param>
            /// <param name="b" type="Number">blue</param>
            /// <returns type="String" />
            return "";
        };
        R.rgb2hsb = function (r, g, b) {
            /// <summary>Converts RGB values to HSB object.</summary>
            /// <param name="r" type="Number">red</param>
            /// <param name="g" type="Number">green</param>
            /// <param name="b" type="Number">blue</param>
            /// <returns type="Object" />
            /// <field name="h" type="Number">hue</field>
            /// <field name="s" type="Number">saturation</field>
            /// <field name="b" type="Number">brightness</field>
            return { h: 0, s: 0, b: 0 };
        };
        R.rgb2hsl = function (r, g, b) {
            /// <summary>Converts RGB values to HSL object.</summary>
            /// <param name="r" type="Number">red</param>
            /// <param name="g" type="Number">green</param>
            /// <param name="b" type="Number">blue</param>
            /// <returns type="Object" />
            /// <field name="h" type="Number">hue</field>
            /// <field name="s" type="Number">saturation</field>
            /// <field name="l" type="Number">luminosity</field>
            return { h: 0, s: 0, l: 0 };
        };
        R.setWindow = function (newwin) {
            /// <summary>Used when you need to draw in &lt;iframe&gt;. Switched window to the iframe one.</summary>
            /// <param name="newwin" type="Window">new window object</param>
        };
        R.snapTo = function (values, value, tolerance) {
            /// <summary>Snaps given value to given grid.</summary>
            /// <param name="values" type="Array/Number">given array of values or step of the grid</param>
            /// <param name="value" type="Number">value to adjust</param>
            /// <param name="tolerance" type="Number">tolerance for snapping. Default is 10.</param>
            /// <returns type="Number" />
            return 0;
        };
    }(Raphael));
    //easing functions
    (function (R) {
        var ef = R.easing_formulas = {
            linear: function (n) {
                return n;
            },
            "<": function (n) {
                return pow(n, 1.7);
            },
            ">": function (n) {
                return pow(n, .48);
            },
            "<>": function (n) {
                var q = .48 - n / 1.04,
                Q = math.sqrt(.1734 + q * q),
                x = Q - q,
                X = pow(abs(x), 1 / 3) * (x < 0 ? -1 : 1),
                y = -Q - q,
                Y = pow(abs(y), 1 / 3) * (y < 0 ? -1 : 1),
                t = X + Y + .5;
                return (1 - t) * 3 * t * t + t * t * t;
            },
            backIn: function (n) {
                var s = 1.70158;
                return n * n * ((s + 1) * n - s);
            },
            backOut: function (n) {
                n = n - 1;
                var s = 1.70158;
                return n * n * ((s + 1) * n + s) + 1;
            },
            elastic: function (n) {
                if (n == !!n) {
                    return n;
                }
                return pow(2, -10 * n) * math.sin((n - .075) * (2 * PI) / .3) + 1;
            },
            bounce: function (n) {
                var s = 7.5625,
                p = 2.75,
                l;
                if (n < (1 / p)) {
                    l = s * n * n;
                } else {
                    if (n < (2 / p)) {
                        n -= (1.5 / p);
                        l = s * n * n + .75;
                    } else {
                        if (n < (2.5 / p)) {
                            n -= (2.25 / p);
                            l = s * n * n + .9375;
                        } else {
                            n -= (2.625 / p);
                            l = s * n * n + .984375;
                        }
                    }
                }
                return l;
            }
        };
        ef.easeIn = ef["ease-in"] = ef["<"];
        ef.easeOut = ef["ease-out"] = ef[">"];
        ef.easeInOut = ef["ease-in-out"] = ef["<>"];
        ef["back-in"] = ef.backIn;
        ef["back-out"] = ef.backOut;
    }(Raphael));
    (function (setproto) {
        setproto.clear = function () {
            /// <summary>Remove's all elements from the set</summary>
        };
        setproto.exclude = function (el) {
            /// <summary>Removes given element from the set and returns true if element was found and removed.</summary>
            /// <param name="el" type="Element">Element to remove</param>
            /// <returns type="Boolean" />
            return true;
        };
        setproto.forEach = function (callback, thisArg) {
            /// <summary>
            ///   Executes given function for each element in the set.
            ///   If function returns false it will stop loop running.
            /// </summary>
            /// <param name="callback" type="Function">Function to run</param>
            /// <param name="thisArg" type="Object" optional="true">Context object for the callback</param>
            /// <returns type="Set" />
            return this;
        };
        setproto.pop = function () {
            /// <summary>Removes last element and returns it</summary>
            /// <returns type="Element" />
            return new Element();
        };
        setproto.push = function () {
            /// <summary>Adds each argument to the current set</summary>
            /// <returns type="Set" />
            return this;
        };
        setproto.splice = function (index, count, insertion) {
            /// <summary>Removes given element from the set and returns the removed elements as a set.</summary>
            /// <param name="index" type="Number">Position of the deletion</param>
            /// <param name="count" type="Number">Number of elements to removev</param>
            /// <param name="insertion" type="Object" optional="true">Element(s) to insert</param>
            /// <returns type="Set" />
            return new Set();
        };
    }(Set.prototype));

})();