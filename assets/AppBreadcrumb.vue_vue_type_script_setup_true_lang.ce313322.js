import{o as s,c as r,b as e,d as f,k as p,a as n,w as c,F as v,h as g,u as a,f as h,g as y,t as i,l as k,r as w}from"./app.7634e678.js";import{_ as M}from"./home.3c932c84.js";const B={preserveAspectRatio:"xMidYMid meet",viewBox:"0 0 24 24",width:"1.2em",height:"1.2em"},C=e("path",{fill:"currentColor",d:"M8.59 16.58L13.17 12L8.59 7.41L10 6l6 6l-6 6l-1.41-1.42Z"},null,-1),b=[C];function $(t,o){return s(),r("svg",B,b)}const V={name:"mdi-chevron-right",render:$},R={preserveAspectRatio:"xMidYMid meet",viewBox:"0 0 32 32",width:"1.2em",height:"1.2em"},z=e("path",{fill:"currentColor",d:"M29.693 25.849H2.308a2.311 2.311 0 0 1-2.307-2.307V8.459a2.311 2.311 0 0 1 2.307-2.307h27.385A2.311 2.311 0 0 1 32 8.459v15.078a2.305 2.305 0 0 1-2.307 2.307zm-22-4.62v-6l3.078 3.849l3.073-3.849v6h3.078V10.771h-3.078l-3.073 3.849l-3.078-3.849H4.615v10.464zM28.307 16h-3.078v-5.229h-3.073V16h-3.078l4.615 5.385z"},null,-1),A=[z];function H(t,o){return s(),r("svg",R,A)}const L={name:"cib-markdown",render:H},N={class:"flex","aria-label":"Breadcrumb"},F={role:"list",class:"flex items-center sm:space-x-4 flex-wrap sm:flex-nowrap"},S={class:"text-gray-600 hover:text-gray-700"},Y=e("span",{class:"sr-only"},"Home",-1),D={class:"flex items-center"},E={class:"ml-1 sm:ml-4 sm:text-2xl text-gray-500 hover:text-gray-700"},Z={class:"flex items-center"},j=["aria-current"],q=["aria-current"],P=f({__name:"AppBreadcrumb",props:{crumbs:{default:()=>[]},name:null,href:null,current:{type:Boolean,default:!0}},setup(t){const{current:o}=t,_=!!k().currentRoute.value.path.includes("posts"),u=p(()=>o?"page":void 0);return(I,J)=>{const x=M,l=w("router-link"),m=V;return s(),r("nav",N,[e("ol",F,[e("li",null,[e("div",null,[n(l,{to:"/"},{default:c(()=>[e("a",S,[n(x,{class:"flex-shrink-0 h-10 w-10","aria-hidden":"true"}),Y])]),_:1})])]),(s(!0),r(v,null,g(t.crumbs,d=>(s(),r("li",null,[e("div",D,[n(m,{class:"flex-shrink-0 h-8 w-8 text-gray-400","aria-hidden":"true"}),n(l,{to:d.href},{default:c(()=>[e("a",E,i(d.name),1)]),_:2},1032,["to"])])]))),256)),e("li",null,[e("div",Z,[n(m,{class:"flex-shrink-0 h-8 w-8 text-gray-400","aria-hidden":"true"}),a(_)?(s(),h(a(L),{key:0,style:{fontSize:"2em"},class:"ml-4"})):y("",!0),t.href?(s(),h(l,{key:1,to:t.href},{default:c(()=>[e("a",{class:"ml-1 sm:ml-4 sm:text-2xl text-gray-500 hover:text-gray-700","aria-current":a(u)},i(t.name),9,j)]),_:1},8,["to"])):(s(),r("span",{key:2,class:"ml-1 sm:ml-4 sm:text-3xl text-gray-700","aria-current":a(u)},i(t.name),9,q))])])])])}}});export{P as _};
