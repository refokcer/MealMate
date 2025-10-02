const navToggle = document.querySelector('.nav-toggle');
const nav = document.querySelector('.app-nav');

if (navToggle && nav) {
    navToggle.addEventListener('click', () => {
        const isOpen = nav.classList.toggle('is-open');
        navToggle.setAttribute('aria-expanded', String(isOpen));
    });

    nav.querySelectorAll('a').forEach(link =>
        link.addEventListener('click', () => {
            nav.classList.remove('is-open');
            navToggle.setAttribute('aria-expanded', 'false');
        }));
}
