//  Andy Langton's show/hide/mini-accordion @ http://andylangton.co.uk/jquery-show-hide

// this tells jquery to run the function below once the DOM is ready
$(function() {

// choose text for the show/hide link - can contain HTML (e.g. an image)
var showText='Show';
var hideText='Hide';

// initialise the visibility check
var is_visible = false;

// append show/hide links to the element directly preceding the element with a class of "toggle"
$('.toggle').prev().append(' <a href="#" class="toggleLink">'+hideText+'</a>');

// hide all of the elements with a class of 'toggle'
$('.toggle').show();

// capture clicks on the toggle links
$('a.toggleLink').click(function() {

// switch visibility
is_visible = !is_visible;

// change the link text depending on whether the element is shown or hidden
if ($(this).text()==showText) {
$(this).text(hideText);
$(this).parent().next('.toggle').slideDown();
}
else {
$(this).text(showText);
$(this).parent().next('.toggle').slideUp();
}

// return false so any link destination is not followed
return false;

});

});

function ShowMenuSidebar(isOpen, obj) {
    if (isOpen) {
        obj.text('Hide');
        obj.parent().next('.toggle').show();
    }
    else {
        obj.text('Show');
        obj.parent().next('.toggle').hide();
    }
}