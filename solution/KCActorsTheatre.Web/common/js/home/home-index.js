/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />
$().ready(function () {

    // event carousel
    $('.carousel').carousel();

    var currentSlideIndex = 0;

    // show first image and set class for caption
    $('.slide-image-container').hide();
    $('.slide-image-container[data-slide-index=' + currentSlideIndex + ']').show();
    $('.slide-caption-container[data-slide-index=0]').removeClass('hero-text');
    $('.slide-caption-container[data-slide-index=0]').addClass('hero-text-active');

    // change caption and image on click
    $('.slide-title').on('click', function (e) {
        e.preventDefault();

        var slideIndex = $(this).attr('data-slide-index');
        var captionContainer = $('.slide-caption-container[data-slide-index=' + slideIndex + ']');
        var imageContainer = $('.slide-image-container[data-slide-index=' + slideIndex + ']');

        // reset current slide image and caption
        $('.slide-caption-container[data-slide-index=' + currentSlideIndex + ']').removeClass('hero-text-active');
        $('.slide-caption-container[data-slide-index=' + currentSlideIndex + ']').addClass('hero-text');

        // set new slide active
        captionContainer.addClass('hero-text-active');
        captionContainer.removeClass('hero-text');

        // fade-out current image
        $('.slide-image-container[data-slide-index=' + currentSlideIndex + ']').fadeOut('normal', function () {
            // fade-in current image
            imageContainer.fadeIn();
            currentSlideIndex = slideIndex;
        });
    });
});