import{o as s,c as a,b as e,d as f,k as p,a as r,w as l,F as g,h as y,u as n,f as _,g as v,t as c,l as k,r as w,M as B}from"./app.c85615b2.js";import{_ as C}from"./home.66645626.js";const M={preserveAspectRatio:"xMidYMid meet",viewBox:"0 0 24 24",width:"1.2em",height:"1.2em"},b=e("path",{fill:"currentColor",d:"M8.59 16.58L13.17 12L8.59 7.41L10 6l6 6l-6 6l-1.41-1.42Z"},null,-1),L=[b];function R(t,i){return s(),a("svg",M,L)}const N={name:"mdi-chevron-right",render:R},V={class:"flex","aria-label":"Breadcrumb"},$={role:"list",class:"flex items-center sm:space-x-4 flex-wrap sm:flex-nowrap"},A={class:"text-gray-600 hover:text-gray-700"},F=e("span",{class:"sr-only"},"Home",-1),H={class:"flex items-center"},S={class:"ml-1 sm:ml-4 sm:text-2xl text-gray-500 hover:text-gray-700"},z={class:"flex items-center"},D=["aria-current"],E=["aria-current"],I=f({__name:"AppBreadcrumb",props:{crumbs:{default:()=>[]},name:null,href:null,current:{type:Boolean,default:!0}},setup(t){const{current:i}=t,h=!!k().currentRoute.value.path.includes("posts"),u=p(()=>i?"page":void 0);return(Z,j)=>{const x=C,o=w("router-link"),m=N;return s(),a("nav",V,[e("ol",$,[e("li",null,[e("div",null,[r(o,{to:"/"},{default:l(()=>[e("a",A,[r(x,{class:"flex-shrink-0 h-10 w-10","aria-hidden":"true"}),F])]),_:1})])]),(s(!0),a(g,null,y(t.crumbs,d=>(s(),a("li",null,[e("div",H,[r(m,{class:"flex-shrink-0 h-8 w-8 text-gray-400","aria-hidden":"true"}),r(o,{to:d.href},{default:l(()=>[e("a",S,c(d.name),1)]),_:2},1032,["to"])])]))),256)),e("li",null,[e("div",z,[r(m,{class:"flex-shrink-0 h-8 w-8 text-gray-400","aria-hidden":"true"}),n(h)?(s(),_(n(B),{key:0,style:{fontSize:"2em"},class:"ml-4"})):v("",!0),t.href?(s(),_(o,{key:1,to:t.href},{default:l(()=>[e("a",{class:"ml-1 sm:ml-4 sm:text-2xl text-gray-500 hover:text-gray-700","aria-current":n(u)},c(t.name),9,D)]),_:1},8,["to"])):(s(),a("span",{key:2,class:"ml-1 sm:ml-4 sm:text-3xl text-gray-700","aria-current":n(u)},c(t.name),9,E))])])])])}}});export{I as _};
