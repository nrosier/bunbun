--- src/webui/www/private/scripts/lib/mootools-1.2-core-yc.js.orig	2020-01-30 22:19:56.694307000 +0100
+++ src/webui/www/private/scripts/lib/mootools-1.2-core-yc.js	2020-01-30 22:22:05.237838000 +0100
@@ -37,7 +37,7 @@
 A.forEachMethod=function(J){if(!w){for(var I=0,G=y.length;I<G;I++){J.call(E,E[y[I]],y[I]);}}for(var H in E){J.call(E,E[H],H);}};}return c;};c("String",String,["charAt","charCodeAt","concat","indexOf","lastIndexOf","match","quote","replace","search","slice","split","substr","substring","trim","toLowerCase","toUpperCase"])("Array",Array,["pop","push","reverse","shift","sort","splice","unshift","concat","join","slice","indexOf","lastIndexOf","filter","forEach","every","map","some","reduce","reduceRight"])("Number",Number,["toExponential","toFixed","toLocaleString","toPrecision"])("Function",f,["apply","call","bind"])("RegExp",RegExp,["exec","test"])("Object",Object,["create","defineProperty","defineProperties","keys","getPrototypeOf","getOwnPropertyDescriptor","getOwnPropertyNames","preventExtensions","isExtensible","seal","isSealed","freeze","isFrozen"])("Date",Date,["now"]);
 Object.extend=t.overloadSetter();Date.extend("now",function(){return +(new Date);});new k("Boolean",Boolean);Number.prototype.$family=function(){return isFinite(this)?"number":"null";
 }.hide();Number.extend("random",function(v,i){return Math.floor(Math.random()*(i-v+1)+v);});var l=Object.prototype.hasOwnProperty;Object.extend("forEach",function(i,w,x){for(var v in i){if(l.call(i,v)){w.call(x,i[v],v,i);
-}}});Object.each=Object.forEach;Array.implement({forEach:function(x,y){for(var w=0,v=this.length;w<v;w++){if(w in this){x.call(y,this[w],w,this);}}},each:function(i,v){Array.forEach(this,i,v);
+}}});Object.each=Object.forEach;Array.implement({forEach:function(x,y){for(var w=0,v=this.length;w<v;w++){if(w in this){x.call(y,this[w],w,this);}}},each:function(i,v){Array.prototype.forEach(this,i,v);
 return this;}});var s=function(i){switch(e(i)){case"array":return i.clone();case"object":return Object.clone(i);default:return i;}};Array.implement("clone",function(){var v=this.length,w=new Array(v);
 while(v--){w[v]=s(this[v]);}return w;});var a=function(v,i,w){switch(e(w)){case"object":if(e(v[i])=="object"){Object.merge(v[i],w);}else{v[i]=Object.clone(w);
 }break;case"array":v[i]=w.clone();break;default:v[i]=w;}return v;};Object.extend({merge:function(C,y,x){if(e(y)=="string"){return a(C,y,x);}for(var B=1,w=arguments.length;
@@ -50,14 +50,14 @@
 }return n;};var m=Array.type;Array.type=function(i){return u(i,Array)||m(i);};this.$A=function(i){return Array.from(i).slice();};this.$arguments=function(v){return function(){return arguments[v];
 };};this.$chk=function(i){return !!(i||i===0);};this.$clear=function(i){clearTimeout(i);clearInterval(i);return null;};this.$defined=function(i){return(i!=null);
 };this.$each=function(w,v,x){var i=e(w);((i=="arguments"||i=="collection"||i=="array"||i=="elements")?Array:Object).each(w,v,x);};this.$empty=function(){};
-this.$extend=function(v,i){return Object.append(v,i);};this.$H=function(i){return new g(i);};this.$merge=function(){var i=Array.slice(arguments);i.unshift({});
+this.$extend=function(v,i){return Object.append(v,i);};this.$H=function(i){return new g(i);};this.$merge=function(){var i=Array.prototype.slice(arguments);i.unshift({});
 return Object.merge.apply(null,i);};this.$lambda=f.from;this.$mixin=Object.merge;this.$random=Number.random;this.$splat=Array.from;this.$time=Date.now;
 this.$type=function(i){var v=e(i);if(v=="elements"){return"array";}return(v=="null")?false:v;};this.$unlink=function(i){switch(e(i)){case"object":return Object.clone(i);
 case"array":return Array.clone(i);case"hash":return new g(i);default:return i;}};})();Array.implement({every:function(c,d){for(var b=0,a=this.length>>>0;
 b<a;b++){if((b in this)&&!c.call(d,this[b],b,this)){return false;}}return true;},filter:function(d,f){var c=[];for(var e,b=0,a=this.length>>>0;b<a;b++){if(b in this){e=this[b];
 if(d.call(f,e,b,this)){c.push(e);}}}return c;},indexOf:function(c,d){var b=this.length>>>0;for(var a=(d<0)?Math.max(0,b+d):d||0;a<b;a++){if(this[a]===c){return a;
 }}return -1;},map:function(c,e){var d=this.length>>>0,b=Array(d);for(var a=0;a<d;a++){if(a in this){b[a]=c.call(e,this[a],a,this);}}return b;},some:function(c,d){for(var b=0,a=this.length>>>0;
-b<a;b++){if((b in this)&&c.call(d,this[b],b,this)){return true;}}return false;},clean:function(){return this.filter(function(a){return a!=null;});},invoke:function(a){var b=Array.slice(arguments,1);
+b<a;b++){if((b in this)&&c.call(d,this[b],b,this)){return true;}}return false;},clean:function(){return this.filter(function(a){return a!=null;});},invoke:function(a){var b=Array.prototype.slice(arguments,1);
 return this.map(function(c){return c[a].apply(c,b);});},associate:function(c){var d={},b=Math.min(this.length,c.length);for(var a=0;a<b;a++){d[c[a]]=this[a];
 }return d;},link:function(c){var a={};for(var e=0,b=this.length;e<b;e++){for(var d in c){if(c[d](this[e])){a[d]=this[e];delete c[d];break;}}}return a;},contains:function(a,b){return this.indexOf(a,b)!=-1;
 },append:function(a){this.push.apply(this,a);return this;},getLast:function(){return(this.length)?this[this.length-1]:null;},getRandom:function(){return(this.length)?this[Number.random(0,this.length-1)]:null;
@@ -79,11 +79,11 @@
 }});Number.alias("each","times");(function(b){var a={};b.each(function(c){if(!Number[c]){a[c]=function(){return Math[c].apply(null,[this].concat(Array.from(arguments)));
 };}});Number.implement(a);})(["abs","acos","asin","atan","atan2","ceil","cos","exp","floor","log","max","min","pow","sin","sqrt","tan"]);Function.extend({attempt:function(){for(var b=0,a=arguments.length;
 b<a;b++){try{return arguments[b]();}catch(c){}}return null;}});Function.implement({attempt:function(a,c){try{return this.apply(c,Array.from(a));}catch(b){}return null;
-},bind:function(e){var a=this,b=arguments.length>1?Array.slice(arguments,1):null,d=function(){};var c=function(){var g=e,h=arguments.length;if(this instanceof c){d.prototype=a.prototype;
-g=new d;}var f=(!b&&!h)?a.call(g):a.apply(g,b&&h?b.concat(Array.slice(arguments)):b||arguments);return g==e?f:g;};return c;},pass:function(b,c){var a=this;
+},bind:function(e){var a=this,b=arguments.length>1?Array.prototype.slice(arguments,1):null,d=function(){};var c=function(){var g=e,h=arguments.length;if(this instanceof c){d.prototype=a.prototype;
+g=new d;}var f=(!b&&!h)?a.call(g):a.apply(g,b&&h?b.concat(Array.prototype.slice(arguments)):b||arguments);return g==e?f:g;};return c;},pass:function(b,c){var a=this;
 if(b!=null){b=Array.from(b);}return function(){return a.apply(c,b||arguments);};},delay:function(b,c,a){return setTimeout(this.pass((a==null?[]:a),c),b);
 },periodical:function(c,b,a){return setInterval(this.pass((a==null?[]:a),b),c);}});delete Function.prototype.bind;Function.implement({create:function(b){var a=this;
-b=b||{};return function(d){var c=b.arguments;c=(c!=null)?Array.from(c):Array.slice(arguments,(b.event)?1:0);if(b.event){c=[d||window.event].extend(c);}var e=function(){return a.apply(b.bind||null,c);
+b=b||{};return function(d){var c=b.arguments;c=(c!=null)?Array.from(c):Array.prototype.slice(arguments,(b.event)?1:0);if(b.event){c=[d||window.event].extend(c);}var e=function(){return a.apply(b.bind||null,c);
 };if(b.delay){return setTimeout(e,b.delay);}if(b.periodical){return setInterval(e,b.periodical);}if(b.attempt){return Function.attempt(e);}return e();};
 },bind:function(c,b){var a=this;if(b!=null){b=Array.from(b);}return function(){return a.apply(c,b||arguments);};},bindWithEvent:function(c,b){var a=this;
 if(b!=null){b=Array.from(b);}return function(d){return a.apply(c,(b==null)?arguments:[d].concat(b));};},run:function(a,b){return this.apply(b,Array.from(a));
@@ -293,10 +293,10 @@
 }else{if(Type.isEnumerable(e)){return new Elements(e);}}}return new Elements(arguments);});}var w={before:function(l,e){var D=e.parentNode;if(D){D.insertBefore(l,e);
 }},after:function(l,e){var D=e.parentNode;if(D){D.insertBefore(l,e.nextSibling);}},bottom:function(l,e){e.appendChild(l);},top:function(l,e){e.insertBefore(l,e.firstChild);
 }};w.inside=w.bottom;Object.each(w,function(l,D){D=D.capitalize();var e={};e["inject"+D]=function(E){l(this,document.id(E,true));return this;};e["grab"+D]=function(E){l(document.id(E,true),this);
-return this;};Element.implement(e);});var j={},d={};var k={};Array.forEach(["type","value","defaultValue","accessKey","cellPadding","cellSpacing","colSpan","frameBorder","rowSpan","tabIndex","useMap"],function(e){k[e.toLowerCase()]=e;
+return this;};Element.implement(e);});var j={},d={};var k={};Array.prototype.forEach(["type","value","defaultValue","accessKey","cellPadding","cellSpacing","colSpan","frameBorder","rowSpan","tabIndex","useMap"],function(e){k[e.toLowerCase()]=e;
 });k.html="innerHTML";k.text=(document.createElement("div").textContent==null)?"innerText":"textContent";Object.forEach(k,function(l,e){d[e]=function(D,E){D[l]=E;
 };j[e]=function(D){return D[l];};});var x=["compact","nowrap","ismap","declare","noshade","checked","disabled","readOnly","multiple","selected","noresize","defer","defaultChecked","autofocus","controls","autoplay","loop"];
