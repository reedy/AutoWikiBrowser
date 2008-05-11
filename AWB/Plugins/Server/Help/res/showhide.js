function toggle_visibility(id) {
	var e = document.getElementById(id);
	// alert (e.style.display); // why is this null at first call despite me setting the property in the .css?
	if(e.style.display == 'none')
	  e.style.display = 'block';
	else
	  e.style.display = 'none';
}
