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
});