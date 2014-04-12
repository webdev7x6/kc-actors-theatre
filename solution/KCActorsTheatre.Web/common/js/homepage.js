$(function() {
  $(window).resize(function() {
     if (screen.width > 480) {
        setEqualHeight();
     }
  });
  
  $(window).load(function() {
     if (screen.width > 480) {
        setEqualHeight();
     }
  });
  
  
  
  //$(document).ready(setEqualHeight);
  //$(window).resize(setEqualHeight);
  //$(window).load(setEqualHeight);
  
  function setEqualHeight() {
    //console.log('Hi!');
    //alert('Running');
    var currentTallest = 0,
      currentRowStart = 0,
      rowDivs = new Array(),
      $el,
      topPosition = 0;
  
    $('.main-homepage-widget').each(function() {
      $el = $(this);
      topPostion = $el.position().top;
      if (currentRowStart != topPostion) {
  
        // we just came to a new row.  Set all the heights on the completed row
        for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
          rowDivs[currentDiv].height(currentTallest);
        }
        // set the variables for the new row
        rowDivs.length = 0; // empty the array
        currentRowStart = topPostion;
        currentTallest = $el.height();
        rowDivs.push($el);
      } else {
        // another div on the current row.  Add it to the list and check if it's taller
        rowDivs.push($el);
        currentTallest = (currentTallest < $el.height()) ? ($el.height()) : (currentTallest);
      }
      // do the last row
      for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
        rowDivs[currentDiv].height(currentTallest);
      }
    });
  }
  
  var $introStories = $('.intro-story'); //Gets an array of all html nodes with this class
  var curStory = 0;
  
  // Change Mission story...
  $("a.prev-story").on("click", function(){
    switchSlide(-1);
    return false;
  });
  
  $("a.next-story").on("click", function(){
    switchSlide(1);
    return false;
  });
  
  function switchSlide(dir) {
    $($introStories[curStory]).removeClass('xs-show').addClass('xs-hide'); //In intro stories, find the item with the value of curStory, remove its current class and add a different one.
    curStory += dir;
    if (curStory < 0){
      curStory = $introStories.length - 1;
    }
    else if (curStory > $introStories.length - 1){
      curStory = 0;
    }
    $($introStories[curStory]).removeClass('xs-hide').addClass('xs-show'); //Switching out classes for new one, after having modfiied curStory.
  }
});