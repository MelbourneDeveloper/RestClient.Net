import techdoc from "eleventy-plugin-techdoc";

export default function(eleventyConfig) {
  eleventyConfig.addPlugin(techdoc, {
    site: {
      name: "RestClient.Net",
      url: "https://restclient.net",
      description: "The safest way to make REST calls in C#. Built with functional programming, type safety, and modern .NET patterns.",
    },
    features: {
      blog: true,
      docs: true,
      darkMode: true,
      i18n: true,
    },
    i18n: {
      defaultLanguage: 'en',
      languages: ['en', 'zh'],
    },
  });

  eleventyConfig.addPassthroughCopy("src/assets");

  return {
    dir: { input: "src", output: "_site" },
    markdownTemplateEngine: "njk",
  };
}
