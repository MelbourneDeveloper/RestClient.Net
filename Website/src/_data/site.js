const origin = process.env.SITE_URL || "https://melbournedeveloper.github.io";
const prefix = "/" + (process.env.SITE_PATH_PREFIX || "").replace(/^\/+|\/+$/g, "");
export default {
  name: "RestClient.Net", title: "RestClient.Net",
  description: "Typed HTTP for C#. Explicit results, functional error handling, and exhaustiveness checking for every outcome.",
  url: new URL(prefix, origin).href.replace(/\/$/, ""), author: "Christian Findlay",
  ogImage: "/assets/images/social-card.png", themeColor: "#101313"
};
