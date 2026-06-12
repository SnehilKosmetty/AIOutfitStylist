/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        ink: '#111827',
        mist: '#f6f7fb',
        coral: '#f9735b',
        teal: {
          200: '#99f6e4',
          300: '#5eead4',
          700: '#0f766e',
          800: '#115e59'
        },
        gold: {
          DEFAULT: '#d4a72c'
        }
      }
    }
  },
  plugins: []
};
