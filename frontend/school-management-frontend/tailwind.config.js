/** @type {import('tailwindcss').Config} */
export default {
  // This 'content' array is the most critical part for Tailwind CSS to work.
  // It tells Tailwind where to scan for your utility classes.
  content: [
    // Include your main HTML file
    "./index.html",
    // This line tells Tailwind to scan ALL .js, .ts, .jsx, and .tsx files
    // within your 'src' directory and its subdirectories.
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      // You can define custom colors, fonts, spacing, etc. here
      fontFamily: {
        sans: ['Inter', 'sans-serif'], // Set Inter as default sans-serif font
      },
    },
  },
  plugins: [],
}
