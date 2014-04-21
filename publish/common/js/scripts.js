$(document).ready(function () {
    var hn = window.hn = window.hn || {};

    $("#mobile-menu-btn").on("click", function () {
		$("#mobile-nav").toggleClass("opened");
    });

	$("ul#mobile-nav-main > li > a.expandable").on("click", function(e) {
		if($(this).parent().has("ul")) {
			e.preventDefault();
			$(this).closest("li").addClass("animated fadeOut").delay(300);
		}
		if (!$(this).hasClass("open")) {
			//hide any open menus and remove all other classes
			$("ul#mobile-nav-main li ul").slideUp(250);
			$("ul#mobile-nav-main li a, ul#mobile-nav-main li").removeClass("open fadeOut").addClass("fadeIn");
			
			// open our new menu and add the open class
			$(this).next("ul").slideDown(250, function(){
				$(this).closest("li").addClass("open");
			});
			$(this).addClass("open");
			
		}
		
		else if($(this).hasClass("open")) {
			$(this).removeClass("open");
			$(this).next("ul").slideUp(250);
			$(this).closest("li").removeClass("open fadeOut").addClass("fadeIn");
		}
	});
    // $(this).next("ul").slideToggle(350);   One line solution to above.

	var newsletterModal = $('#modal-signup');

    // function to hide modal after completing form
	hn.NewsletterSignUpComplete = function () {
	    $('.newsletter-container').html('Thank you for signing up, your information has been submitted.');
	    setTimeout(function () {
	        newsletterModal.modal('hide');
	    }, 2000);
	};

    // check for cookie. show modal and set cookie if not found
	var cookieName = 'newsletterSignUpCookie';
	if ($.cookie(cookieName) == null) {
	    newsletterModal.modal();
	    $.cookie(cookieName, 1, { expires: 180 });
	}

    // show modal on click in utility nav
	$('#newsletter-modal-toggle').on('click', function (e) {
	    e.preventDefault();
	    newsletterModal.modal('show');
	});

});

