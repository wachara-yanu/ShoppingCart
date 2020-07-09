//  <style type="text/css">
//  div.progress_wrapper
//  {     
//	position: relative;       
//	background:url(percentage-bg.png) no-repeat;	
//	height: 39px; width:296px; z-index:91;
//  }

//  div.progress_bar
//  {        
//	position: absolute;
//	top: 0; left: 0;
//	height: 39px;
//	z-index:0;
//	/*background:green;*/
//	background:url(progressbar.png) no-repeat;
//	
//  }

//  div.progress_text
//  {             
//	position: relative;	
//	padding-top:30px;
//	font-size:11px;
//	color:#666;        
//  }
//</style>
  
  function drawProgressBar(percent)  
  {          
	var pixels = 296 * (percent / 100);
	var position_text = percent - 4;
	percent = percent.toFixed(0);	 
  
	document.write('<div class="progress_wrapper">');  
	document.write('<div class="progress_bar" style="width:'+percent+'%;"></div>');  
	document.write('<div class="progress_text" style="padding-left:'+position_text+'%;">' + percent + '%</div>');  
	document.write('</div>');
}  