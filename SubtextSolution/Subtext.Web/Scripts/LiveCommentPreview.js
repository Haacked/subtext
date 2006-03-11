/*
Adds Live Comment Preview for comment forms.

USAGE: Simply add the css class "livepreview" to the textarea that 
is the source of the comment and to the <div> or <p> element that 
will display the live preview.

Note that livepreview does not have to be the only css class.

ex... Make this edit in PostComment.ascx

<asp:TextBox id="tbComment" runat="server" Rows="10" Columns="40" width="100%" Height="193px"
					TextMode="MultiLine" class="livepreview"></asp:TextBox>
					
<p class="comment livepreview"></p>

*/

function initLiveCommentPreview()
{
	if (!document.getElementsByTagName) { return; }

	var paras = document.getElementsByTagName('p');
	var previewElement = getPreviewDisplayElement(paras);
	if(!previewElement)
	{
		var divs = document.getElementsByTagName('div');
		previewElement = getPreviewDisplayElement(divs);
	}
	
	if(!previewElement) {return;}	
	var textareas = document.getElementsByTagName('textarea');

	// loop through all input tags
	for (var i = 0; i < textareas.length; i++)
	{
		var textarea = textareas[i];
		if (getClassName(textarea).indexOf('livepreview') >= 0)
		{
			textarea.onkeyup = function () {reloadPreview(this, previewElement); return false;}
		}
	}
}

// Returns the html element responsible for previewing 
// comments.
function getPreviewDisplayElement(elements)
{
	// loop through all elements
	for (var i = 0; i < elements.length; i++)
	{
		var element = elements[i];

		if (getClassName(element).indexOf('livepreview') >= 0)
		{
			return element;
		}
	}
}

function getClassName(element)
{
	if(element.getAttribute && element.getAttribute('class'))
	{
		return element.getAttribute('class');
	}
	else if(element.className)
	{
		return element.className;
	}
	return "";
}

function reloadPreview(textarea, previewDisplay) 
{
	var previewString = textarea.value;
	if (previewString.length > 0)
	{
		previewString = htmlUnencode(previewString);
		previewString = previewString.replace(new RegExp("(.*)\n\n([^#\*\n\n].*)","g"), "<p>$1</p><p>$2</p>");
		previewString = previewString.replace(new RegExp("(.*)\n([^#\*\n].*)","g"), "$1<br />$2");
	}
	previewDisplay.innerHTML = previewString;
}

function htmlUnencode(s)
{
	return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

//
// addLoadEvent()
// Adds event to window.onload without overwriting currently assigned onload functions.
// Function found at Simon Willison's weblog - http://simon.incutio.com/
//
function addLoadEvent(func)
{	
	var oldonload = window.onload;
	if (typeof window.onload != 'function'){
    	window.onload = func;
	} else {
		window.onload = function(){
		oldonload();
		func();
		}
	}

}

addLoadEvent(initLiveCommentPreview);	// run initLightbox onLoad