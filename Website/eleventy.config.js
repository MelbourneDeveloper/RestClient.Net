import syntaxHighlight from "@11ty/eleventy-plugin-syntaxhighlight";
import markdownIt from "markdown-it";
import anchor from "markdown-it-anchor";

const origin = process.env.SITE_URL || "https://melbournedeveloper.github.io";
const prefix = "/" + (process.env.SITE_PATH_PREFIX || "").replace(/^\/+|\/+$/g, "") + "/";
const basePath = prefix === "//" ? "/" : prefix;
const absoluteUrl = value => new URL(String(value || "/").replace(/^\//,""), new URL(basePath,origin)).href;
export default function (config) {
  config.addPlugin(syntaxHighlight);
  config.setLibrary("md", markdownIt({html:true,linkify:true}).use(anchor, {slugify:s=>s.toLowerCase().replace(/\s+/g,"-").replace(/[^\p{L}\p{N}_-]/gu,"")}));
  config.addPassthroughCopy("src/assets");
  config.addPassthroughCopy({"src/api/reference/api.json":"api/reference/api.json", "src/api/reference/schema.json":"api/reference/schema.json"});
  config.addCollection("posts", api=>api.getFilteredByGlob("src/blog/*.md").sort((a,b)=>b.date-a.date));
  config.addCollection("zhposts", api=>api.getFilteredByGlob("src/zh/blog/*.md").sort((a,b)=>b.date-a.date));
  config.addCollection("journalArchives", api => {
    const archives = [];
    for (const lang of ["en", "zh"]) {
      const prefix = lang === "zh" ? "/zh" : "";
      const posts = api.getFilteredByGlob(`src${prefix}/blog/*.md`).sort((a,b)=>b.date-a.date);
      for (const kind of ["tags", "categories"]) {
        archives.push({url:`${prefix}/blog/${kind}/`,lang,title:lang === "zh" ? (kind === "tags" ? "博客主题" : "博客分类") : (kind === "tags" ? "Journal topics" : "Journal categories"),posts});
        const values = [...new Set(posts.flatMap(p=>kind === "tags" ? (p.data.tags || []).filter(t=>!["post","posts"].includes(t)) : p.data.category ? [p.data.category] : []))];
        for (const value of values) archives.push({url:`${prefix}/blog/${kind}/${value.toLowerCase().replace(/[^a-z0-9]+/g,"-")}/`,lang,title:(lang === "zh" ? "博客 / " : "Journal / ")+value,posts:posts.filter(p=>kind === "tags" ? (p.data.tags || []).includes(value) : p.data.category === value)});
      }
    }
    return archives;
  });
  config.addCollection("publicPages", api=>api.getAll().filter(p=>p.url && !p.data.eleventyExcludeFromCollections));
  config.addFilter("isoDate", value=>new Date(value).toISOString());
  config.addFilter("dateFormat", value=>new Date(value).toLocaleDateString("en",{year:"numeric",month:"long",day:"numeric",timeZone:"UTC"}));
  config.addFilter("xmlEscape", value=>String(value??"").replace(/[<>&"']/g,c=>({"<":"&lt;",">":"&gt;","&":"&amp;",'"':"&quot;","'":"&apos;"}[c])));
  config.addFilter("absoluteUrl", absoluteUrl);
  config.addFilter("translationUrl", (url,lang,pages=[])=>{const route=(lang==="zh"?"/zh":"")+url.replace(/^\/zh(?=\/)/,"");return pages.some(p=>p.url===route)?route:(lang==="zh"?"/zh/":"/");});
  config.addFilter("hasTranslation",(url,lang,pages=[])=>pages.some(p=>p.url===(lang==="zh"?"/zh":"")+url.replace(/^\/zh(?=\/)/,"")));
  config.addFilter("section", url=>url.replace(/^\/zh(?=\/)/,"").split("/")[1]||"home");
  config.addFilter("withoutHeading", html=>html.replace(/<h1[^>]*>[\s\S]*?<\/h1>/,""));
  config.addFilter("json", value=>JSON.stringify(value).replace(/</g,"\\u003c"));
  config.addTransform("deploymentPaths", function (content) {
    if (!(this.page?.outputPath || this.outputPath || "").endsWith(".html") || basePath === "/") return content;
    return content.replace(/(href|src|action)=("|')\/(?!\/)([^"']*)\2/g, (all,attr,quote,path)=>`${attr}=${quote}${path.startsWith(basePath.slice(1))?"/":""}${path.startsWith(basePath.slice(1))?path:basePath+path}${quote}`);
  });
  return {dir:{input:"src",output:"_site"},markdownTemplateEngine:"njk",pathPrefix:basePath};
}
