/**
 * 
 */

var nasca = nasca || {};
nasca.frame = nasca.frame || {};

$(function(){
	//画面のフレーム情報
	nasca.frame = (function(){
		var wWindow;
		var hWindow;
		var wNodeList;    
		var wMain;
		var hMain;
		var hHeader;
		
		var initialize = function(){
			wWindow = $(window).innerWidth();
			hWindow = $(window).innerHeight();
			wNodeList = $("#nodeList").width();    
			hHeader = $("#header").height();
//			wMain = wWindow - wNodeList -1;
			wMain = wWindow - wNodeList;
			hMain = hWindow - hHeader;
		};

		return {
			wWindow: function(){initialize(); return wWindow;},
			hWindow: function(){initialize(); return hWindow;},
			wNodeList: function(){initialize(); return wNodeList;},
			hHeader: function(){initialize(); return hHeader;},
			wMain: function(){initialize(); return wMain;},
			hMain: function(){initialize(); return hMain;}
		};
	})();
	
	//画面リサイズイベント登録
	$(window).resize(function(){
		$("#drawingPaper").attr("width", nasca.frame.wMain());
	});	
});
