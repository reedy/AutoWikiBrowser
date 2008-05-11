// From http://blog.movalog.com/a/javascript-toggle-visibility/
function toggle_visibility(id) {
var e = document.getElementById(id);
alert (e.display);
if(e.style.display == 'none')
  e.style.display = 'block';
else
  e.style.display = 'none';
}
