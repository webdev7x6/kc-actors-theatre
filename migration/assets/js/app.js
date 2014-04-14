$('.responsive').slick({
  dots: true,
  infinite: false,
  speed: 300,
  slidesToShow: 3,
  slidesToScroll: 3,
  responsive: [
    {
      breakpoint: 1024,
      settings: {
        slidesToShow: 2,
        slidesToScroll: 2,
        infinite: true,
        dots: true
      }
    },
    {
      breakpoint: 600,
      settings: {
        slidesToShow: 2,
        slidesToScroll: 2
      }
    },
    {
      breakpoint: 480,
      settings: {
        slidesToShow: 1,
        slidesToScroll: 1
      }
    }
  ]
});

//Handle animation with CSS
// ADD SLIDEDOWN ANIMATION TO DROPDOWN //
$('.dropdown').on('show.bs.dropdown', function(e){
  var screenWidth = $(window).width();
  var offset = $(this).offset().left;
  var navItemWidth = $(this).outerWidth();
  var right = screenWidth - offset - navItemWidth;
  $(this).find('.dropdown-menu li').css('padding-right', right);
  //$(this).find('.dropdown-menu').addClass('magictime slideDown');
});