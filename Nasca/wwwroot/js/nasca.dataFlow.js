/**
 * 
 */

nasca.dataFlow = nasca.dataFlow || {};

$(function(){
	nasca.dataFlow = (function(){
		var force;
		var svg;
		var nodes = [];    //ノードを収める配列
		var links = [];    //ノード間のリンク情報を収める配列
		var groups = [];
		var comments = [];
		var colors = [
		      		[255,255,0], [0,255,0], [0,0,255], [255,200,0],
		      		[150,80,90], [0,64,255],　[100,255,100],　[64,160,255],
		      		[255,0,64],　[160,32,32],　[196,128,64],　[128,196,64],
		      		[64,128,196],　[64,196,128],　[96,96,96], [0,0,0],
		      		[245,245,245]	//半透明リンク用
		];
		var addingLink = {};
		
		//初期化処理
		(function(){
			var linearGradient;
			
			force = d3.layout.force()
				.size([nasca.frame.wMain(), nasca.frame.hMain()])
				.nodes(nodes)
				.links(links)
				.linkDistance(150)
				.linkStrength(0.8)	//0..1
				.charge(-8000)
				.gravity(0.3)
                .friction(0.7);
			
			svg = d3.select("body")
				.append("svg")
				.attr("id", "drawingPaper")
				.attr("width", nasca.frame.wWindow())
				.attr("height", nasca.frame.hWindow())
		        .attr("preserveAspectRatio", "xMidYMid meet")
				.attr("pointer-events", "all")
				.append("g")
				.call(d3.behavior.zoom().scaleExtent([0.1, 5]).on("zoom", function(){
					svg.attr("transform", "translate(" + d3.event.translate + ")scale(" + d3.event.scale + ")");
				}))
				.on("dblclick.zoom", null)
				.append("g")
				.attr("id", "test");
			
			//領域全体をドラッグアンドドロップするためのプレースホルダ
			svg.append("rect")
				.attr("id", "background")
//				.attr("width",nasca.frame.wMain())
//				.attr("height",nasca.frame.hMain())
				.attr("width",5000)
				.attr("height",4000)
				.attr("fill","transparent");

			//矢印定義（終端）
			svg.append("svg:defs").selectAll("marker")
			    .data(colors)      // Different link/path types can be defined here
			    .enter().append("svg:marker")    // This section adds in the arrows
			    .attr("id", function(d,i){
			    	return "marker-end-" + i;
			    })
			    .attr("viewBox", "0 -5 10 10")
			    .attr("refX", 10)	//矢印の位置
			    .attr("refY", 0.0)	//矢印の位置
			    .attr("markerWidth", 6)
			    .attr("markerHeight", 6)
			    .attr("orient", "auto")
			    .append("svg:path")	//矢印の形
			    .attr("d", "M0,-5L10,0L0,5")	//矢印の形
				.attr("fill", function(d){
					return "rgb(" + d[0] + "," + d[1] + "," + d[2] + ")";
				});
			
			//矢印定義（始端）
			svg.append("svg:defs").selectAll("marker")
			    .data(colors)      // Different link/path types can be defined here
			    .enter().append("svg:marker")    // This section adds in the arrows
			    .attr("id", function(d,i){
			    	return "marker-start-" + i;
			    })
			    .attr("viewBox", "0 -5 10 10")
			    .attr("refX", 0)	//矢印の位置
			    .attr("refY", 0)	//矢印の位置
			    .attr("markerWidth", 6)
			    .attr("markerHeight", 6)
			    .attr("orient", "auto")
			    .append("svg:path")	//矢印の形
			    .attr("d", "M10,-5L0,0L10,5")	//矢印の形
				.attr("fill", function(d){
					return "rgb(" + d[0] + "," + d[1] + "," + d[2] + ")";
				});
			
			
			svg.append("svg:g").attr("class", "hulls");
			svg.append("svg:g").attr("class", "images");
			svg.append("svg:g").attr("class", "paths");
			
			//Windowへのイベント追加
			var w = d3
				.select(window)
				.on("mousemove", function(){
					if(!addingLink.state) return;
					
					//リンク追加のガイド表示
					addingLink.guide.attr("d", "M" + addingLink.source.x + "," + addingLink.source.y + " " + d3.event.offsetX + "," + d3.event.offsetY);
				})
				.on("click", function(){
					if(addingLink.state){
						addingLink.state = false;
						addingLink.guide.remove();
					}
				});
		})();
		
		var draw = function(json){
			//d3にバインドされているデータを更新します。
			updateData(json);
			
			//リンク描画
			var link = svg.select("g.paths").selectAll("path").data(links, function(d,i){return d.source.id + '-' + d.target.id;});
			setLinkStyleAndAttribute(setLinkStyleAndAttribute(link).enter().append("svg:path").attr("id", function(d,i){return d.source.id + '-' + d.target.id;}));
			link
				.on('contextmenu', d3.contextMenu(menu2, {
					onOpen: function() {},
					onClose: function() {}
				}))
				.on("mouseover", function(d){
					$("#" + nasca.utility.escapePeriod(d.source.id) + '-' + nasca.utility.escapePeriod(d.target.id)).css("stroke-width", "3.0px");
				})
				.on("mouseleave", function(d){
					$("#" + nasca.utility.escapePeriod(d.source.id) + '-' + nasca.utility.escapePeriod(d.target.id)).css("stroke-width", "2.0px");
				});
			link.exit().remove();
			
			//ノード描画
			var img = svg.select("g.images").selectAll("image").data(nodes, function(d,i){return d.id;});
			img
                .attr("xlink:href", function (d) {
                    if (d.visible) return "./images/" + d["svgFile"]; else return null;
                }) //ノード用画像の設定
				.attr("width", function(d){if(d.visible) return d.size; else return "0px";})
				.attr("height", function(d){if(d.visible) return d.size; else return "0px";})
				.enter().append("image")
				.attr("id", function(d){return "node-" + d.id.replace(/\./g, "_");})
				.attr("xlink:href", function(d){if(d.visible) return "./images/" + d["svgFile"]; else return null;}) //ノード用画像の設定
				.attr("width", function(d){if(d.visible) return d.size; else return "0px";})
				.attr("height", function(d){if(d.visible) return d.size; else return "0px";})
				.attr("data-depth", function(d){return d.depth;})
				.on("mousedown", function(d){
					d3.event.stopPropagation();
				})
				.on("click", function(d){
					if (d3.event.defaultPrevented) return;
					d3.event.stopPropagation();
					
					if(addingLink.state){
						nasca.utility.showModal(
							generateHtmlLinkRegister(null),
							function(){
								nasca.utility.ajaxPost(
									"LinkRegister/insert",
									{
										//"source": addingLink.source,
          //                              "target": d,
                                        "source": addingLink.source.id,
                                        "target": d.id,
										"dependencyTypeC": $("[name=CRUDC]").prop("checked"),
										"dependencyTypeR": $("[name=CRUDR]").prop("checked"),
										"dependencyTypeU": $("[name=CRUDU]").prop("checked"),
										"dependencyTypeD": $("[name=CRUDD]").prop("checked"),
										"remark": $("[name=remark]").val()
									},
									nasca.nodeTree.refresh
								);
							},
							null
						);
						
						addingLink.state = false;
						addingLink.guide.remove();
					}else{
						nasca.nodeTree.select(d.id);
					}
				})
				.on('contextmenu', d3.contextMenu(menu, {
					onOpen: function() {},
					onClose: function() {}
				}))
				.on("mousemove", function(d){
					if(addingLink.state){
						d3.event.stopPropagation();
						addingLink.guide.attr("d", "M" + addingLink.source.x + "," + addingLink.source.y + " " + d.x + "," + d.y);
					}
				})
				.call(force.drag);
			img.exit().remove();
			
			img.order();
						
			//convex hull
            var hulls = svg.select("g.images").selectAll("path.hull").data(groups, function (d, i) { return d.key; });
            hulls
                .style("stroke-width", function(d){
                return 20 + 15 * d.level;
	               })		    
	               .enter().append("path")
	               .attr("data-key", function(d){return d.key;})
	               .attr("data-depth", function(d){return d.depth;})
	            .attr("class", "hull")
	            .style("fill", function(d){
	                var brightness = 255 - d.depth * 10;
	                return "rgb(" + brightness + "," + brightness + "," + brightness + ")";
	            })
	            .style("stroke", function(d){
	                var brightness = 255 - d.depth * 10;
	                return "rgb(" + brightness + "," + brightness + "," + brightness + ")";
	            })
	            .style("stroke-width", function(d){
	                return 20 + 15 * d.level;
	            })
	            .style("stroke-linejoin", "round")
	            .style("opacity", 0.5)
	            .on('contextmenu', d3.contextMenu(menuHull, {
	                onOpen: function() {},
	                onClose: function() {}
	            }));
            hulls.exit().remove();

            //Reorder hull because images are under hull.
	        var removedHulls = svg.select("g.images").selectAll("path.hull").remove();
	        for (var i=0; i<removedHulls[0].length; i++){
	        	for (var j=0; j<img[0].length; j++){
	        		if($(removedHulls[0][i]).data("depth") <= $(img[0][j]).data("depth")){
	        			$(removedHulls[0][i]).insertBefore(img[0][j]);
	        			break;
	        		}
	        	}
	        }
			
			//オブジェクト名称描画
			var text = svg.selectAll("text.nodeName").data(nodes, function(d,i){return d.id;});
			text
				.text(function(d){if(d.visible) return d.name; else return null;})
				.enter().append("text")
				.attr("class", "nodeName")
			    .text(function(d){if(d.visible) return d.name; else return null;})
			    .attr("font-family", "Noto Sans Japanese")
			    .attr("font-size", "14px")
			    .attr("fill", "darkgray");
			text.exit().remove();
			
			force.on("tick", function(e) {
				//クラスター化				
				nodes.forEach(cluster(e.alpha));
				
				//convex hull
			    hulls.attr("d", groupPath);
			    
//			    //衝突判定
//			    hulls[0].forEach(function(hull){
//			    	var vertexesEx = [];
//			    	//hullの頂点座標はDOMにしか存在しないのでjQueryを使わざるを得ない。pathタグのd属性を取得。
//			    	var vertexes = $(hull).attr("d").split("L");
//			    	var splittedText;
//			    	
//			    	//先頭の頂点から"M"を削除
//			    	splittedText = vertexes[0];
//			    	vertexes[0] = splittedText.substr(1);
//			    	
//			    	//末尾の頂点から"Z"を削除
//			    	splittedText = vertexes[vertexes.length - 1];
//			    	vertexes[vertexes.length - 1] = splittedText.substr(0, splittedText.length - 1);
//			    	
//			    	//文字列をX座標、Y座標に変換
//			    	vertexes.forEach(function(vertex, index){
//			    		var xy = vertex.split(",");
//			    		vertexes[index] = {"x":Number(xy[0]), "y":Number(xy[1])};
//			    		
////			    		var nextIndex = vertexes.length <= index + 1 ? 0 : index + 1; 
////			    		var xy1 = vertexes[index].split(",");
////			    		var xy2 = vertexes[nextIndex].split(",");
////
////			    		var shiftedLine = shiftParallel(Number(xy1[0]), Number(xy1[1]), Number(xy2[0]), Number(xy2[1]), 100, 0);
////			    		vertexesEx.push({"x":shiftedLine.x1, "y":shiftedLine.y1});
////			    		vertexesEx.push({"x":shiftedLine.x2, "y":shiftedLine.y2});
//			    	});
//			    	
//			    	//衝突判定
//			    	nodes.forEach(function(node){
//			    		var pointWork, pointNearest;
//			    		var dx, dy;
//			    		//グループに含まれないノードの場合
//						var str = " " + node.parent;
//						if(node.parent !== $(hull).data("key") && str.indexOf(" " + $(hull).data("key")) === -1) {
//							//衝突判定を行った結果衝突していたら
//							if(judgeInclusion(node, vertexes)){
//								//hullの各辺までの距離を求めます
//								vertexes.forEach(function(vertex, index){
//									var adjacentIndex = index === vertexes.length - 1 ? 0 : index + 1;
//									pointWork = calculationDistance(node.x, node.y, vertexes[index].x, vertexes[index].y, vertexes[adjacentIndex].x, vertexes[adjacentIndex].y);
//									if(pointNearest === undefined || pointWork.d < pointNearest.d){
//										pointNearest = pointWork;
//									}
//								});
//								
//								dx = node.x - pointNearest.x;
//								dy = node.y - pointNearest.y;
//								
//								node.x -= dx * e.alpha;
//								node.y -= dy * e.alpha;
////								node.collision = true;
//				    		}else{
////				    			node.collision = false;
//				    		}
//						}
//			    	});
//			    });
			    
				link.attr("d", function(d) {
					var sr,tr,l,dx,dy,sx,sy,tx,ty,margin;
					margin = 8;
					sr = (d.source.size + margin) / 2;
					tr = (d.target.size + margin) / 2;
					dx = d.target.x - d.source.x;
					dy = d.target.y - d.source.y;
					l = Math.sqrt(dx * dx + dy * dy);
					sx = d.source.x + dx * sr / l;
					sy = d.source.y + dy * sr / l;
					tx = d.target.x - dx * tr / l;
					ty = d.target.y - dy * tr / l;
					
					//2段階目の点線描画のため「ソースノードが非表示」かつ「ターゲットノードが表示」の場合のみ線の向きを反転します。
					if(!d.source.visible && d.target.visible){
						return "M" + tx + "," + ty + " " + sx + "," + sy;
					}else{
						return "M" + sx + "," + sy + " " + tx + "," + ty;
					}
			    });
				
				img
					.attr('x', function(d) { return d.x - d.size / 2; })
					.attr('y', function(d) { return d.y - d.size / 2; });

				text
				    .attr('x', function(d) { return d.x - d.size / 2; })
				    .attr('y', function(d) { return d.y + d.size / 2 + 12; });
			});	
	
			force.start();
		};
		
		var updateData = function(json){
			var i,j;
			var countJsonNodes, countJsonLinks, countDataNodes, countDataLinks;
			var isExists;
			var target,source;
			var additionalGroups = [];
	
			//ノードを追加・削除
			//変更のないデータを削除⇒追加するとD3への束縛に不具合が発生するので変更分のみ追加・削除を行う
			countJsonNodes = json["nodes"].length;
			for(i=0; i<countJsonNodes; i++){
				countDataNodes = nodes.length;
				isExists = false;
				for(j=0; j<countDataNodes; j++){
					//D3データに既に存在する場合はプロパティを更新します（nodeごと入れ替えると不具合が発生するため必要なプロパティを明示します）
					if(json["nodes"][i].id === nodes[j].id){
                        isExists = true;
						nodes[j].visible = json["nodes"][i].visible;
						break;
					}
				}
				//D3データに存在しない場合は追加します
				if(!isExists){
					nodes.push(json["nodes"][i]);
				}
			}
			countDataNodes = nodes.length;
			for(i=countDataNodes-1; 0<=i; i--){
				countJsonNodes = json["nodes"].length;
				isExists = false;
				for(j=0; j<countJsonNodes; j++){
					if(nodes[i].id === json["nodes"][j].id){
                        isExists = true;
						break;
					}
				}
				//JSONに存在しなければD3データを削除します
				if(!isExists){
					nodes.splice(i, 1);
				}
			}
			
			nodes.sort(function(a,b){
		        if( a.depth < b.depth ) return -1;
		        if( a.depth > b.depth ) return 1;
		        return 0;
			});
			
			//リンクを追加・削除
			//変更のないデータを削除⇒追加するとD3への束縛に不具合が発生するので変更分のみ追加・削除を行う
			countJsonLinks = json["links"].length;
			for(i=0; i<countJsonLinks ; i++){
				countDataLinks = links.length;
				isExists = false;
				for(j=0; j<countDataLinks; j++){
					//D3データに既に存在する場合はプロパティを更新します（linkごと入れ替えると不具合が発生するため必要なプロパティを明示します）
					if(json["links"][i].source === links[j].source.id && json["links"][i].target === links[j].target.id){
                        isExists = true;
						links[j].isCreate = json["links"][i].isCreate;
						links[j].isRead = json["links"][i].isRead;
						links[j].isUpdate = json["links"][i].isUpdate;
						links[j].isDelete = json["links"][i].isDelete;
						links[j].remark = json["links"][i].remark;
						links[j].io = json["links"][i].io;
						links[j].colorIndex = json["links"][i].colorIndex;
						links[j].visible = json["links"][i].visible;
						links[j].virtual = json["links"][i].virtual;
						break;
					}
				}
				//D3データに存在しない場合は追加します
				if(!isExists){
					//ID文字列だと不具合が発生するのでノードオブジェクトへの参照を取得します。
					target = nodes.filter(function(item, index){
						if (item.id === json["links"][i]["target"]) return true;
					});
					source = nodes.filter(function(item, index){
						if (item.id === json["links"][i]["source"]) return true;
					});
					
					links.push({
						source: source[0],
						target: target[0],
						isCreate: json["links"][i].isCreate,
						isRead: json["links"][i].isRead,
						isUpdate: json["links"][i].isUpdate,
						isDelete: json["links"][i].isDelete,
						remark: json["links"][i].remark,
						io: json["links"][i].io,
						colorIndex: json["links"][i].colorIndex,
						visible: json["links"][i].visible,
						virtual: json["links"][i].virtual
					});
				}
			}
			countDataLinks = links.length;
			for(i=countDataLinks-1; 0<=i ; i--){				
				countJsonLinks = json["links"].length;
				isExists = false;
				for(j=0; j<countJsonLinks; j++){
					if(links[i].source.id === json.links[j].source && links[i].target.id === json.links[j].target){
						isExists = true
						break;
					}
				}
				//JSONに存在しなければD3データを削除します
				if(!isExists){
					links.splice(i, 1);
				}
			}
			
			//hull描画のため、親ノードでグルーピングします。
			groups = d3.nest().key(function(d) { return d.parent; }).entries(nodes.filter(function(element, index, array) {
				if(element.parent === "root" || !element.visible){
					return false;
				} else{
					return true;
				}
			}));
			
			//ノードが存在しない上位グループを追加します
			groups.forEach(function(val1,index1,ar1){
				var k;
				var nameSpace = "";
				var nameSpaceTemp = "";
				var names = val1.key.split(".");
				
				if(names.length === 1){
					return false;
				}
				
                names.forEach(function (name, indexName, arName) {
                    var isExists = false;
                    nameSpaceTemp = nameSpaceTemp + "." + name;
                    nameSpace = nameSpaceTemp.substr(1);

                    groups.forEach(function (val2, index2, ar2) {
                        if (val2.key === nameSpace) {
                            isExists = true;
                            return false;
                        }
                    });
                    if (isExists) {
                        return false;
                    }

                    additionalGroups.forEach(function (val2, index2, ar2) {
                        if (val2.key === nameSpace) {
                            isExists = true;
                            return false;
                        }
                    });
                    if (isExists) {
                        return false;
                    }

                    additionalGroups.push({ "key": nameSpace, "values": [] });
                });
			});
			Array.prototype.push.apply(groups, additionalGroups);
			
			//配下の階層のノードを自分のレベルにも含めます
			groups.forEach(function(val1,index1,ar1){
				//深い階層＜浅い階層となるようなlevelを設定します。
				var depth1,depth2,depthMax;
				depth1 = nasca.utility.countString(val1.key, nasca.utility.escapePeriod("."));
				depthMax = depth1;
				
				groups.forEach(function(val2,index2,ar2){
					//前方一致検索
					var str = " " + val2.key;
					if (val1.key !== val2.key && str.indexOf(" " + val1.key) !== -1) {
						Array.prototype.push.apply(val1.values, val2.values);
						depth2 = nasca.utility.countString(val2.key, nasca.utility.escapePeriod("."));
						if(depthMax < depth2){
							depthMax = depth2;
						}
					}
				});
				
				val1.level = depthMax - depth1;
				val1.depth = depth1 + 1;
			});
		};
		
		var setLinkStyleAndAttribute = function(selection){
			selection
		    	.style("fill", "none")
		    	.style("stroke-width", "2.0px")
		    	.style("stroke", function(d){
		    		if(d.visible){
		    			return "rgb(" + colors[d.colorIndex][0] + "," + colors[d.colorIndex][1] + "," + colors[d.colorIndex][2] + ")";
		    		}else{
		    			return "rgb(" + colors[16][0] + "," + colors[16][1] + "," + colors[16][2] + ")";
		    		}
		    	})
		    	.style("stroke-dasharray", function(d){
		    		if(d.visible || (d.source.visible && d.target.visible)){
		    			return "0";
		    		}else{
		    			return "40,1,4,2,3,3,2,4,1,9999";
		    		}
		    	})
		    	.attr("marker-start", function(d){
		    		if(!d.source.visible && d.target.visible){
		    			return getMarkerDefinition(d, true, true);
		    		} else{
		    			return getMarkerDefinition(d, false, true);
		    		}
		    	})
		    	.attr("marker-end", function(d){
		    		if(!d.source.visible && d.target.visible){
		    			return getMarkerDefinition(d, true, false);
		    		} else{
		    			return getMarkerDefinition(d, false, false);
		    		}
		    	});
			
			return selection;
		};
		
		var getMarkerDefinition = function(line, isReverse, isStart){
			var result;
			
			//着目するべきノード。
			//2段階目の点線描画のため線の向きを反転させている場合があるため着目すのはどちらかを設定します。
			var subjectNode = isReverse ? (isStart ? line.target : line.source) : (isStart ? line.source : line.target);
			
			//マーク（矢印）を表示するかどうかを示します。
			//着目するべきノードが可視かつ、IOがあればマークを表示します。
			var marked = subjectNode.visible && (line.io === "IO" || line.io === (isReverse ? (isStart ? "O" : "I") : (isStart ? "I" : "O")))

			if(marked){
				if(isStart){
					if(line.visible){
						result = "#marker-start-" + line.colorIndex;
	    			}else{
	    				result = "#marker-start-16";
	    			}
				}else{
					if(line.visible){
						result = "#marker-end-" + line.colorIndex;
	    			}else{
	    				result = "#marker-end-16";
	    			}
				}
				result = "url(" + result + ")";
			}else{
				result = null;
			}
			
			return result;
		};
		
		var generateHtmlLinkRegister = function(link){
			var checkedCreate, checkedRead, checkedUpdate, checkedDelete;
			
			checkedCreate = (link && link.isCreate) ? ('checked="checked"') :('');
			checkedRead = (link && link.isRead) ? ('checked="checked"') :('');
			checkedUpdate = (link && link.isUpdate) ? ('checked="checked"') :('');
			checkedDelete = (link && link.isDelete) ? ('checked="checked"') :('');

			var html = 
				'<input type="checkbox" name="CRUDC" value="C" ' + checkedCreate + '>Create<br>' +
				'<input type="checkbox" name="CRUDR" value="R" ' + checkedRead + '>Read<br>' +
				'<input type="checkbox" name="CRUDU" value="U" ' + checkedUpdate + '>Update<br>' +
				'<input type="checkbox" name="CRUDD" value="D" ' + checkedDelete + '>Delete<br>' +
				'<textarea name="remark" rows="4" cols="40">input remark</textarea><br>';
			
			return html;
		};
		
        var menu = function (data) {
            return [
                {
                    title: 'Expand',
                    action: function (elm, d, i) {
                        nasca.nodeTree.selectChild(d.id);
                    },
                    disabled: !data.group
                },
                {
                    title: 'Add link',
                    action: function (elm, d, i) {
                        d3.event.stopPropagation();

                        addingLink.state = true;
                        addingLink.source = d;
                        addingLink.guide = svg.select("g").append("path").style("stroke", "black");
                    },
                    disabled: data.group
                }
            ];
        };
		
		var menu2 = function(data) {
			return [
				{
					title: 'Edit',
					action: function(elm, d, i) {
						nasca.utility.showModal(
							generateHtmlLinkRegister(d),
							function(){
								nasca.utility.ajaxPost(
									"LinkRegister/update",
									{
										"source": d.source.id,
										"target": d.target.id,
										"dependencyTypeC": $("[name=CRUDC]").prop("checked"),
										"dependencyTypeR": $("[name=CRUDR]").prop("checked"),
										"dependencyTypeU": $("[name=CRUDU]").prop("checked"),
										"dependencyTypeD": $("[name=CRUDD]").prop("checked"),
										"remark": $("[name=remark]").val()
									},
									nasca.nodeTree.refresh
								);
							},
							null
						);
					},
					disabled : data.virtual || !data.visible
				},
				{
					title: 'Remove',
					action: function(elm, d, i) {
						nasca.utility.showModal(
							'Are you sure to remove?',
							function(){
								nasca.utility.ajaxPost(
									"LinkRegister/delete",
									{
										"source": d.source.id,
										"target": d.target.id
									},
									nasca.nodeTree.refresh
								);
							},
							null
						);
					},
					disabled : data.virtual || !data.visible
				}
			];
		}
		
		var menuHull = function(data) {
			return [
				{
					title: 'Collapse',
					action: function(elm, d, i) {
						nasca.nodeTree.select(d.key);
					}
				}
			];
		};
		
		//pathタグのd属性に設定する文字列を作成します
		var groupPath = function(d) {
			var prefix = "M";
			var splitter = "L";
			var safix = "Z";
			var vertex = [];
			var radius;
			
			//各ノードの4隅の座標を利用します
			if(d.values === undefined){
				if(d.visible === true){
					radius = d.size / 2;
					vertex.push([d.x - radius, d.y - radius]);
					vertex.push([d.x + radius, d.y - radius]);
					vertex.push([d.x - radius, d.y + radius]);
					vertex.push([d.x + radius, d.y + radius]);
				}
			}else{
				d.values.forEach(function(node){
					if(node.visible === true){
						radius = node.size / 2;
						vertex.push([node.x - radius, node.y - radius]);
						vertex.push([node.x + radius, node.y - radius]);
						vertex.push([node.x - radius, node.y + radius]);
						vertex.push([node.x + radius, node.y + radius]);
					}
				});
			}
			return prefix + d3.geom.hull(vertex).join(splitter) + safix;
		};
		
		//****************************************************************************************
		// Move d to be adjacent to the cluster node.
        var cluster = function (alpha) {
            return function (d) {
                var vx = 0;
                var vy = 0;
                var count = 0;
                var averageX = 0;
                var averageY = 0;
                var names = d.id.split(".");

                //同一グループの座標の総和を求めます
                nodes.forEach(function (node) {
                    var names2 = node.id.split(".");
                    var commonLength, distance;
                    if (names[0] === names2[0]) {
                        commonLength = nasca.utility.getCommonString(d.id, node.id).split(".").length;;
                        distance = (names.length - commonLength) + (names2.length - commonLength) - 1;

                        vx = vx + (node.x - d.x) / distance;
                        vy = vy + (node.y - d.y) / distance;
                        count = count + 1;
                    }
                });

                if (count === 0) return;

                //平均ベクトル
                averageX = vx / count;
                averageY = vy / count;

                d.x += averageX * alpha;
                d.y += averageY * alpha;
            };
        };
		
		var judgeInclusion = function(p1, comparisonArr) {
		  var deg = 0;
		  var p1x = p1.x;
		  var p1y = p1.y;

		  for (var index = 0; index < comparisonArr.length; index++) {
		    var p2x = comparisonArr[index].x;
		    var p2y = comparisonArr[index].y;
		    if (index < comparisonArr.length - 1) {
		      var p3x = comparisonArr[index + 1].x;
		      var p3y = comparisonArr[index + 1].y;
		    } else {
		      var p3x = comparisonArr[0].x;
		      var p3y = comparisonArr[0].y;
		    }

		    var ax = p2x - p1x;
		    var ay = p2y - p1y;
		    var bx = p3x - p1x;
		    var by = p3y - p1y;

		    var cos = (ax * bx + ay * by) / (Math.sqrt(ax * ax + ay * ay) * Math.sqrt(bx * bx + by * by));
		    deg += getDegree(Math.acos(cos));
		  }

		  if (Math.round(deg) == 360) {
		    return true;
		  } else {
		    return false;
		  }
		};
		
		var calculationDistance = function(x0, y0, x1, y1, x2, y2) {
			var ax, ay, bx, by, r, dx, dy, result = {};
			
			ax = x2 - x1;
			ay = y2 - y1;
			bx = x0 - x1;
			by = y0 - y1;
			
			r = (ax*bx + ay*by) / (ax*ax + ay*ay);
			
			if( r<= 0 ){
				result.x = x1;
				result.y = y1;
			}else if( r>=1 ){
				result.x = x2;
				result.y = y2;
			}else{
				result.x = x1 + r * ax;
				result.y = y1 + r * ay;
			}
			
			dx = x0 - result.x;
			dy = y0 - result.y;
			result.d = Math.sqrt(dx * dx + dy * dy);
			
			return result;
		};
		
		var shiftParallel = function(x1, y1, x2, y2, distance, direction){
			var ax, ay, bx, by, cx, cy, dx , dy, norm;
			
			ax = x2 - x1;
			ay = y2 - y1;
			
			if(direction === 0){
				bx = ay;
				by = -1 * ax;
			}else{
				bx = -1 * ay;
				by = ax;
			}
			norm = Math.sqrt(ax * ax + ay * ay);
			cx = bx / norm * distance;
			cy = by / norm * distance;
			dx = cx + ax;
			dy = cy + ay;
			
			return {"x1":cx + x1, "y1":cy + y1, "x2":dx + x1, "y2":dy + y1};
		};
		
		var getDegree = function(radian) {
			  return radian / Math.PI * 180;
			};
		
		return{
			draw : draw
		};
	})();
});
