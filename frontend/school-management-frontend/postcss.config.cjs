// postcss.config.cjs
// Renamed from .js to .cjs to explicitly use CommonJS syntax,
// resolving 'module is not defined' error when 'type: "module"' is in package.json.
module.exports = {
  plugins: {
    // This correctly uses the @tailwindcss/postcss plugin
    '@tailwindcss/postcss': {},
    autoprefixer: {},
  },
};
