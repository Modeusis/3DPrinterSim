document.addEventListener('DOMContentLoaded', () => {
    const burger = document.querySelector('.burger');
    const nav = document.querySelector('nav');

    if (burger) {
        burger.addEventListener('click', () => {
            nav.classList.toggle('active');
        });
    }

    const enlargeableImgs = document.querySelectorAll('.enlargeable');
    enlargeableImgs.forEach(img => {
        img.addEventListener('click', function() {
            this.classList.toggle('enlarged');
        });
        
        img.addEventListener('mouseleave', function() {
            this.classList.remove('enlarged');
        });
    });

    const textReplacers = document.querySelectorAll('.text-replacer');
    textReplacers.forEach(replacer => {
        replacer.addEventListener('click', function() {
            this.classList.add('active');
        });
        
        replacer.addEventListener('mouseleave', function() {
            this.classList.remove('active');
        });
    });

    const partLinks = document.querySelectorAll('.part-link');
    const targetPartImage = document.getElementById('target-part-image');
    
    if (partLinks.length > 0 && targetPartImage) {
        partLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const newImageSrc = this.getAttribute('data-img');
                targetPartImage.src = newImageSrc;
            });
        });
    }

    const tocNav = document.querySelector('.toc-nav');
    if (tocNav) {
        const tocTitle = tocNav.querySelector('h3');
        if (tocTitle) {
            tocTitle.addEventListener('click', () => {
                tocNav.classList.toggle('collapsed');
            });
        }

        const sections = document.querySelectorAll('h2[id]');
        const tocLinks = document.querySelectorAll('.toc-list a');

        function updateActiveSection() {
            let currentSection = '';
            let currentSectionText = '';
            const scrollPosition = window.scrollY + 150;

            sections.forEach(section => {
                const sectionTop = section.offsetTop;
                const sectionHeight = section.offsetHeight;
                
                if (scrollPosition >= sectionTop && scrollPosition < sectionTop + sectionHeight + 200) {
                    currentSection = section.getAttribute('id');
                    currentSectionText = section.textContent;
                }
            });

            tocLinks.forEach(link => {
                link.classList.remove('active');
                if (link.getAttribute('href') === '#' + currentSection) {
                    link.classList.add('active');
                }
            });

            const tocCurrent = document.querySelector('.toc-current');
            if (tocCurrent && currentSectionText) {
                tocCurrent.textContent = currentSectionText;
            }
        }

        window.addEventListener('scroll', updateActiveSection);
        updateActiveSection();
    }
});