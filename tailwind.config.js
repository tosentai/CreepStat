/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './Views/Shared/**/*.cshtml'
    ],
    theme: {
        extend: {
            fontFamily: {
                'rubik': ['Rubik', 'sans-serif'],
            },
            textShadow: {
                sm: '0 1px 2px var(--tw-shadow-color)',
                DEFAULT: '0 2px 4px var(--tw-shadow-color)',
                lg: '0 8px 16px var(--tw-shadow-color)',
                outline: '2px 2px 0 #000, -2px -2px 0 #000, 2px -2px 0 #000, -2px 2px 0 #000',
            },
        },
    },
    plugins: [
        require('tailwindcss-textshadow'),
    ],
}
