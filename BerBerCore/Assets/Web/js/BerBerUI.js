
var BerBer = (function() {

    //== 1. CoreView ==
    function CoreView(elementType, parentView, options) {
        //= init =
        var _this = this;
        this.parentView = parentView;

        var DEFAULT_ANIM_DURATION = 500;

        this.core = createCore(elementType);
        function createCore(elementType) {
            var type;
            switch (elementType.toLowerCase()) {
                /* // change from <input type=button> to <button>
                case "button":
                    type = elementType;
                    elementType = "input";
                    break;
                */
                case "text":
                    type = elementType;
                    elementType = "input";
                    break;

            }

            var core = document.createElement(elementType);
            if (type) {
                core.setAttribute("type", type);
            }

            return core;
        }

        this.transform = {
            x: 0,
            y: 0,
            w: 100,
            h: 100,
            constAR: false,
            ar: 1,
            constSize: false
        };
        function initTransform(options) {
            //x, y, w, h, constAR, (ar), constSize
            var transform = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.transform !== "undefined" && options.transform != null) {
                    transform = options.transform;
                } else {
                    transform = options;
                }
            }

            if (transform) {
                if (!(typeof transform.x === "undefined" || transform.x == null)) {
                    _this.transform.x = transform.x;
                }
                if (!(typeof transform.y === "undefined" || transform.y == null)) {
                    _this.transform.y = transform.y;
                }
                if (!(typeof transform.w === "undefined" || transform.w == null)) {
                    _this.transform.w = transform.w;
                }
                if (!(typeof transform.h === "undefined" || transform.h == null)) {
                    _this.transform.h = transform.h;
                }
                if (!(typeof transform.constAR === "undefined" || transform.constAR == null)) {
                    _this.transform.constAR = transform.constAR;
                }

                if (!(typeof transform.constSize === "undefined" || transform.constSize == null)) {
                    _this.transform.constSize = transform.constSize;
                }
                if (!(typeof transform.constTrans === "undefined" || transform.constTrans == null)) {
                    _this.transform.constSize = transform.constTrans;
                }

                if (_this.transform.constAR) {
                    _this.transform.ar = transform.w / transform.h;
                }

            }

        }

        this.appearance = {
            bgImgUrl: null,
            bgColor: null,
            borderRadius: 0,
            borderColor: null,
            borderWidth: 1,
            padding: 0
        }
        this.setAppearance_CoreView = function (options) {
            var appearance = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.appearance !== "undefined" && options.appearance != null) {
                    appearance = options.appearance;
                } else {
                    appearance = options;
                }
            }

            if (appearance) {
                if (!(typeof appearance.bgImgUrl === "undefined" || appearance.bgImgUrl == null)) {
                    _this.appearance.bgImgUrl = appearance.bgImgUrl;
                }
                if (!(typeof appearance.bgColor === "undefined" || appearance.bgColor == null)) {
                    _this.appearance.bgColor = appearance.bgColor;
                }
                if (!(typeof appearance.borderRadius === "undefined" || appearance.borderRadius == null)) {
                    _this.appearance.borderRadius = appearance.borderRadius;
                }
                if (!(typeof appearance.borderColor === "undefined" || appearance.borderColor == null)) {
                    _this.appearance.borderColor = appearance.borderColor;
                }
                if (!(typeof appearance.borderWidth === "undefined" || appearance.borderWidth == null)) {
                    _this.appearance.borderWidth = appearance.borderWidth;
                }
                if (!(typeof appearance.borderType === "undefined" || appearance.borderType == null)) {
                    _this.appearance.borderType = appearance.borderType;
                }
                if (!(typeof appearance.padding === "undefined" || appearance.padding == null)) {
                    _this.appearance.padding = appearance.padding;
                }

            }

        }

        //= transform =
        this.doTransform = function (updateChild) {
            var parentW_px = _this.parentView.getPxW();
            var parentH_px = _this.parentView.getPxH();

            if (_this.transform.constAR) { // constant aspect ratio (w major)
                //TODO 自動偵測可符合的長寬, 而非目前只看寬度
                //=> 或許能分成兩種模式. 只看寬度的不限制整頁高度(可捲動); 長寬都看的則限制在一個畫面裡.
                //=> 先保持這樣, 若真有需要再加選項即可
                _this.core.style.left = (parentW_px * _this.transform.x / 100) + "px";
                _this.core.style.top = (parentH_px * _this.transform.y / 100) + "px";
                _this.core.style.width = (parentW_px * _this.transform.w / 100) + "px";
                _this.core.style.height = (parentW_px * _this.transform.w / _this.transform.ar / 100) + "px";

            } else { // not constant aspect ratio
                /*
                _this.core.style.left = (parentW_px * _this.transform.x / 100) + "px";
                _this.core.style.top = (parentH_px * _this.transform.y / 100) + "px";
                _this.core.style.width = (parentW_px * _this.transform.w / 100) + "px";
                _this.core.style.height = (parentH_px * _this.transform.h / 100) + "px";
                */
                var newPxW = parentW_px * _this.transform.w / 100;
                var newPxH = parentH_px * _this.transform.h / 100;
                switch (berAnchor) {
                    case 'left_top':
                        _this.core.style.left = berPadding + "px";
                        _this.core.style.top = berPadding + "px";
                        _this.core.style.width = newPxW + "px";
                        _this.core.style.height = newPxH + "px";
                        break;

                    case 'right_top':
                        _this.core.style.left = (parentW_px - newPxW - berPadding) + "px";
                        _this.core.style.top = berPadding + "px";
                        _this.core.style.width = newPxW + "px";
                        _this.core.style.height = newPxH + "px";
                        break;

                    case 'left_bottom':
                        _this.core.style.left = berPadding + "px";
                        _this.core.style.top = (parentH_px - newPxH - berPadding) + "px";
                        _this.core.style.width = newPxW + "px";
                        _this.core.style.height = newPxH + "px";
                        break;

                    case 'right_bottom':
                        _this.core.style.left = (parentW_px - newPxW - berPadding) + "px";
                        _this.core.style.top = (parentH_px - newPxH - berPadding) + "px";
                        _this.core.style.width = newPxW + "px";
                        _this.core.style.height = newPxH + "px";
                        break;

                    default:
                        _this.core.style.left = (parentW_px * _this.transform.x / 100) + "px";
                        _this.core.style.top = (parentH_px * _this.transform.y / 100) + "px";
                        _this.core.style.width = (parentW_px * _this.transform.w / 100) + "px";
                        _this.core.style.height = (parentH_px * _this.transform.h / 100) + "px";

                }

            }

            if (updateChild) {
                _this.redrawChildViews();
            }

        }
        this.setTransform = function (options) {
            var transform = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.transform !== "undefined" && options.transform != null) {
                    transform = options.transform;
                } else {
                    transform = options;
                }
            }

            //
            if (typeof transform.animation !== "undefined" && transform.animation != null) {
                // animate
                var parentW_px = _this.parentView.getPxW();
                var parentH_px = _this.parentView.getPxH();
                var newAnimation = {};
                newAnimation.transform = {};

                if (transform.x || transform.x == 0) {
                    newAnimation.transform.x = parentW_px * transform.x / 100;
                } else {
                    newAnimation.transform.x = _this.getPxX();
                }
                if (transform.y || transform.y == 0) {
                    newAnimation.transform.y = parentH_px * transform.y / 100;
                } else {
                    newAnimation.transform.y = _this.getPxY();
                }
                if (transform.w || transform.w == 0) {
                    newAnimation.transform.w = parentW_px * transform.w / 100;
                } else {
                    newAnimation.transform.w = _this.getPxW();
                }
                if (transform.h || transform.h == 0) {
                    newAnimation.transform.h = parentH_px * transform.h / 100;
                } else {
                    newAnimation.transform.h = _this.getPxH();
                }

                //
                if (typeof transform.animation.duration_ms !== "undefined" && transform.animation.duration_ms != null) {
                    newAnimation.duration_ms = transform.animation.duration_ms;
                } else {
                    newAnimation.duration_ms = DEFAULT_ANIM_DURATION;
                }
                newAnimation.start_time = new Date();

                //
                newAnimation.callback = {};
                newAnimation.callback.transform_frame = transform.animation.callback_frame;
                newAnimation.callback.transform_end = transform.animation.callback_end;

                animationQueue.push(newAnimation);

            } else {
                // normal
                var updateChild = true;
                if (typeof transform.updateChild !== "undefined" && transform.updateChild != null) {
                    updateChild = transform.updateChild;
                }

                initTransform(options);
                _this.doTransform(updateChild);

            }

        }
        this.setTransformPx = function (options) {
            var transform = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.transform !== "undefined" && options.transform != null) {
                    transform = options.transform;
                } else {
                    transform = options;
                }
            }

            var updateChild = true;
            if (typeof transform.updateChild !== "undefined" && transform.updateChild != null) {
                updateChild = transform.updateChild;
            }

            //
            if (typeof transform === "undefined" || transform == null) {
                console.warn("no transform data");

            } else {
                var parentW_px = _this.parentView.getPxW();
                var parentH_px = _this.parentView.getPxH();

                if (typeof transform.x !== "undefined" && transform.x != null) {
                    _this.core.style.left = transform.x + "px";
                    _this.transform.x = transform.x / parentW_px * 100;
                }
                if (typeof transform.y !== "undefined" && transform.y != null) {
                    _this.core.style.top = transform.y + "px";
                    _this.transform.y = transform.y / parentH_px * 100;
                }
                if (typeof transform.w !== "undefined" && transform.w != null) {
                    _this.core.style.width = transform.w + "px";
                    _this.transform.w = transform.w / parentW_px * 100;
                }
                if (typeof transform.h !== "undefined" && transform.h != null) {
                    _this.core.style.height = transform.h + "px";
                    _this.transform.h = transform.h / parentH_px * 100;
                }

                //
                _this.doTransform(updateChild);

            }

        }
        //
        this.getPxX = function (checkAnimation) {
            var checkAnimation = (typeof checkAnimation === "undefined") ? false : checkAnimation;
            var x;
            if (animation && animation.transform && checkAnimation) {
                x = animation.transform.x;
            } else {
                x = parseFloat(_this.core.style.left);
            }
            return x;
        }
        this.getPxY = function (checkAnimation) {
            var checkAnimation = (typeof checkAnimation === "undefined") ? false : checkAnimation;
            var y;
            if (animation && animation.transform && checkAnimation) {
                y = animation.transform.y;
            } else {
                y = parseFloat(_this.core.style.top);
            }
            return y;
        }
        this.getPxW = function (checkAnimation) {
            var checkAnimation = (typeof checkAnimation === "undefined") ? false : checkAnimation;
            var w;
            if (animation && animation.transform && checkAnimation) {
                w = animation.transform.w;
            } else {
                w = parseFloat(_this.core.style.width);
            }
            return w;
        }
        this.getPxH = function (checkAnimation) {
            var checkAnimation = (typeof checkAnimation === "undefined") ? false : checkAnimation;
            var h;
            if (animation && animation.transform && checkAnimation) {
                h = animation.transform.h;
            } else {
                h = parseFloat(_this.core.style.height);
            }
            return h;
        }
        this.getTransformPx = function (checkAnimation) {
            return {
                x: _this.getPxX(checkAnimation),
                y: _this.getPxY(checkAnimation),
                w: _this.getPxW(checkAnimation),
                h: _this.getPxH(checkAnimation)
            }
        }

        //= appearance =
        this.core.style.backgroundRepeat = "no-repeat";
        //
        this.setClassName = function (className) {
            _this.core.className = className;
        }

        this.boxEffectType = "none";
        this.boxEffectRange = 0;
        this.boxEffectColor = "";
        this.setBoxEffect = function (visible, type, color) {//TODO animation
            if (visible) {
                _this.boxEffectRange = ((_this.getPxW()+_this.getPxH()) / 2 / 30);
                _this.boxEffectType = type;
                _this.boxEffectColor = color;

                if (this.boxEffectType == "glow") { //UNDONE 正在決定此處參數, 以及新的閃爍邊框要實作到哪層
                    //_this.core.style.boxShadow = "0px 0px " + _this.boxEffectRange + "px " + _this.boxEffectRange + "px " + _this.boxEffectColor;
                    _this.core.style.boxShadow = "0px 0px " + _this.boxEffectRange + "px " + _this.boxEffectRange / 3 + "px " + _this.boxEffectColor;

                } else if (this.boxEffectType == "shadow") {
                    _this.core.style.boxShadow = "0px 0px " + _this.boxEffectRange + "px " + _this.boxEffectColor;

                } else {
                    console.warn("[setBoxEffect] no matching type");
                }

            } else {
                _this.core.style.boxShadow = "";
                _this.boxEffectType = "none";
            }
        }

        this.setVisibility = function (visible, animOptions) {
            if (typeof animOptions !== "undefined" && animOptions != null) {
                var newAnimation = {};
                //
                if (typeof animOptions.duration_ms !== "undefined" && animOptions.duration_ms != null) {
                    newAnimation.duration_ms = animOptions.duration_ms;
                } else {
                    newAnimation.duration_ms = DEFAULT_ANIM_DURATION;
                }
                //
                if (typeof animOptions.callback_frame !== "undefined") {
                    newAnimation.callback = {};
                    newAnimation.callback.visibility_frame = animOptions.callback_frame;
                }
                if (typeof animOptions.callback_end !== "undefined") {
                    newAnimation.callback = {};
                    newAnimation.callback.visibility_end = animOptions.callback_end;
                }
                //
                if (visible) {
                    newAnimation.opacity = 1;
                } else {
                    newAnimation.opacity = 0;
                }
                newAnimation.start_time = new Date();
                animationQueue.push(newAnimation);

            } else {
                if (visible) {
                    _this.core.style.opacity = 1;
                    _this.core.style.display = "block";

                } else {
                    _this.core.style.opacity = 0;
                    _this.core.style.display = "none";

                }

            }

            _this.animToVisible = visible;//

        }
        this.getVisibility = function () {
            if (_this.core.style.display == "none") {
                return false;
            } else {
                return true;
            }
        }

        this.rotation = {
            x: 0,
            y: 0,
            z: 0
        };
        this.setRotation = function (xAngle, yAngle, zAngle) {//TODO animation
            var x = (xAngle === undefined || xAngle == null) ? 0 : xAngle;
            var y = (yAngle === undefined || yAngle == null) ? 0 : yAngle;
            var z = (zAngle === undefined || zAngle == null) ? 0 : zAngle;

            _this.rotation = {
                x: x,
                y: y,
                z: z
            };

            _this.core.style.transform = "rotateX(" + x + "deg) rotateY(" + y + "deg) rotateZ(" + z + "deg)";
        }
        //this.setRotation3d = function (xAxis, yAxis, zAxis, angle) {//理論上和上面作法等效, 指保留上面那個
        //    //https://www.w3.org/Talks/2012/0416-CSS-WWW2012/Demos/transforms/demo-rotate3d.html
        //    var x = (xAxis === undefined || xAxis == null) ? 0 : xAxis;
        //    var y = (yAxis === undefined || yAxis == null) ? 0 : yAxis;
        //    var z = (zAxis === undefined || zAxis == null) ? 0 : zAxis;
        //    var _angle = (angle === undefined || angle == null) ? 0 : angle;
        //    _this.core.style.transform = "rotate3d(" + x + "," + y + "," + z + "," + _angle + "deg)";
        //}

        //= animation =
        var animation = null;
        /*
        var animation = {
          opacity: null,
          transform: null,
          rotation: null,
          duration_ms: 500,
          callback: {
            visibility_frame: null,
            visibility_end: null,
            transform_frame: null,
            transform_end: null,
            rotation_frame: null,
            rotation_end: null,
          }
        }
        */
        var animationQueue = [];
        var animLoop = false;
        this.setAnimLoop = function (loop) {
            animLoop = loop;
        }
        this.animToVisible = true;// 檢查是否正在執行動畫 (有些計算會需要知道) //TODO 確認是否需要
        //
        this.setAnimation = function (options) {
            if (options) {
                var newAnimation = {};

                // Opacity
                if (typeof options.opacity !== "undefined" && options.opacity != null) {
                    newAnimation.opacity = options.opacity;
                }

                // Transform
                if (typeof options.transform !== "undefined" && options.transform != null) {
                    var parentW_px = _this.parentView.getPxW();
                    var parentH_px = _this.parentView.getPxH();
                    newAnimation.transform = {};
                    var transform = options.transform;

                    if (transform.x || transform.x == 0) {
                        newAnimation.transform.x = parentW_px * transform.x / 100;
                    } else {
                        newAnimation.transform.x = _this.getPxX();
                    }
                    if (transform.y || transform.y == 0) {
                        newAnimation.transform.y = parentH_px * transform.y / 100;
                    } else {
                        newAnimation.transform.y = _this.getPxY();
                    }
                    if (transform.w || transform.w == 0) {
                        newAnimation.transform.w = parentW_px * transform.w / 100;
                    } else {
                        newAnimation.transform.w = _this.getPxW();
                    }
                    if (transform.h || transform.h == 0) {
                        newAnimation.transform.h = parentH_px * transform.h / 100;
                    } else {
                        newAnimation.transform.h = _this.getPxH();
                    }

                }

                // Rotation (NOTE: this is style.transform)
                if (typeof options.rotation !== "undefined" && options.rotation != null) {
                    newAnimation.rotation = {};
                    var rotation = options.rotation;
                    newAnimation.rotation.initRotation = _this.rotation;//

                    if (rotation.x || rotation.x == 0) {
                        newAnimation.rotation.x = rotation.x;
                    }
                    if (rotation.y || rotation.y == 0) {
                        newAnimation.rotation.y = rotation.y;
                    }
                    if (rotation.z || rotation.z == 0) {
                        newAnimation.rotation.z = rotation.z;
                    }

                }

                // Duration
                if (typeof options.duration_ms !== "undefined" && options.duration_ms != null) {
                    newAnimation.duration_ms = options.duration_ms;
                } else {
                    newAnimation.duration_ms = DEFAULT_ANIM_DURATION;
                }
                newAnimation.start_time = new Date();

                // Callbacks
                if (typeof options.callback !== "undefined" && options.callback != null) {
                    newAnimation.callback = {};
                    // visibility_frame
                    if (typeof options.callback.visibility_frame !== "undefined") {
                        newAnimation.callback.visibility_frame = options.callback.visibility_frame;
                    }
                    // visibility_end
                    if (typeof options.callback.visibility_end !== "undefined") {
                        newAnimation.callback.visibility_end = options.callback.visibility_end;
                    }

                    // transform_frame
                    if (typeof options.callback.transform_frame !== "undefined") {
                        newAnimation.callback.transform_frame = options.callback.transform_frame;
                    }
                    // transform_end
                    if (typeof options.callback.transform_end !== "undefined") {
                        newAnimation.callback.transform_end = options.callback.transform_end;
                    }

                    // rotation_frame
                    if (typeof options.callback.rotation_frame !== "undefined") {
                        newAnimation.callback.rotation_frame = options.callback.rotation_frame;
                    }
                    // rotation_end
                    if (typeof options.callback.rotation_end !== "undefined") {
                        newAnimation.callback.rotation_end = options.callback.rotation_end;
                    }

                }

                //
                animationQueue.push(newAnimation);

            }


        }
        this.setAnimations = function (optionList) {
            if (optionList && optionList.length) {
                for (var i = 0; i < optionList.length; i++){
                    _this.setAnimation(optionList[i]);
                }
            }
        }
        //
        animate();
        function animate() {
            var passingTime = null;
            var animFactor = null;

            //
            if (animation == null && animationQueue.length > 0) {
                //
                animation = animationQueue.shift();
                animation.start_time = new Date();

                // transform
                if (animation != null && animation.transform) {
                    if (animation.transform.x != null) {
                        animation.initX = _this.getPxX();
                        animation.finalOffsetX = animation.transform.x - animation.initX;
                    }
                    if (animation.transform.y != null) {
                        animation.initY = _this.getPxY();
                        animation.finalOffsetY = animation.transform.y - animation.initY;
                    }
                    if (animation.transform.w != null) {
                        animation.initW = _this.getPxW();
                        animation.finalOffsetW = animation.transform.w - animation.initW;
                    }
                    if (animation.transform.h != null) {
                        animation.initH = _this.getPxH();
                        animation.finalOffsetH = animation.transform.h - animation.initH;
                    }
                }

                // opacity
                if (animation != null && animation.opacity != null) {
                    animation.initOpacity = parseFloat(_this.core.style.opacity);
                    animation.finalOffsetOpacity = (animation.opacity - animation.initOpacity)
                }

                // time
                passingTime = ((new Date()).getTime() - animation.start_time);
                animFactor = (passingTime / animation.duration_ms);

                //
                if (animation.opacity != null && animation.opacity == 1) {
                    _this.core.style.display = "block";
                }

            }

            // Opacity
            if (animation != null && animation.opacity != null) {
                if (passingTime == null) {
                    passingTime = ((new Date()).getTime() - animation.start_time);
                    animFactor = (passingTime / animation.duration_ms);
                }

                var offset = animation.finalOffsetOpacity * animFactor;
                _this.core.style.opacity = animation.initOpacity + offset;

                //
                if (animation.callback && animation.callback.visibility_frame) {
                    animation.callback.visibility_frame();
                }

            }

            // Transform
            if (animation != null && animation.transform) {
                if (passingTime == null) {
                    passingTime = ((new Date()).getTime() - animation.start_time);
                    animFactor = (passingTime / animation.duration_ms);
                }

                var offsetX, offsetY, offsetW, offsetH;

                if (animation.transform.x != null) {
                    offsetX = animation.finalOffsetX * animFactor;
                    _this.core.style.left = (animation.initX + offsetX) + "px";
                }
                if (animation.transform.y != null) {
                    offsetY = animation.finalOffsetY * animFactor;
                    _this.core.style.top = (animation.initY + offsetY) + "px";
                }
                if (animation.transform.w != null) {
                    offsetW = animation.finalOffsetW * animFactor;
                    _this.core.style.width = (animation.initW + offsetW) + "px";
                }
                if (animation.transform.h != null) {
                    offsetH = animation.finalOffsetH * animFactor;
                    _this.core.style.height = (animation.initH + offsetH) + "px";
                }

                //
                _this.redrawChildViews();

                //
                if (animation.callback && animation.callback.transform_frame) {
                    animation.callback.transform_frame();
                }

            }

            // Rotation
            if (animation != null && animation.rotation) {
                if (passingTime == null) {
                    passingTime = ((new Date()).getTime() - animation.start_time);
                    animFactor = (passingTime / animation.duration_ms);
                }

                var transfromStr = "";

                if (animation.rotation.x != null) {
                    var xAngle = animation.rotation.initRotation.x + (animation.rotation.x - animation.rotation.initRotation.x) * animFactor;
                    transfromStr += "rotateX(" + xAngle + "deg) ";
                }
                if (animation.rotation.y != null) {
                    var yAngle = animation.rotation.initRotation.y + (animation.rotation.y - animation.rotation.initRotation.y) * animFactor;
                    transfromStr += "rotateY(" + yAngle + "deg) ";
                }
                if (animation.rotation.z != null) {
                    var zAngle = animation.rotation.initRotation.z + (animation.rotation.z - animation.rotation.initRotation.z) * animFactor;
                    transfromStr += "rotateZ(" + zAngle + "deg) ";
                }

                _this.core.style.transform = transfromStr;// TODO NOTE 所有 style.transform 都要用 transfromStr 串好再一起設定

                //
                if (animation.callback && animation.callback.rotation_frame) {
                    animation.callback.rotation_frame();
                }

            }

            //
            if (animation != null && passingTime >= animation.duration_ms) {

                _this.endAnimation();

                if (animLoop) {
                    animationQueue.push(animation);
                }

                if (animationQueue.length > 0) {
                    //
                    animation = animationQueue.shift();
                    animation.start_time = new Date();

                    // transform
                    if (animation != null && animation.transform) {
                        if (animation.transform.x != null) {
                            animation.initX = _this.getPxX();
                            animation.finalOffsetX = animation.transform.x - animation.initX;
                        }
                        if (animation.transform.y != null) {
                            animation.initY = _this.getPxY();
                            animation.finalOffsetY = animation.transform.y - animation.initY;
                        }
                        if (animation.transform.w != null) {
                            animation.initW = _this.getPxW();
                            animation.finalOffsetW = animation.transform.w - animation.initW;
                        }
                        if (animation.transform.h != null) {
                            animation.initH = _this.getPxH();
                            animation.finalOffsetH = animation.transform.h - animation.initH;
                        }
                    }

                    // opacity
                    if (animation != null && animation.opacity != null) {
                        animation.initOpacity = parseFloat(_this.core.style.opacity);
                        animation.finalOffsetOpacity = (animation.opacity - animation.initOpacity)
                    }

                    //
                    if (animation.opacity != null && animation.opacity == 1) {
                        _this.core.style.display = "block";
                    }

                } else {
                    animation = null;
                }

            }

            //
            requestAnimationFrame(animate);

        }
        this.endAnimation = function () {
            // Opacity
            if (animation.opacity != null) {
                if (animation.opacity == 1) {
                    _this.core.style.opacity = 1;
                    _this.core.style.display = "block";
                } else {
                    _this.core.style.opacity = 0;
                    _this.core.style.display = "none";
                }
            }

            // Transform
            if (animation.transform) {
                var oldAnimTransform = animation.transform;
                _this.setTransformPx({
                    transform: oldAnimTransform,
                    updateChild: true
                });
            }

            // Rotation
            if (animation.rotation) {
                _this.setRotation(animation.rotation.x, animation.rotation.y, animation.rotation.z);
            }

            // BoxEffect //TODO 新動畫功能待納入這個 現在只是調整靜態效果已符合改變後的元件
            this.boxEffectRange = ((this.getPxW()+this.getPxH()) / 2 / 30);
            if (this.boxEffectType == "glow") {
                this.core.style.boxShadow = "0px 0px " + this.boxEffectRange + "px " + this.boxEffectRange + "px " + this.boxEffectColor;
            } else if (this.boxEffectType == "shadow") {
                this.core.style.boxShadow = "0px 0px " + this.boxEffectRange + "px " + this.boxEffectColor;
            }

            // Callback End
            if (animation.callback && animation.callback.visibility_end) {
                animation.callback.visibility_end();
            }
            if (animation.callback && animation.callback.transform_end) {
                animation.callback.transform_end();
            }
            if (animation.callback && animation.callback.rotation_end) {
                animation.callback.rotation_end();
            }

        }


        //= hierarchy =
        this.core.style.position = "absolute";
        this.parentView.core.appendChild(this.core);
        this.childViews = [];
        this.parentView.childViews.push(this);
        //
        this.removeFromParent = function () {
            if (_this.core.parentNode) {
                _this.core.parentNode.removeChild(_this.core);
            }
        }
        this.addView = function (view) {
            _this.core.appendChild(view.core);
            _this.childViews.push(view);
            _this.redraw();
        }
        this.removeChildView = function (view) {
            _this.core.removeChild(view.core);

            var idx = _this.childViews.indexOf(view);
            if (idx > -1) {
                _this.childViews.splice(idx, 1);
            }

            _this.redrawChildViews();
        }
        this.redrawChildViews = function () {
            for (var i = 0; i < _this.childViews.length; i++) {
                if (_this.childViews[i].redraw) {
                    _this.childViews[i].redraw();
                }
            }
        }


        // = others =
        this.setPositionFixed = function (fixed) {
            if (fixed) {
                _this.core.style.position = "fixed";
            } else {
                _this.core.style.position = "absolute";
            }
        }
        this.setZIndex = function (zIndex) {
            _this.core.style.zIndex = zIndex;
        }
        this.setDisabled = function (disabled) {
            _this.core.disabled = disabled;
        }

        //NOTE 是否取代掉原本的 padding
        var berPadding = -1;
        var berAnchor = ''; // left_top / right_top / left_bottom / right_bottom
        this.setBerPadding = function(padding, anchor) {
            berPadding = padding;
            berAnchor = anchor;
            //_this.doBerPadding();
            _this.doTransform(true);
        }
        this.doBerPadding = function() {
            if(berPadding != -1) {
                var refTrans = _this.parentView.getTransformPx();
                if(berAnchor == 'left') {
                    _this.setTransformPx({
                        x: berPadding,
                        y: berPadding,
                        w: refTrans.w - berPadding * 2,
                        h: refTrans.h - berPadding * 2
                    });

                } else {
                    _this.setTransformPx({
                        x: (100 - refTrans.w) - berPadding,
                        y: berPadding,
                        w: refTrans.w - berPadding * 2,
                        h: refTrans.h - berPadding * 2
                    });


                }

            }
        }


        //= event =
        //舊版 會清除前一個事件
        this.on = {};
        //
        var oldListener = null;
        this.setClickListener = function (listener) {
            var newListener = function () {
                if (event) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                if (listener) {
                    listener();
                } else {
                    console.warn("no listener");
                }
            }

            if (oldListener != null) {
                _this.core.removeEventListener("click", oldListener);
            }
            _this.core.addEventListener("click", newListener, false);
            oldListener = newListener;

            _this.core.style.cursor = "pointer";
        }
        //新版 通用
        this.addListener = function (type, listener, consumeEvent, changeCursor) {
            var consumeEvent = (consumeEvent === undefined || consumeEvent == null) ? true : consumeEvent;
            var changeCursor = (changeCursor === undefined || changeCursor == null) ? true : changeCursor;

            _this.core.addEventListener(type, function (event) {
                if (event) {
                    if (consumeEvent) {
                        event.preventDefault();
                        event.stopPropagation();
                    }

                    if (listener) {
                        listener(event);
                    } else {
                        console.warn("no listener");
                    }

                }

            }, false);

            if (changeCursor) {
                _this.core.style.cursor = "pointer";
            }

        }


        //====
        this.core.style.opacity = 1;// necessary defult value (for animation)
        //
        this.setAppearance_CoreView(options);//
        //bgImgUrl
        this.core.style.backgroundSize = this.getPxW() + "px " + this.getPxH() + "px";
        this.core.style.backgroundImage = this.appearance.bgImgUrl;//
        //bgColor
        this.core.style.backgroundColor = this.appearance.bgColor;
        //borderRadius
        this.core.style.borderRadius = this.appearance.borderRadius + "px";
        //border
        if (this.appearance.borderColor) {
            this.core.style.border = this.appearance.borderWidth + "px solid " + this.appearance.borderColor;
        } else {
            this.core.style.border = "hidden";
        }
        //padding
        this.core.style.padding = this.appearance.padding + "px";
        //
        initTransform(options);//
        this.doTransform(false);//


    }
    CoreView.prototype.redraw = function () {
        //
        //bgImgUrl
        this.core.style.backgroundSize = this.getPxW() + "px " + this.getPxH() + "px";
        this.core.style.backgroundImage = this.appearance.bgImgUrl;//
        //bgColor
        this.core.style.backgroundColor = this.appearance.bgColor;
        //borderRadius
        this.core.style.borderRadius = this.appearance.borderRadius + "px";
        //border
        if (this.appearance.borderColor) {
            this.core.style.border = this.appearance.borderWidth + "px solid " + this.appearance.borderColor;
        } else {
            this.core.style.border = "hidden";
        }
        //padding
        this.core.style.padding = this.appearance.padding + "px";

        //
        if (this.transform.constSize) {
            return;
        }

        this.doTransform(true);
        if (this.core.style.backgroundImage) {
            this.core.style.backgroundSize = this.getPxW() + "px " + this.getPxH() + "px";

        }

        //boxEffect
        this.boxEffectRange = ((this.getPxW()+this.getPxH()) / 2 / 30);
        if (this.boxEffectType == "glow") {
            this.core.style.boxShadow = "0px 0px " + this.boxEffectRange + "px " + this.boxEffectRange + "px " + this.boxEffectColor;
        } else if (this.boxEffectType == "shadow") {
            this.core.style.boxShadow = "0px 0px " + this.boxEffectRange + "px " + this.boxEffectColor;
        }

        //
        if(this.on.redraw) this.on.redraw();

        //this.doBerPadding();

    }
    CoreView.prototype.initAppearance = function (options) {
        this.setAppearance_CoreView(options);
    }
    CoreView.prototype.setAppearance = function (options, redraw) {
        var doRedraw = (typeof redraw === "undefined") ? true : redraw;

        this.setAppearance_CoreView(options);
        if (doRedraw) {
            this.redraw();
        }
    }


    //== 2. DivView ==
    function DivView(parentView, options) {
        CoreView.call(this, "div", parentView, options);
        var _this = this;

        this.core.style.overflowX = "hidden";
        this.core.style.overflowY = "hidden";
        this.setOverflowX = function (scroll) {
            if (scroll) {
                _this.core.style.overflowX = "scroll";
            } else {
                _this.core.style.overflowX = "hidden";
            }
        }
        this.setOverflowY = function (scroll) {
            if (scroll) {
                _this.core.style.overflowY = "scroll";
            } else {
                _this.core.style.overflowY = "hidden";
            }
        }
        this.setOverflowScroll = function(scroll) {
            if(scroll) {
                this.core.style.overflowX = "scroll";
                this.core.style.overflowY = "scroll";
            } else {
                this.core.style.overflowX = "hidden";
                this.core.style.overflowY = "hidden";
            }
        }

        this.setOverflowVisible = function(visible) {
            if(visible) {
                this.core.style.overflowX = "visible";
                this.core.style.overflowY = "visible";
            } else {
                this.core.style.overflowX = "hidden";
                this.core.style.overflowY = "hidden";
            }
        }

        this.scrollToTop = function () {
            _this.core.scrollTop = 0;
        }
        this.scrollToBottom = function () {
            _this.core.scrollTop = _this.core.scrollHeight;
        }
        this.setScrollTop = function(top) {
            _this.core.scrollTop = top;
        }
        this.getScrollTop = function () {
            return _this.core.scrollTop;
        }

        //
        var tapped = false;
        var transformPx_origin;
        this.toggleOrientation = function () {
            if (!transformPx_origin) transformPx_origin = _this.getTransformPx();
            var temp = _this.getTransformPx();

            if (tapped) {
                _this.setTransformPx(transformPx_origin);
                _this.setRotation(0, 0, 0);

            } else {
                var offset = (temp.h - temp.w) / 2;
                _this.setTransformPx({
                    x: -offset,
                    y: offset + transformPx_origin.y,
                    w: temp.h,
                    h: temp.w
                });

                _this.setRotation(0, 0, 90);

            }

            tapped = !tapped;
        }

    }
    DivView.prototype = CoreView.prototype;


    //====

    //== BodyView ==
    function BodyView() {
        var _this = this;

        this.core = document.body;

        this.getPxW = function () {
            return Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
        }
        this.getPxH = function () {
            return Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
        }

        this.childViews = [];
        this.addView = function (view) {
            _this.childViews.push(view);
        }

        this.autoResizeChild = true;

        window.addEventListener("resize", function () {
            if (_this.autoResizeChild) {
                for (var i = 0; i < _this.childViews.length; i++) {
                    _this.childViews[i].redraw();
                }
            }
        }, false);



        window.addEventListener('orientationchange', function() {
            // switch(window.orientation)  {
            //     case -90:
            //     case 90:
            //         alert('landscape');
            //         break;
            //     default:
            //         alert('portrait');
            //     break;
            // }

            console.log('orientationchange');
            //if (_this.autoResizeChild) {
                for (var i = 0; i < _this.childViews.length; i++) {
                    _this.childViews[i].redraw();
                }
            //}

        });

    }
    //== 3. RootView ==
    function RootView() {
        var bodyView = new BodyView();
        DivView.call(this, bodyView, {
            transform: {
                x: 0,
                y: 0,
                w: 100,
                h: 100
            }
        });

        this.setAutoResizeChild = function (auto) {
            bodyView.autoResizeChild = auto;
        }

    }
    RootView.prototype = DivView.prototype;



    //====

    //== 4. TextBoxView ==
    function TextBoxView(parentView, options) {
        CoreView.call(this, "text", parentView, options);
        var _this = this;

        this.appearance.text = "";
        this.appearance.textColor = "#000";
        this.appearance.txtAlign = "left"; // left | right | center | justify
        this.appearance.placeholder = "";
        this.appearance.fontSizeScale = 1;
        this.setAppearance_TextBoxView = function (options) {
            var appearance = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.appearance !== "undefined" && options.appearance != null) {
                    appearance = options.appearance;
                } else {
                    appearance = options;
                }
            }

            if (appearance) {
                if (!(typeof appearance.text === "undefined" || appearance.text == null)) {
                    _this.appearance.text = appearance.text;
                }
                if (!(typeof appearance.textColor === "undefined" || appearance.textColor == null)) {
                    _this.appearance.textColor = appearance.textColor;
                }
                if (!(typeof appearance.txtAlign === "undefined" || appearance.txtAlign == null)) {
                    _this.appearance.txtAlign = appearance.txtAlign;
                }
                if (!(typeof appearance.placeholder === "undefined" || appearance.placeholder == null)) {
                    _this.appearance.placeholder = appearance.placeholder;
                }
                if (!(typeof appearance.fontSizeScale === "undefined" || appearance.fontSizeScale == null)) {
                    _this.appearance.fontSizeScale = appearance.fontSizeScale;
                }
            }
        }

        //
        this.getText = function () {
            return _this.core.value;
        }

        this.setFocus = function () {
            _this.core.focus();
        }

        this.asPassword = function () {
            _this.core.type = 'password';
        }

        //this.setIsNumber = function () {
        //    core.setAttribute('pattern', '[0-9]*');
        //}


        //====
        //
        this.setAppearance_TextBoxView(options);
        //
        this.addListener("change", function () { _this.appearance.text = _this.core.value }, true, false);
        //
        this.core.value = this.appearance.text;
        this.core.style.color = this.appearance.textColor;
        this.core.style.textAlign = this.appearance.txtAlign;
        this.core.placeholder = this.appearance.placeholder;
        //
        this.coreH = this.getPxH();
        this.fontSize = this.coreH / 2;
        this.fontSizeScale = this.appearance.fontSizeScale;
        this.adjustedFontSize = this.fontSize * this.fontSizeScale;
        this.core.style.fontSize = this.adjustedFontSize + "px";

    }
    TextBoxView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        this.core.value = this.appearance.text;
        this.core.style.color = this.appearance.textColor;
        this.core.style.textAlign = this.appearance.txtAlign;
        this.core.placeholder = this.appearance.placeholder;

        //
        if (this.transform.constSize) {
            return;
        }

        //
        this.coreH = this.getPxH();
        this.fontSize = this.coreH / 2;
        this.fontSizeScale = this.appearance.fontSizeScale;
        this.adjustedFontSize = this.fontSize * this.fontSizeScale;
        this.core.style.fontSize = this.adjustedFontSize + "px";

    }
    TextBoxView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        this.setAppearance_TextBoxView(options);
    }
    TextBoxView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        this.setAppearance_TextBoxView(options);
        this.redraw();//this also makes parent-redraw
    }


    //== 5. TextAreaView ==
    function TextAreaView(parentView, options) {
        CoreView.call(this, "textarea", parentView, options);
        var _this = this;

        this.appearance.text = '';
        this.appearance.placeholder = '';
        this.appearance.readonly = false;
        this.setAppearance_TextAreaView = function (options) {
            var appearance = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.appearance !== "undefined" && options.appearance != null) {
                    appearance = options.appearance;
                } else {
                    appearance = options;
                }
            }

            if (appearance) {
                if (!(typeof appearance.text === "undefined" || appearance.text == null)) {
                   _this.appearance.text = appearance.text;
                }
                if (!(typeof appearance.placeholder === "undefined" || appearance.placeholder == null)) {
                    _this.appearance.placeholder = appearance.placeholder;
                }
                if (!(typeof appearance.fontSize === "undefined" || appearance.fontSize == null)) {
                    _this.appearance.fontSize = appearance.fontSize;
                }
                if (!(typeof appearance.readonly === "undefined" || appearance.readonly == null)) {
                    _this.appearance.readonly = appearance.readonly;
                }
            }

        }

        this.addListener('keyup', function() {
            _this.appearance.text = _this.core.value;
        });

        //
        this.setReadOnly = function (readonly) {
            if (readonly) {
                _this.core.setAttribute("readonly", "readonly");
            } else {
                _this.core.removeAttribute("readonly");
            }
        }

        this.appendValue = function (msg) {
            _this.core.value += msg;
        }

        this.scrollToBottom = function () {
            _this.core.scrollTop = _this.core.scrollHeight;
        }

        this.getText = function () {
            return _this.core.value;
        }

        this.setFocus = function () {
            _this.core.focus();
        }

        //====
        this.setAppearance_TextAreaView(options);
        //
        if (this.appearance.readonly) {
            this.core.setAttribute("readonly", "readonly");
        } else {
            this.core.removeAttribute("readonly");
        }

    }
    TextAreaView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        this.core.value = this.appearance.text;
        this.core.placeholder = this.appearance.placeholder;

        //
        if (this.transform.constSize) {
            return;
        }

        //
        if (this.appearance.readonly) {
            this.core.setAttribute("readonly", "readonly");
        } else {
            this.core.removeAttribute("readonly");
        }

        //
        this.core.style.fontSize = this.appearance.fontSize + "px";

    }
    TextAreaView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        this.setAppearance_TextAreaView(options);
    }
    TextAreaView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        this.setAppearance_TextAreaView(options);
        this.redraw();//this also makes parent-redraw
    }


    //== 6. VideoView ==
    function VideoView(parentView, options) {
        CoreView.call(this, "video", parentView, options);
        var _this = this;

        this.vid = -1;
        var src = "";
        var autoplay = true;
        var loop = false;
        var mute = false;
        var controls = false;

        initSettings(options);
        function initSettings(options) {
            if (options) {
                if (!(typeof options.vid === "undefined" || options.vid == null)) {
                    _this.vid = options.vid;
                }
                if (!(typeof options.src === "undefined" || options.src == null)) {
                    src = options.src;
                }
                if (!(typeof options.autoplay === "undefined" || options.autoplay == null)) {
                    autoplay = options.autoplay;
                }
                if (!(typeof options.loop === "undefined" || options.loop == null)) {
                    loop = options.loop;
                }
                if (!(typeof options.mute === "undefined" || options.mute == null)) {
                    mute = options.mute;
                }
                if (!(typeof options.controls === "undefined" || options.controls == null)) {
                    controls = options.controls;
                }
            }
        }

        //
        this.setSrc = function (src) {
            _this.core.src = src;
        }
        this.setAutoplay = function (autoplay) {
            _this.core.autoplay = autoplay;
        }
        this.setLoop = function (loop) {
            _this.core.loop = loop;
        }
        this.setMuted = function (mute) {
            _this.core.muted = mute;
        }
        this.setControls = function (controls) {
            _this.core.controls = controls;
        }

        this.play = function () {
            _this.core.play();
        }
        this.pause = function () {
            _this.core.pause();
        }
        this.stop = function () {
            _this.core.pause();
            _this.core.currentTime = 0;
        }
        this.rewind = function () {
            _this.core.currentTime = 0;
        }

        //
        this.core.style.background = "#000000";
        this.core.src = src;
        this.core.autoplay = autoplay;
        this.core.loop = loop;
        this.core.muted = mute;
        this.core.controls = controls;

    }
    VideoView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        if (this.transform.constSize) {
            return;
        }

    }
    VideoView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        //this.setAppearance_VideoView(options);
    }
    VideoView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        //this.setAppearance_VideoView(options);
        this.redraw();//this also makes parent-redraw
    }


    //== 7. CanvasView ==
    function CanvasView(parentView, options) {
        CoreView.call(this, "canvas", parentView, options);
        var _this = this;

        this.context2D = null;
        this.appearance.src = null;
        this.appearance.image = new Image();
        this.appearance.image.onload = function () {
            updateUtils(_this.appearance.fillMode);
        };
        this.appearance.fillMode = "origin";

        this.setAppearance_CanvasView = function (options) {
            var appearance = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.appearance !== "undefined" && options.appearance != null) {
                    appearance = options.appearance;
                } else {
                    appearance = options;
                }
            }

            if (appearance) {
                if (!(typeof appearance.src === "undefined" || appearance.src == null)) {
                    _this.appearance.src = appearance.src;
                }
                if (!(typeof appearance.image === "undefined" || appearance.image == null)) {
                    _this.appearance.image = appearance.image;
                }
                if (!(typeof appearance.fillMode === "undefined" || appearance.fillMode == null)) {
                    _this.appearance.fillMode = appearance.fillMode;
                }

            }

        }

        // UNDONE: 動態使用可能會造成困惑, 因為有兩個模式會改變高度或寬度
        this.setFillMode = function (mode) {
            _this.appearance.fillMode = mode;
            updateUtils(mode);
        }

        this.loadImage = function (src) {
            _this.appearance.image.src = src;
        }
        this.setImage = function (image) {
            _this.appearance.image = image;
            updateUtils(_this.appearance.fillMode);
        }
        function updateUtils(mode) {
            var imageObj = _this.appearance.image;

            switch (mode) {
                case "fit": //填滿元件原尺寸 (圖片可能變形)
                    _this.core.width = imageObj.width;
                    _this.core.height = imageObj.height;
                    _this.context2D = _this.core.getContext("2d");
                    _this.context2D.drawImage(imageObj, 0, 0);
                    break;

                case "const_w": //符合元件寬度, 調整高度以符合比例
                    var viewSize = { w: _this.getPxW(), h: _this.getPxH() };
                    var picSize = { w: imageObj.width, h: imageObj.height };
                    var newSize = {};
                    newSize.w = viewSize.w;
                    newSize.h = viewSize.w / (picSize.w / picSize.h);
                    _this.setTransformPx(newSize);

                    //NOTE: this will auto adjust y
                    _this.setTransform({ y: (100 - _this.transform.h) / 2 });

                    _this.core.width = imageObj.width;
                    _this.core.height = imageObj.height;
                    _this.context2D = _this.core.getContext("2d");
                    _this.context2D.drawImage(imageObj, 0, 0);
                    break;

                case "const_h": //符合元件高度, 調整寬度以符合比例
                    var viewSize = { w: _this.getPxW(), h: _this.getPxH() };
                    var picSize = { w: imageObj.width, h: imageObj.height };
                    var newSize = {};
                    newSize.w = viewSize.h * (picSize.w / picSize.h);
                    newSize.h = viewSize.h;
                    _this.setTransformPx(newSize);

                    //NOTE: this will auto adjust x
                    _this.setTransform({ x: (100 - _this.transform.w) / 2 });

                    _this.core.width = imageObj.width;
                    _this.core.height = imageObj.height;
                    _this.context2D = _this.core.getContext("2d");
                    _this.context2D.drawImage(imageObj, 0, 0);
                    break;

                case "center": //保持圖片比例, 讓圖片置中 (先調到長或寬符合元件)
                    var draw_x = (_this.getPxW() - imageObj.width) / 2;
                    var draw_y = (_this.getPxH() - imageObj.height) / 2;
                    _this.core.width = _this.getPxW();
                    _this.core.height = _this.getPxH();
                    _this.context2D = _this.core.getContext("2d");
                    _this.context2D.drawImage(imageObj, draw_x, draw_y);
                    break;

                default: // origin
                    _this.core.width = _this.getPxW();
                    _this.core.height = _this.getPxH();
                    _this.context2D = _this.core.getContext("2d");
                    _this.context2D.drawImage(imageObj, 0, 0);
                    break;
            }

        }

        //
        this.fillColor = function (color) {
            if (!this.context2D) {
                this.context2D = this.core.getContext("2d");
            }
            this.context2D.fillStyle = color;
            this.context2D.fillRect(0, 0, this.core.width, this.core.height);
        }

        //====
        //
        this.setAppearance_CanvasView(options);
        //
        if (_this.appearance.src) { // src 優先
            _this.loadImage(_this.appearance.src);
        } else {
            if (_this.appearance.image) {
                _this.setImage(_this.appearance.image);
            }
        }

    }
    CanvasView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        if (this.transform.constSize) {
            return;
        }

        //
        if (this.appearance.src) { // src 優先
            this.loadImage(this.appearance.src);
        } else {
            if (this.appearance.image) {
                this.setImage(this.appearance.image);
            }
        }

    }
    CanvasView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        this.setAppearance_CanvasView(options);
    }
    CanvasView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        this.setAppearance_CanvasView(options);
        this.redraw();//this also makes parent-redraw
    }

    //== 7.1 VideoCanvasView == //TODO test
    function VideoCanvasView(parentView, options) {
        CoreView.call(this, "canvas", parentView, options);
        var _this = this;

        this.context2D = null;

        //TODO find a better way to init
        this.setSourceResolution = function (w, h) {
            this.core.width = w;
            this.core.height = h;

            this.context2D = this.core.getContext("2d");
            this.context2D.fillStyle = '#000000';
            this.context2D.fillRect(0, 0, this.core.width, this.core.height);
        }

        this.fillColor = function (color) {
            if (!this.context2D) {
                this.context2D = this.core.getContext("2d");
            }
            this.context2D.fillStyle = color;
            this.context2D.fillRect(0, 0, this.core.width, this.core.height);
        }

        this.drawByVideo = function (video) {
            if (this.context2D) {
                this.context2D.drawImage(video, 0, 0);
            }
        }

    }
    VideoCanvasView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        if (this.transform.constSize) {
            return;
        }

    }
    VideoCanvasView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        //this.setAppearance_VideoCanvasView(options);
    }
    VideoCanvasView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        //this.setAppearance_VideoCanvasView(options);
        this.redraw();//this also makes parent-redraw
    }

    //== 7.2 DrawCanvasView ==
    function DrawCanvasView(parentView, options) {
        CoreView.call(this, "canvas", parentView, options);
        var _this = this;

        //
        this.cb = {
            getData: null
        };

        //
        this.core.width = this.getPxW();
        this.core.height = this.getPxH();
        this.context2D = this.core.getContext("2d");

        var ctx = this.context2D;
        this.lineWidth = 5;
        this.lineJoin = 'round';
        this.lineCap = 'round';
        this.canvasBG = 'rgba(0,0,0,0)';
        this.strokeStyle = '#000';
        fillColor(this.canvasBG);

        var isDown = false;
        var canvasX, canvasY;
        var history = [];
        var redoQueue = [];

        //this.addListener('mouseover', function (event) {
        //    console.log(event.clientX + ", " + event.clientY);
        //});

        // == Mouse ==
        this.addListener('mousedown', function (e) {
            //
            var rect = _this.core.getBoundingClientRect();
            //
            isDown = true;
            ctx.lineWidth = _this.lineWidth;
            ctx.lineJoin = _this.lineJoin;
            ctx.lineCap = _this.lineCap;
            ctx.beginPath();
            canvasX = e.pageX - rect.left;
            canvasY = e.pageY - rect.top;
            ctx.moveTo(canvasX, canvasY);

            //
            var data = {
                type: "begin",
                x: canvasX / _this.getPxW() * 100,
                y: canvasY / _this.getPxH() * 100,
                strokeStyle: _this.strokeStyle
            };
            history.push(data);
            //
            if (_this.cb.getData) _this.cb.getData(data);

        });
        this.addListener('mousemove', function (e) {
            if (isDown !== false) {
                //
                var rect = _this.core.getBoundingClientRect();
                //
                canvasX = e.pageX - rect.left;
                canvasY = e.pageY - rect.top;
                ctx.lineTo(canvasX, canvasY);
                ctx.strokeStyle = _this.strokeStyle;
                ctx.stroke();

                //
                var data = {
                    type: "move",
                    x: canvasX / _this.getPxW() * 100,
                    y: canvasY / _this.getPxH() * 100
                };
                history.push(data);
                //
                if (_this.cb.getData) _this.cb.getData(data);

            }
        });
        this.addListener('mouseup', function (e) {
            if (isDown) {
                isDown = false;
                ctx.closePath();

                //
                var data = {
                    type: "end"
                };
                history.push(data);
                //
                if (_this.cb.getData) _this.cb.getData(data);
            }

        });
        this.addListener('mouseout', function (e) {
            if (isDown) {
                isDown = false;
                ctx.closePath();

                //
                var data = {
                    type: "end"
                };
                history.push(data);
                //
                if (_this.cb.getData) _this.cb.getData(data);
            }
        });

        // == Touch == //TODO 如滑鼠事件般修改座標
        var draw = {
            started: false,
            start: function (e) {
                //
                var rect = _this.core.getBoundingClientRect();
                //
                ctx.lineWidth = _this.lineWidth;
                ctx.beginPath();
                canvasX = e.touches[0].pageX - rect.left;
                canvasY = e.touches[0].pageY - rect.top;
                ctx.moveTo(canvasX, canvasY);

                this.started = true;

                //
                history.push({
                    type: "begin",
                    x: canvasX,
                    y: canvasY
                });

            },
            move: function (e) {
                if (this.started) {
                    //
                    var rect = _this.core.getBoundingClientRect();
                    //
                    canvasX = e.touches[0].pageX - rect.left;
                    canvasY = e.touches[0].pageY - rect.top;
                    ctx.lineTo(canvasX, canvasY);

                    ctx.strokeStyle = _this.strokeStyle;
                    ctx.lineWidth = _this.lineWidth;
                    ctx.stroke();

                    //
                    history.push({
                        type: "move",
                        x: canvasX,
                        y: canvasY
                    });

                }

            },
            end: function (e) {
                this.started = false;

                //
                history.push({
                    type: "end"
                });

            }
        };
        this.addListener('touchstart', draw.start);
        this.addListener('touchend', draw.end);
        this.addListener('touchmove', draw.move);
        //// Disable Page Move
        //document.body.addEventListener('touchmove', function (evt) {
        //    evt.preventDefault();
        //}, false);

        // ====
        function fillColor(color) {
            ctx.fillStyle = color;
            ctx.fillRect(0, 0, _this.core.width, _this.core.height);
        }
        this.redrawHistory = function () {
            //
            fillColor(_this.canvasBG);
            //
            var p;
            for (id in history) {
                p = history[id];
                if (p.type == "begin") {
                    _this.strokeStyle = p.strokeStyle;
                    ctx.lineWidth = _this.lineWidth;
                    ctx.lineJoin = _this.lineJoin;
                    ctx.lineCap = _this.lineCap;
                    ctx.beginPath();
                    canvasX = p.x / 100 * _this.getPxW();
                    canvasY = p.y / 100 * _this.getPxH();
                    ctx.moveTo(canvasX, canvasY);

                } else if (p.type == "move") {
                    canvasX = p.x / 100 * _this.getPxW();
                    canvasY = p.y / 100 * _this.getPxH();
                    ctx.lineTo(canvasX, canvasY);
                    ctx.strokeStyle = _this.strokeStyle;
                    ctx.stroke();

                } else {
                    ctx.closePath();
                }

            }
        }
        this.clearHistory = function () {
            history = [];
            //TODO redraw
        }
        //TODO undo redo
        //this.undo = function () {
        //}
        //this.redo = function () {
        //}
        this.clearAll = function () {
            history = [];
            ctx.clearRect(0, 0, _this.core.width, _this.core.height);
        }

        //test
        this.remoteDraw = function (data) {
            switch (data.type) {
                case 'begin':
                    _this.strokeStyle = data.strokeStyle;
                    ctx.lineWidth = _this.lineWidth;
                    ctx.lineJoin = _this.lineJoin;
                    ctx.lineCap = _this.lineCap;
                    ctx.beginPath();
                    canvasX = data.x / 100 * _this.getPxW();
                    canvasY = data.y / 100 * _this.getPxH();
                    ctx.moveTo(canvasX, canvasY);
                    break;

                case 'move':
                    canvasX = data.x / 100 * _this.getPxW();
                    canvasY = data.y / 100 * _this.getPxH();
                    ctx.lineTo(canvasX, canvasY);
                    ctx.strokeStyle = _this.strokeStyle;
                    ctx.stroke();
                    break;

                case 'end':
                    ctx.closePath();
                    break;

                default:
                    return;
            }

            history.push(data);

        }

    }
    DrawCanvasView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        if (this.transform.constSize) {
            return;
        }

        //
        this.core.width = this.getPxW();
        this.core.height = this.getPxH();
        this.redrawHistory();

    }
    DrawCanvasView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        //this.setAppearance_DrawCanvasView(options);
    }
    DrawCanvasView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        //this.setAppearance_DrawCanvasView(options);
        this.redraw();//this also makes parent-redraw
    }


    //== 8. TextView ==
    var TextViewEnums = {};
    Object.defineProperties(TextViewEnums, {
        "MODE_NORMAL": { // all flexible
            get: function () { return "normal"; },
            configurable: false
        },
        /*
        "MODE_SINLGLE": { // 不會自動縮放, 且必須設定 fontSize, 若初始時未設定, 會自動給予一個符合介面的值.
            get: function () { return "single"; },
            configurable: false
        },
        */
        "MODE_MULTI": { // 同 MODE_SINLGLE, 但會在超過 maxPxW 時換行, 且自動增加元件高度.
            get: function () { return "multi"; },
            configurable: false
        }
    });
    function TextView(parentView, options) {
        CoreView.call(this, "div", parentView, options);
        var _this = this;

        var mode = null;
        var autoFontSize = true;
        Object.defineProperties(this, {
            "Mode": {
                get: function () { return mode; },
                //set: function (value) { mode = value; },
                configurable: false
            },
            "AutoFontSize": {
                get: function () { return autoFontSize; },
                configurable: false
            }
        });

        this.appearance.text = "";
        this.appearance.txtColor = "#000";
        this.appearance.txtAlign = "left"; // left | right | center | justify
        this.appearance.fontSize = -1;
        this.appearance.maxPxW = -1; // -1 for unlimited

        this.setAppearance_TextView = function (options) {
            var appearance = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.appearance !== "undefined" && options.appearance != null) {
                    appearance = options.appearance;
                } else {
                    appearance = options;
                }
            }

            if (appearance) {
                if (!(typeof appearance.text === "undefined" || appearance.text == null)) {
                    _this.appearance.text = appearance.text;
                }
                if (!(typeof appearance.txtColor === "undefined" || appearance.txtColor == null)) {
                    _this.appearance.txtColor = appearance.txtColor;
                }
                if (!(typeof appearance.txtAlign === "undefined" || appearance.txtAlign == null)) {
                    _this.appearance.txtAlign = appearance.txtAlign;
                }
                if (!(typeof appearance.fontSize === "undefined" || appearance.fontSize == null)) {
                    _this.appearance.fontSize = appearance.fontSize;
                    autoFontSize = false;
                    _this.transform.constSize = true;// TODO: 也改成 readonly
                }
                if (!(typeof appearance.maxPxW === "undefined" || appearance.maxPxW == null)) {
                    _this.appearance.maxPxW = appearance.maxPxW;
                }
                if (!(typeof appearance.mode === "undefined" || appearance.mode == null)) { // !!!
                    if(mode == null){ // 只能在初始時設定一次
                        mode = appearance.mode;
                        switch (mode) {
                            case TextViewEnums.MODE_SINLGLE:
                                if (_this.appearance.fontSize == -1){
                                    _this.appearance.fontSize = (_this.getPxH() * 0.85) + "px";
                                }
                                autoFontSize = false;
                                _this.transform.constSize = true;
                                break;

                            case TextViewEnums.MODE_MULTI:
                                if (_this.appearance.fontSize == -1) {
                                    _this.appearance.fontSize = (_this.getPxH() * 0.85) + "px";
                                }
                                autoFontSize = false;
                                _this.transform.constSize = true;
                                break;

                            default: // & MODE_NORMAL
                                _this.transform.constSize = false;//
                                break;

                        }
                    }
                }

            }

        }

        //
        this.setText = function (text) {
            _this.appearance.text = text;
            _this.core.innerHTML = text;

            var lineHeight = null;

            // CHECK: 以下細部參數可能不需要 expose 到 appearance 裡
            switch (mode) {
                case TextViewEnums.MODE_SINLGLE:
                    //暫不使用
                    /*
                    if (_this.appearance.maxPxW != -1) {
                        if (_this.getPxW() < _this.appearance.maxPxW) {
                            var detectW = detectTextSize(_this.appearance.text, _this.appearance.fontSize)[0];
                            if (detectW > _this.getPxW()) {
                                if (detectW < _this.appearance.maxPxW) {
                                    //增寬
                                    _this.setTransformPx({ transform: { w: detectW } });
                                }
                            }

                        }
                    }
                    */
                    break;

                case TextViewEnums.MODE_MULTI:
                    this.core.style.wordWrap = "break-word";

                    if (_this.appearance.maxPxW != -1) {
                        var size_single_line = detectTextSize(_this.appearance.text, _this.appearance.fontSize);
                        var w_single_line = size_single_line.w;
                        var h_single_line = size_single_line.h;

                        if (w_single_line > _this.appearance.maxPxW) {
                            //符合多行
                            if (w_single_line >= _this.getPxW()) {
                                //需先把元件寬度加到最大
                                _this.setTransformPx({ w: _this.appearance.maxPxW });
                            }
                            //設定多行需要的元件高度
                            var size_multi = detectTextSize_multi(_this.appearance.text, _this.appearance.fontSize, _this.getPxW());
                            var detectH = size_multi.h;
                            //console.log("detectH " + detectH);
                            _this.setTransformPx({ h: detectH });
                            lineHeight = size_multi.lineHeight;//

                        } else {
                            //符合單行
                            if (w_single_line > _this.getPxW()) {
                                //設定新寬度
                                _this.setTransformPx({ w: w_single_line, h: h_single_line });
                            } else {
                                _this.setTransformPx({ h: h_single_line });
                            }

                        }

                    } else {
                        //未設定最大寬度, 只根據多行高度調整
                        var size_multi = detectTextSize_multi(_this.appearance.text, _this.appearance.fontSize, _this.getPxW());
                        var detectH = size_multi.h;
                        //console.log("detectH " + detectH);
                        _this.setTransformPx({ h: detectH });
                        lineHeight = size_multi.lineHeight;//

                    }

                    break;

                default: // & MODE_NORMAL
                    if (this.AutoFontSize) {
                        this.appearance.fontSize = calcAutoFontSize_TextView(_this.appearance.text, {w: _this.getPxW(), h: _this.getPxH()});
                    }
                    this.core.style.fontSize = this.appearance.fontSize + "px";
                    break;

            }

            // lineHeight
            if (lineHeight) {
                _this.core.style.lineHeight = lineHeight + "px";
            } else {
                _this.core.style.lineHeight = _this.getPxH() + "px";
            }

        }
        this.getText = function () {
            return _this.core.innerHTML;
        }

        //====
        //
        this.setAppearance_TextView(options);
        this.core.style.overflowX = "hidden";
        this.core.style.overflowY = "hidden";
        //
        this.core.style.color = this.appearance.txtColor;
        this.core.align = this.appearance.txtAlign;
        if (this.AutoFontSize) {
            this.appearance.fontSize = calcAutoFontSize_TextView(_this.appearance.text, { w: _this.getPxW(), h: _this.getPxH() });
        }
        this.core.style.fontSize = this.appearance.fontSize + "px";
        this.setText(this.appearance.text);

    }
    TextView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        this.core.style.color = this.appearance.txtColor;
        this.core.align = this.appearance.txtAlign;
        this.setText(this.appearance.text);
        if (this.AutoFontSize) {
            this.appearance.fontSize = calcAutoFontSize(this.appearance.text, { w: this.getPxW(), h: this.getPxH() });
        }

        //
        if (this.transform.constSize) {
            return;
        }

        //

    }
    TextView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        this.setAppearance_TextView(options);
    }
    TextView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        this.setAppearance_TextView(options);
        this.redraw();//this also makes parent-redraw
    }


    //== 9. ButtonView ==
    var ButtonStatus = {};
    Object.defineProperties(ButtonStatus, {
        "STATUS_NORMAL": {
            get: function () { return "normal"; },
            configurable: false
        },
        "STATUS_ENTER": {
            get: function () { return "enter"; },
            configurable: false
        },
        "STATUS_DOWN": {
            get: function () { return "down"; },
            configurable: false
        }
    });
    function ButtonView(parentView, options) {
        CoreView.call(this, "button", parentView, options);
        var _this = this;

        var status = ButtonStatus.STATUS_NORMAL;
        var autoFontSize = true;
        Object.defineProperties(this, {
            "Status": {
                get: function () { return status; },
                configurable: false
            },
            "AutoFontSize": {
                get: function () { return autoFontSize; },
                configurable: false
            }
        });

        this.appearance.bgNormal = null;
        this.appearance.bgMouseEnter = null;
        this.appearance.bgMouseDown = null;
        this.appearance.txtColor = null;
        this.appearance.txtNormal = null;
        this.appearance.txtMouseEnter = null;
        this.appearance.txtMouseDown = null;
        this.appearance.text = "";
        this.appearance.fontSize = -1;

        this.setAppearance_ButtonView = function (options) {
            var appearance = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.appearance !== "undefined" && options.appearance != null) {
                    appearance = options.appearance;
                } else {
                    appearance = options;
                }
            }

            if (appearance) {
                if (!(typeof appearance.bgColor === "undefined" || appearance.bgColor == null)) {
                    if (_this.appearance.bgNormal == null){
                        _this.appearance.bgNormal = appearance.bgColor; //
                    }
                }
                if (!(typeof appearance.bgMouseEnter === "undefined" || appearance.bgMouseEnter == null)) {
                    _this.appearance.bgMouseEnter = appearance.bgMouseEnter;
                }
                if (!(typeof appearance.bgMouseDown === "undefined" || appearance.bgMouseDown == null)) {
                    _this.appearance.bgMouseDown = appearance.bgMouseDown;
                }
                if (!(typeof appearance.txtColor === "undefined" || appearance.txtColor == null)) {
                    _this.appearance.txtColor = appearance.txtColor;
                    if (_this.appearance.txtNormal == null){
                        _this.appearance.txtNormal = appearance.txtColor; //
                    }
                }
                if (!(typeof appearance.txtMouseEnter === "undefined" || appearance.txtMouseEnter == null)) {
                    _this.appearance.txtMouseEnter = appearance.txtMouseEnter;
                }
                if (!(typeof appearance.txtMouseDown === "undefined" || appearance.txtMouseDown == null)) {
                    _this.appearance.txtMouseDown = appearance.txtMouseDown;
                }
                if (!(typeof appearance.text === "undefined" || appearance.text == null)) {
                    _this.appearance.text = appearance.text;
                }
                if (!(typeof appearance.fontSize === "undefined" || appearance.fontSize == null)) {
                    _this.appearance.fontSize = appearance.fontSize;
                    autoFontSize = false;
                }

            }

        }

        this.getText = function () {
            return _this.appearance.text;
        }

        //
        var isMouseOver = false;
        this.addListener("mouseleave", function () {
            isMouseOver = false;
            status = ButtonStatus.STATUS_NORMAL;//
            _this.setAppearance({
                appearance: {
                    bgColor: _this.appearance.bgNormal,
                    txtColor: _this.appearance.txtNormal
                }
            });
        });
        this.addListener("mouseenter", function () {
            isMouseOver = true;
            status = ButtonStatus.STATUS_ENTER;//
            _this.setAppearance({
                appearance: {
                    bgColor: _this.appearance.bgMouseEnter,
                    txtColor: _this.appearance.txtMouseEnter
                }
            });
        });
        this.addListener("mousedown", function () {
            status = ButtonStatus.STATUS_DOWN;//
            _this.setAppearance({
                appearance: {
                    bgColor: _this.appearance.bgMouseDown,
                    txtColor: _this.appearance.txtMouseDown
                }
            });
        });
        this.addListener("mouseup", function () {
            if (isMouseOver) {
                status = ButtonStatus.STATUS_ENTER;//
                _this.setAppearance({
                    appearance: {
                        bgColor: _this.appearance.bgMouseEnter,
                        txtColor: _this.appearance.txtMouseEnter
                    }
                });
            } else {
                status = ButtonStatus.STATUS_NORMAL;//
                _this.setAppearance({
                    appearance: {
                        bgColor: _this.appearance.bgNormal,
                        txtColor: _this.appearance.txtNormal
                    }
                });
            }
        });

        //
        this.setAppearance_ButtonView(options);
        //
        this.core.style.overflowX = "hidden";
        this.core.style.overflowY = "hidden";
        this.core.appendChild(document.createTextNode(this.appearance.text));
        this.core.style.color = this.appearance.textColor;
        if (this.AutoFontSize) {
            this.appearance.fontSize = calcAutoFontSize(this.appearance.text, { w: this.getPxW(), h: this.getPxH() });
        }
        this.core.style.fontSize = this.appearance.fontSize + "px";

    }
    ButtonView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        this.core.firstChild.nodeValue = this.appearance.text;
        switch (this.Status) {
            case ButtonStatus.STATUS_ENTER:
                this.core.style.color = this.appearance.txtMouseEnter;
                this.core.style.backgroundColor = this.appearance.bgMouseEnter;
                break;

            case ButtonStatus.STATUS_DOWN:
                this.core.style.color = this.appearance.txtMouseDown;
                this.core.style.backgroundColor = this.appearance.bgMouseDown;
                break;

            default: // & ButtonStatus.STATUS_NORMAL
                this.core.style.color = this.appearance.txtNormal;
                this.core.style.backgroundColor = this.appearance.bgNormal;
                break;

        }
        if (this.AutoFontSize) {
            this.appearance.fontSize = calcAutoFontSize(this.appearance.text, { w: this.getPxW(), h: this.getPxH() });
        }
        this.core.style.fontSize = this.appearance.fontSize + "px";

        //
        if (this.transform.constSize) {
            return;
        }

        //

    }
    ButtonView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        this.setAppearance_ButtonView(options);
    }
    ButtonView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        this.setAppearance_ButtonView(options);
        this.redraw();//this also makes parent-redraw
    }


    //== 10. RadioButtonView ==
    function RadioButtonView(parentView, options, radioGroup) {
        CoreView.call(this, "button", parentView, options);
        var _this = this;
        radioGroup.push(this);

        var _checked = false;
        var autoFontSize = true;
        Object.defineProperties(this, {
            "checked": {
                get: function () { return _checked; },
                configurable: false
            },
            "AutoFontSize": {
                get: function () { return autoFontSize; },
                configurable: false
            }
        });

        this.appearance.bgNormal = null;
        this.appearance.bgChecked = null;
        this.appearance.txtColor = null;
        this.appearance.txtNormal = null;
        this.appearance.txtChecked= null;
        this.appearance.text = "";
        this.appearance.fontSize = -1;

        this.setAppearance_RadioButtonView = function (options) {
            var appearance = null;
            if (typeof options !== "undefined" && options != null) {
                if (typeof options.appearance !== "undefined" && options.appearance != null) {
                    appearance = options.appearance;
                } else {
                    appearance = options;
                }
            }

            if (appearance) {
                if (!(typeof appearance.bgColor === "undefined" || appearance.bgColor == null)) {
                    if (_this.appearance.bgNormal == null){
                        _this.appearance.bgNormal = appearance.bgColor; //
                    }
                }
                if (!(typeof appearance.bgChecked === "undefined" || appearance.bgChecked == null)) {
                    _this.appearance.bgChecked = appearance.bgChecked;
                }
                if (!(typeof appearance.txtColor === "undefined" || appearance.txtColor == null)) {
                    _this.appearance.txtColor = appearance.txtColor;
                    if (_this.appearance.txtNormal == null){
                        _this.appearance.txtNormal = appearance.txtColor; //
                    }
                }
                if (!(typeof appearance.txtChecked === "undefined" || appearance.txtChecked == null)) {
                    _this.appearance.txtChecked = appearance.txtChecked;
                }
                if (!(typeof appearance.text === "undefined" || appearance.text == null)) {
                    _this.appearance.text = appearance.text;
                }
                if (!(typeof appearance.fontSize === "undefined" || appearance.fontSize == null)) {
                    _this.appearance.fontSize = appearance.fontSize;
                    autoFontSize = false;
                }

            }

        }

        this.getText = function () {
            return _this.appearance.text;
        }

        //
        this.setChecked = function (checked) {
            _checked = checked;
            if(_checked) {
                _this.setAppearance({
                    appearance: {
                        bgColor: _this.appearance.bgChecked,
                        txtColor: _this.appearance.txtChecked
                    }
                });
                for(id in radioGroup) {
                    if(radioGroup[id] !== _this) {
                        radioGroup[id].setChecked(false);
                    }
                }

            } else {
                _this.setAppearance({
                    appearance: {
                        bgColor: _this.appearance.bgNormal,
                        txtColor: _this.appearance.txtNormal
                    }
                });

            }
        }

        // == events ==
        this.on = {
            click: null
        };

        this.addListener('click', function () {
            var thisId = -1;
            for(id in radioGroup) {
                if(radioGroup[id] === _this) {
                    thisId = id;
                    break;
                }
            }
            _this.setChecked(true);
            if(_this.on.click) _this.on.click(thisId);
        });



        // == init ==
        this.setAppearance_RadioButtonView(options);
        //
        this.core.style.overflowX = "hidden";
        this.core.style.overflowY = "hidden";
        this.core.appendChild(document.createTextNode(this.appearance.text));
        this.core.style.color = this.appearance.textColor;
        if (this.AutoFontSize) {
            this.appearance.fontSize = calcAutoFontSize(this.appearance.text, { w: this.getPxW(), h: this.getPxH() });
        }
        this.core.style.fontSize = this.appearance.fontSize + "px";

    }
    RadioButtonView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //
        this.core.firstChild.nodeValue = this.appearance.text;
        if(this.checked) {
            this.core.style.color = this.appearance.txtChecked;
            this.core.style.backgroundColor = this.appearance.bgChecked;
        } else {
            this.core.style.color = this.appearance.txtNormal;
            this.core.style.backgroundColor = this.appearance.bgNormal;
        }

        if (this.AutoFontSize) {
            this.appearance.fontSize = calcAutoFontSize(this.appearance.text, { w: this.getPxW(), h: this.getPxH() });
        }
        this.core.style.fontSize = this.appearance.fontSize + "px";

        //
        if (this.transform.constSize) {
            return;
        }

        //

    }
    RadioButtonView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        this.setAppearance_RadioButtonView(options);
    }
    RadioButtonView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        this.setAppearance_RadioButtonView(options);
        this.redraw();//this also makes parent-redraw
    }



    // == 11. IFrameView ==
    function IFrameView(parentView, options) {
        CoreView.call(this, "iframe", parentView, options);
        var _this = this;


        // = extened options =
        var opt = {};
        opt.src = '';


        // = events =


        // == methods ==

        // = private =
        function setOptions(options) {
            if (options) {
                if (!(typeof options.src === "undefined" || options.src == null)) {
                    opt.src = options.src;
                    _this.core.src = opt.src;
                }
            }
        }

        // = public =
        this.setOptions = setOptions;


        // == init ==
        this.core.style.position = "static";
        setOptions(options);

    }
    IFrameView.prototype.redraw = function () {
        CoreView.prototype.redraw.call(this);

        //

        //
        if (this.transform.constSize) {
            return;
        }


    }
    IFrameView.prototype.initAppearance = function (options) {
        CoreView.prototype.initAppearance.call(this, options);
        //this.setAppearance_IFrameView(options);
    }
    IFrameView.prototype.setAppearance = function (options) {
        CoreView.prototype.setAppearance.call(this, options, false);//stop parent-redraw
        //this.setAppearance_IFrameView(options);
        this.redraw();//this also makes parent-redraw
    }




    //====

    //== Utils ==
    var adj_detect_w = 5;

    var textSizeDetector = null;
    function detectTextSize(text, fontSize) {
        if (textSizeDetector) {
            textSizeDetector.parentNode.removeChild(textSizeDetector);//textSizeDetector.remove();
            textSizeDetector = null;
            //console.log("remove old textSizeDetector");
        }

        //console.log("create textSizeDetector");
        textSizeDetector = document.createElement("div");
        textSizeDetector.style.position = "absolute";
        textSizeDetector.style.visibility = "hidden";
        textSizeDetector.style.width = "auto";
        textSizeDetector.style.height = "auto";
        textSizeDetector.style.whiteSpace = "nowrap";
        document.body.appendChild(textSizeDetector);

        textSizeDetector.style.fontSize = fontSize + "px";
        textSizeDetector.innerHTML = text;

        //console.log({ w: (textSizeDetector.clientWidth + adj_detect_w), h: textSizeDetector.clientHeight, lineHeight: textSizeDetector.clientHeight });//
        return { w: (textSizeDetector.clientWidth + adj_detect_w), h: textSizeDetector.clientHeight, lineHeight: textSizeDetector.clientHeight };
    }

    var textSizeDetector_multi = null;
    function detectTextSize_multi(text, fontSize, wPx) {
        if (textSizeDetector_multi) {
            textSizeDetector_multi.parentNode.removeChild(textSizeDetector_multi);//textSizeDetector.remove();
            textSizeDetector_multi = null;
            //console.log("remove old textSizeDetector_multi");
        }

        //console.log("create textSizeDetector");
        textSizeDetector_multi = document.createElement("div");
        textSizeDetector_multi.style.position = "absolute";
        textSizeDetector_multi.style.visibility = "hidden";
        textSizeDetector_multi.style.width = (wPx - adj_detect_w) + "px";
        textSizeDetector_multi.style.height = "auto";
        textSizeDetector_multi.style.wordWrap = "break-word";
        document.body.appendChild(textSizeDetector_multi);

        textSizeDetector_multi.style.fontSize = fontSize + "px";
        textSizeDetector_multi.innerHTML = text;

        var size_single = detectTextSize(text, fontSize);
        if (size_single.h == textSizeDetector_multi.scrollHeight) {
            return detectTextSize(text, fontSize);
        } else {
            return { w: (textSizeDetector_multi.scrollWidth + adj_detect_w), h: textSizeDetector_multi.scrollHeight, lineHeight: size_single.lineHeight };
        }
    }

    // for single line
    function calcAutoFontSize(text, targetSize) {
        var targetW = targetSize.w * 0.8;
        var fontSize = targetSize.h * 0.5;
        var resultFontSize, size;
        while (true) {
            size = detectTextSize(text, fontSize);
            if ((targetW - size.w) >= 1) {
                return fontSize;
            } else {
                fontSize -= 4;
                if (fontSize <= 4) {
                    return 4;
                }
            }
        }
    }
    function calcAutoFontSize_TextView(text, targetSize) {
        var targetW = targetSize.w * 0.8;
        var fontSize = targetSize.h * 0.8;
        var resultFontSize, size;
        while (true) {
            size = detectTextSize(text, fontSize);
            if ((targetW - size.w) >= 1) {
                return fontSize;
            } else {
                fontSize -= 4;
                if (fontSize <= 4) {
                    return 4;
                }
            }
        }
    }

    //
    var textWidthDetector;
    function detectTextWidth(text, fontSize) {
        if (textWidthDetector) {
            textWidthDetector.parentNode.removeChild(textWidthDetector);
            textWidthDetector = null;
        }

        textWidthDetector = document.createElement("div");
        textWidthDetector.style.visibility = "hidden";
        textWidthDetector.style.display = 'inline-block';
        textWidthDetector.style.fontSize = fontSize + "px";
        textWidthDetector.innerHTML = text;
        document.body.appendChild(textWidthDetector);

        return textWidthDetector.offsetWidth;
    }


    //====
    String.prototype.paddingLeft = function (paddingValue) {
       return String(paddingValue + this).slice(-paddingValue.length);
    };




	//====
    return {
        // views
		CoreView: CoreView,
		DivView: DivView,
		RootView: RootView,
		TextBoxView: TextBoxView,
		TextAreaView: TextAreaView,
		VideoView: VideoView,
		CanvasView: CanvasView,
		VideoCanvasView: VideoCanvasView,
		DrawCanvasView: DrawCanvasView,
		TextView: TextView,
        ButtonView: ButtonView,
        RadioButtonView: RadioButtonView,
        IFrameView: IFrameView,

        // enums
		TextViewEnums: TextViewEnums,

        // utils
        detectTextSize: detectTextSize,
        detectTextWidth: detectTextWidth

	};

})();


// ====
if(typeof module !== 'undefined') {
    module.exports = BerBer;
}
