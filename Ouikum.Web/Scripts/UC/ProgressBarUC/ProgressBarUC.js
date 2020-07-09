
function drawProgressBar(percent) {    
	var pixels = 296 * (percent / 100);
	var position_text = percent - 4;
	percent = percent.toFixed(0);	
	document.write('<div class="progress_wrapper">');  
	document.write('<div class="progress_bar" style="width:'+percent+'%;"></div>');  
	document.write('<div class="progress_text" >' + percent + '%</div>');  
	document.write('</div>');
}  
