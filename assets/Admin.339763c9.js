import{o as n,c as i,b as t,d as x,e as d,u as e,f as g,w as m,g as v,a as u,t as o,h as y,F as $,s as k,i as w}from"./app.304c8fcb.js";import{_ as A}from"./AppPage.vue_vue_type_script_setup_true_lang.8317d686.js";import{_ as N}from"./SecondaryButton.vue_vue_type_script_setup_true_lang.58c24860.js";import"./AppBreadcrumb.vue_vue_type_script_setup_true_lang.56caa97e.js";import"./home.ad457c71.js";const B={preserveAspectRatio:"xMidYMid meet",viewBox:"0 0 24 24",width:"1.2em",height:"1.2em"},C=t("path",{fill:"currentColor",d:"M12 1L3 5v6c0 5.55 3.84 10.74 9 12c5.16-1.26 9-6.45 9-12V5l-9-4m0 4a3 3 0 0 1 3 3a3 3 0 0 1-3 3a3 3 0 0 1-3-3a3 3 0 0 1 3-3m5.13 12A9.69 9.69 0 0 1 12 20.92A9.69 9.69 0 0 1 6.87 17c-.34-.5-.63-1-.87-1.53c0-1.65 2.71-3 6-3s6 1.32 6 3c-.24.53-.53 1.03-.87 1.53Z"},null,-1),V=[C];function M(_,s){return n(),i("svg",B,V)}const S={name:"mdi-shield-account",render:M},b={class:"flex flex-col items-center justify-center"},j={class:"mt-2"},F=w("Sign Out"),R=x({__name:"Admin",setup(_){var c,r;const s=d.value,p=(r=(c=d.value)==null?void 0:c.roles)!=null?r:[];return(f,l)=>{const h=S;return e(s)?(n(),g(A,{key:0,title:"Admin Page",class:"max-w-prose flex justify-center"},{default:m(()=>[t("div",b,[u(h,{class:"w-36 h-36 text-gray-700"}),t("div",null,o(e(s).displayName),1),t("div",null,o(e(s).userName),1),t("div",j,[(n(!0),i($,null,y(e(p),a=>(n(),i("span",{key:a,class:"ml-3 inline-flex items-center px-3 py-0.5 rounded-full text-xs font-medium leading-5 bg-indigo-100 text-indigo-800"},o(a),1))),128))]),u(N,{class:"mt-8",onClick:l[0]||(l[0]=a=>e(k)(f.$router,"/admin"))},{default:m(()=>[F]),_:1})])]),_:1})):v("",!0)}}});export{R as default};