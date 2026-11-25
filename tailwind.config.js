/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        // IBM Carbon: IBM Plex Sans
        sans: ['"IBM Plex Sans"', 'Arial', 'sans-serif'],
        mono: ['"IBM Plex Mono"', 'monospace'],
      },
      colors: {
        // IBM Carbon color tokens
        'ibm-blue': '#0f62fe',
        'ibm-blue-hover': '#0353e9',
        'ibm-blue-light': '#d0e2ff',
        'ibm-red': '#da1e28',
        'ibm-red-light': '#fff1f1',
        'ibm-orange': '#ff832b',
        'ibm-green': '#198038',
        'ibm-green-light': '#defbe6',
        'ibm-yellow': '#f1c21b',
        // Carbon neutrals
        'carbon-10': '#f4f4f4',
        'carbon-20': '#e0e0e0',
        'carbon-30': '#c6c6c6',
        'carbon-50': '#8d8d8d',
        'carbon-60': '#6f6f6f',
        'carbon-70': '#525252',
        'carbon-80': '#393939',
        'carbon-90': '#262626',
        'carbon-100': '#161616',
        // UI Shell
        'ui-background': '#ffffff',
        'ui-01': '#f4f4f4',
        'ui-02': '#ffffff',
        'text-01': '#161616',
        'text-02': '#525252',
        'text-helper': '#6f6f6f',
      },
      borderRadius: {
        // IBM Carbon: 0px corners throughout
        DEFAULT: '0px',
        none: '0px',
        sm: '0px',
        md: '0px',
        lg: '0px',
        xl: '0px',
        '2xl': '0px',
        full: '9999px', // keep for pills/badges only
      },
      boxShadow: {
        // IBM Carbon: no drop shadows
        DEFAULT: 'none',
        sm: 'none',
        md: 'none',
        lg: 'none',
        xl: 'none',
        '2xl': 'none',
        // Only keep focus ring
        focus: '0 0 0 3px #0f62fe',
      },
      fontSize: {
        // Carbon type scale
        'body-sm': ['12px', { lineHeight: '16px', letterSpacing: '0.32px' }],
        'body': ['14px', { lineHeight: '20px', letterSpacing: '0.16px' }],
        'body-lg': ['16px', { lineHeight: '24px', letterSpacing: '0px' }],
        'label': ['12px', { lineHeight: '16px', letterSpacing: '0.32px' }],
        'heading-sm': ['14px', { lineHeight: '18px', letterSpacing: '0.16px', fontWeight: '600' }],
        'heading': ['20px', { lineHeight: '28px', letterSpacing: '0px', fontWeight: '400' }],
        'heading-lg': ['28px', { lineHeight: '36px', letterSpacing: '0px', fontWeight: '400' }],
        'display': ['42px', { lineHeight: '50px', letterSpacing: '0px', fontWeight: '300' }],
      },
    },
  },
  plugins: [],
}
