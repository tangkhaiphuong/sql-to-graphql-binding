(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-237db988"],{"499d":function(e,t,r){"use strict";r.r(t);var n=function(){var e=this,t=e.$createElement,r=e._self._c||t;return r("WindowFrame",[r("v-container",{attrs:{fluid:"","fill-height":"","ma-0":"","pa-0":""}},[r("splitpanes",{staticClass:"default-theme",on:{resize:e.handleMouseDown,resized:e.handleMouseUp}},[r("pane",{attrs:{size:50}},[r("textarea",{ref:"root",staticClass:"d-none"})]),r("pane",{attrs:{size:50}},[r("iframe",{ref:"iframe",attrs:{width:"100%",height:"100%",frameborder:"0"}})])],1)],1),r("portal",{staticClass:"portal",attrs:{to:"app-bar"}},[r("tool-bar")],1)],1)},a=[],i=(r("c975"),r("498a"),r("96cf"),r("1da1")),o=r("8bbf"),s=r.n(o),c=r("752c"),u=(r("d3b7"),r("d4ec")),p=r("bee2"),l=(r("99af"),r("796d")),f=r.n(l),h=r("b85c"),d=function(){function e(){Object(u["a"])(this,e),this._waiters=null}return Object(p["a"])(e,[{key:"lock",value:function(){var e=this;return new Promise((function(t){e._waiters?e._waiters.push(t):(e._waiters=[],t())}))}},{key:"unlock",value:function(){if(this._waiters){var e,t=Object(h["a"])(this._waiters);try{for(t.s();!(e=t.n()).done;){var r=e.value;r()}}catch(n){t.e(n)}finally{t.f()}}this._waiters=null}}]),e}(),m=5e3,g="https://apis.google.com/js/api.js";function w(e){if(!e.getBasicProfile)return null;var t=e.getBasicProfile();return{id:t.getId(),name:t.getName(),firstName:t.getGivenName(),lastName:t.getFamilyName(),avatarUrl:t.getImageUrl(),email:t.getEmail()}}var v=function(){function e(t){Object(u["a"])(this,e),this._locker=new d,this._config=t}return Object(p["a"])(e,[{key:"load",value:function(){return window.gapi?Promise.resolve(window.gapi):new Promise((function(e,t){var r=document.createElement("script");r.src=g;var n=setTimeout((function(){r.remove(),t(new Error("gapi load timeout."))}),m);document.head.appendChild(r),r.onload=function(){clearTimeout(n),window.gapi||t(new Error("gapi load error.")),e(window.gapi)}}))}},{key:"libraryLoad",value:function(e){return this.load().then((function(t){return t[e]?Promise.resolve(t[e]):new Promise((function(r,n){t.load(e,{timeout:m,callback:function(){r(t[e])},onerror:function(t){n(new Error("Error on gapi ".concat(e," load: ").concat(t.message)))},ontimeout:function(){n(new Error("Error on gapi ".concat(e," load: timeout")))}})}))}))}},{key:"libraryInit",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(t){var r;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.next=2,this.libraryLoad(t);case 2:return r=e.sent,e.abrupt("return",r.init(this._config).then((function(e){return Promise.resolve("auth2"===t?e:r)}),(function(){return Promise.reject(new Error("Error on gapi ".concat(t," init.")))})));case 4:case"end":return e.stop()}}),e,this)})));function t(t){return e.apply(this,arguments)}return t}()},{key:"isSignedIn",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.next=2,this.libraryInit("auth2");case 2:return t=e.sent,e.abrupt("return",t.isSignedIn.get());case 4:case"end":return e.stop()}}),e,this)})));function t(){return e.apply(this,arguments)}return t}()},{key:"getCurrentUserProfile",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.prev=0,e.next=3,this.libraryInit("auth2");case 3:if(t=e.sent,!t.isSignedIn.get()){e.next=6;break}return e.abrupt("return",w(t.currentUser.get()));case 6:return e.abrupt("return",null);case 9:throw e.prev=9,e.t0=e["catch"](0),e.t0.message=e.t0.message||e.t0.error,e.t0;case 13:case"end":return e.stop()}}),e,this,[[0,9]])})));function t(){return e.apply(this,arguments)}return t}()},{key:"signIn",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t,r;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.prev=0,e.next=3,this.libraryInit("auth2");case 3:if(t=e.sent,!t.isSignedIn.get()){e.next=6;break}return e.abrupt("return",w(t.currentUser.get()));case 6:return e.next=8,t.signIn();case 8:return r=e.sent,e.abrupt("return",w(r));case 12:throw e.prev=12,e.t0=e["catch"](0),e.t0.message=e.t0.message||e.t0.error,e.t0;case 16:case"end":return e.stop()}}),e,this,[[0,12]])})));function t(){return e.apply(this,arguments)}return t}()},{key:"signOut",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.prev=0,e.next=3,this.libraryInit("auth2");case 3:t=e.sent,t.isSignedIn.get()&&t.disconnect(),e.next=11;break;case 7:throw e.prev=7,e.t0=e["catch"](0),e.t0.message=e.t0.message||e.t0.error,e.t0;case 11:case"end":return e.stop()}}),e,this,[[0,7]])})));function t(){return e.apply(this,arguments)}return t}()},{key:"getAccessToken",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t,r;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.next=2,this.libraryInit("auth2");case 2:if(t=e.sent,!t.isSignedIn.get()){e.next=18;break}if(r=t.currentUser.get().getAuthResponse(),!(new Date(r.expires_at-6e4)<=new Date)){e.next=16;break}return e.prev=6,e.next=9,this._locker.lock();case 9:if(r=t.currentUser.get().getAuthResponse(),!(new Date(r.expires_at-6e4)<=new Date)){e.next=13;break}return e.next=13,t.currentUser.get().reloadAuthResponse();case 13:return e.prev=13,this._locker.unlock(),e.finish(13);case 16:return r=t.currentUser.get().getAuthResponse(),e.abrupt("return",r.access_token);case 18:return e.abrupt("return",null);case 19:case"end":return e.stop()}}),e,this,[[6,,13,16]])})));function t(){return e.apply(this,arguments)}return t}()}]),e}(),b={discoveryDocs:["https://people.googleapis.com/$discovery/rest"],clientId:"904796907236-ngv4pq5g9lq9b884cfudlhm27i9oj2u0.apps.googleusercontent.com",scope:"https://www.googleapis.com/auth/script.send_mail https://www.googleapis.com/auth/gmail.send https://www.googleapis.com/auth/drive https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/script.external_request https://www.googleapis.com/auth/spreadsheets https://www.googleapis.com/auth/userinfo.email"},y=new v(b),k=function(e,t){return x.apply(this,arguments)};function x(){return x=Object(i["a"])(regeneratorRuntime.mark((function e(t,r){var n,a,i,o,s,c,u;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return n=f.a.stringify({supportsAllDrives:!0,supportsTeamDrives:!0,uploadType:"multipart"},{addQueryPrefix:!0,encodeValuesOnly:!0}),a="https://www.googleapis.com/upload/drive/v3/files".concat(n),i="--cpmp3vn",o="--".concat(i,"\n"),o+="Content-Type: application/json; charset=UTF-8\n\n",o+="".concat(JSON.stringify(t),"\n"),o+="--".concat(i,"\n"),o+="Content-Type: ".concat(t.mimeType,"\n"),o+="Content-Transfer-Encoding: base64\n\n",o+="".concat(r,"\n"),o+="--".concat(i,"--"),e.t0=o,e.t1="Bearer ",e.next=15,y.getAccessToken();case 15:return e.t2=e.sent,e.t3=e.t1.concat.call(e.t1,e.t2),e.t4="multipart/related; boundary=".concat(i),e.t5={Authorization:e.t3,"Content-Type":e.t4},s={method:"post",body:e.t0,headers:e.t5},e.next=22,fetch(a,s);case 22:if(c=e.sent,403!==c.status){e.next=25;break}throw new Error("Your account doesn't have permission to create drive file");case 25:if(200===c.status){e.next=31;break}return e.t6=Error,e.next=29,c.text();case 29:throw e.t7=e.sent,new e.t6(e.t7);case 31:return e.next=33,c.json();case 33:return u=e.sent,e.abrupt("return",u);case 35:case"end":return e.stop()}}),e)}))),x.apply(this,arguments)}var O,R=function(){function e(t){Object(u["a"])(this,e),this.document=pdfMake.createPdf(t.documentDefinition),this.folderId=t.folderId,this.fileName=t.fileName}return Object(p["a"])(e,[{key:"getBlob",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t=this;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.abrupt("return",new Promise((function(e){t.document.getBlob((function(t){e(t)}))})));case 1:case"end":return e.stop()}}),e)})));function t(){return e.apply(this,arguments)}return t}()},{key:"getBase64",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t=this;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.abrupt("return",new Promise((function(e){t.document.getBase64((function(t){e(t)}))})));case 1:case"end":return e.stop()}}),e)})));function t(){return e.apply(this,arguments)}return t}()},{key:"getBuffer",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t=this;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.abrupt("return",new Promise((function(e){t.document.getBuffer((function(t){e(t)}))})));case 1:case"end":return e.stop()}}),e)})));function t(){return e.apply(this,arguments)}return t}()},{key:"getDataUrl",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t=this;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:return e.abrupt("return",new Promise((function(e){t.document.getDataUrl((function(t){e(t)}))})));case 1:case"end":return e.stop()}}),e)})));function t(){return e.apply(this,arguments)}return t}()},{key:"open",value:function(){this.document.open()}},{key:"print",value:function(){this.document.print()}},{key:"create",value:function(){var e=Object(i["a"])(regeneratorRuntime.mark((function e(){var t,r,n;return regeneratorRuntime.wrap((function(e){while(1)switch(e.prev=e.next){case 0:if(this.folderId){e.next=2;break}throw new Error("Please provide folder Id to save PDF (Vui lòng cung cấp Folder Id lưu file PDF trên Google Drive)");case 2:return e.next=4,this.getBase64();case 4:return t=e.sent,r={name:this.fileName,mimeType:"application/pdf",parents:[this.folderId]},e.next=8,k(r,t);case 8:return n=e.sent,e.abrupt("return",n);case 10:case"end":return e.stop()}}),e,this)})));function t(){return e.apply(this,arguments)}return t}()}]),e}(),j=r("38ad"),I=r("512e"),_=r("60bb"),D=r.n(_),P=r("9b02"),S=r.n(P),E=function(){var e=this,t=e.$createElement,r=e._self._c||t;return r("div",{staticClass:"panel-wrapper"},[r("v-spacer"),r("v-tooltip",{attrs:{top:""},scopedSlots:e._u([{key:"activator",fn:function(t){var n=t.on;return[r("div",e._g({},n),[r("app-tile",{attrs:{bottomLine:!1,active:!1,icon:"mdi-flower"},on:{click:e.prettifyDoc}})],1)]}}])},[r("span",[e._v("Prettify")])]),r("v-spacer")],1)},C=[],T=r("9526"),U=s.a.extend({components:{AppTile:T["a"]},methods:{prettifyDoc:function(){this.$store.dispatch("pdf/prettifyDoc")}}}),B=U,$=(r("973e"),r("2877")),N=Object($["a"])(B,E,C,!1,null,"1aa1151a",null),A=N.exports,F=s.a.extend({data:function(){return{editor:null,src:""}},computed:{pdfState:function(){return this.$store.state.pdf},doc:function(){return this.pdfState.doc}},watch:{doc:function(e){if(this.editor){var t=this.editor.getCursor();this.editor.setValue(e),this.editor.setCursor(t)}}},mounted:function(){var e=this;this.editor=j["a"].fromTextArea(this.$refs.root,{lineNumbers:!0,matchBrackets:!0,dragDrop:!0,autoCloseBrackets:!0,autofocus:!0,theme:"neo",mode:"javascript",extraKeys:{F11:function(e){e.setOption("fullScreen",!e.getOption("fullScreen"))},Esc:function(e){e.getOption("fullScreen")&&e.setOption("fullScreen",!1)},"Ctrl-Space":function(){e.editor.showHint({completeSingle:!0,container:e.$refs.wrapper})}}}),this.editor.setValue(this.doc.trim()||"{\n    content: [\n\n    ],\n    defaultStyle: {\n        fontSize: 14,\n    }\n}"),this.editor.setSize("100%","100%"),this.editor.on("change",(function(t){e.$store.commit("pdf/setDoc",t.getValue()),e.handleChange()}))},components:{WindowFrame:c["a"],Splitpanes:I["Splitpanes"],Pane:I["Pane"],ToolBar:A},methods:{handleMouseDown:function(){-1===this.src.indexOf("application/pdf")&&(this.src=this.$refs.iframe.src),this.$refs.iframe.src=""},handleMouseUp:function(){this.src.indexOf("application/pdf")>-1&&(this.$refs.iframe.src=this.src)},handleChange:function(){var e=this;void 0===O&&(O=D.a.debounce(function(){var t=Object(i["a"])(regeneratorRuntime.mark((function t(r){var n,a,i;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:if(n=null,t.prev=1,n=S.a.parse(r),!n){t.next=9;break}return a=new R({fileName:"",documentDefinition:n,folderId:""}),t.next=7,a.getDataUrl();case 7:i=t.sent,e.$refs.iframe.src=i;case 9:t.next=13;break;case 11:t.prev=11,t.t0=t["catch"](1);case 13:case"end":return t.stop()}}),t,null,[[1,11]])})));return function(e){return t.apply(this,arguments)}}(),800)),this.doc&&O(this.doc)}}}),z=F,M=(r("d9c9"),Object($["a"])(z,n,a,!1,null,"374bd644",null));t["default"]=M.exports},"4d53":function(e,t,r){},"973e":function(e,t,r){"use strict";var n=r("4d53"),a=r.n(n);a.a},c042:function(e,t,r){},d9c9:function(e,t,r){"use strict";var n=r("c042"),a=r.n(n);a.a}}]);
//# sourceMappingURL=chunk-237db988.bea2c5de.js.map