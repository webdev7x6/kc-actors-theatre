$("#my-menu").mmenu({
  header: true,
  classes: "mm-light"
});
$(".clicka").click(function(){
  $("#my-menu").trigger("open");
})
$("#navig2").mmenu({
  searchfield: true,
  header:true,
  position: "right",
  classes: "mm-light"
});
$(".clicka2").click(function(){
  $("#navig2").trigger("open");
})