-var h={};Array.forEach(x,function(e){var l=e.toLowerCase();h[l]=e;d[l]=function(D,E){D[e]=!!E;};j[l]=function(D){return !!D[e];};});Object.append(d,{"class":function(e,l){("className" in e)?e.className=(l||""):e.setAttribute("class",l);
+var h={};Array.prototype.forEach(x,function(e){var l=e.toLowerCase();h[l]=e;d[l]=function(D,E){D[e]=!!E;};j[l]=function(D){return !!D[e];};});Object.append(d,{"class":function(e,l){("className" in e)?e.className=(l||""):e.setAttribute("class",l);
 },"for":function(e,l){("htmlFor" in e)?e.htmlFor=l:e.setAttribute("for",l);},style:function(e,l){(e.style)?e.style.cssText=l:e.setAttribute("style",l);
 },value:function(e,l){e.value=(l!=null)?l:"";}});j["class"]=function(e){return("className" in e)?e.className||null:e.getAttribute("class");};var f=document.createElement("button");
 try{f.type="button";}catch(z){}if(f.type!="button"){d.type=function(e,l){e.setAttribute("type",l);};}f=null;var p=document.createElement("input");p.value="t";
@@ -524,4 +524,4 @@
 }build+=">";for(var param in params){if(params[param]){build+='<param name="'+param+'" value="'+params[param]+'" />';}}build+="</object>";this.object=((container)?container.empty():new Element("div")).set("html",build).firstChild;
 },replaces:function(element){element=document.id(element,true);element.parentNode.replaceChild(this.toElement(),element);return this;},inject:function(element){document.id(element,true).appendChild(this.toElement());
 return this;},remote:function(){return Swiff.remote.apply(Swiff,[this.toElement()].append(arguments));}});Swiff.CallBacks={};Swiff.remote=function(obj,fn){var rs=obj.CallFunction('<invoke name="'+fn+'" returntype="javascript">'+__flash__argumentsToXML(arguments,2)+"</invoke>");
-return eval(rs);};})();
\ No newline at end of file
+return eval(rs);};})();
