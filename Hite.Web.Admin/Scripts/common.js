//对input上传图片
//obj：Input的ID
function uploadImgForInput(obj) {
    var dialog = new X.UI.Dialog({ width: '400px' });
    dialog.setHeader("增加图片");
    dialog.setBody('<table><tbody><tr><td><div><form><input id=\"img0\" type=\"file\" name=\"img0\"/>&nbsp;<input type=\"button\" class=\"input-button\" value=\"上 传\" name=\"startupimg\" id=\"startupimg\"><br><span> 图片的大小不超过3M,只允许jpg,jpeg,gif,bmp,png</span><br/><br/></form></div><div id=\"loading\" style=\"display:none;\"></div></td></tr></tbody></table>');
    $('#startupimg').click(function () {
        uphandler = $.ajaxFileUpload
                    (
	                    {
	                        url: '/Shared/PhotoUpload.cshtml',
	                        secureuri: false,
	                        fileElementId: 'img0',
	                        dataType: 'redir',
	                        //timeout: 120000,
	                        allowType: 'jpg|png|bmp|gif|jpeg',
	                        beforeSend: function () {
	                            $("#loading").show();
	                            $("#loading").html('正在上传照片,请不要刷新或者离开此页面...');
	                        },
	                        complete: function () {
	                            $("#loading").hide();
	                        },
	                        success: function (data, status) {
	                            if (status == 'success') {
	                                if (data == '0') {
	                                    alert('需要选择图片上传');
	                                } else if (data == '1') {
	                                    alert('照片格式有误(仅支持JPG, JPEG, GIF, PNG或BMP)');
	                                }
	                                else if (data == '2') {
	                                    alert('图片大小不能超过3兆');
	                                }
	                                else {
	                                    $("#"+obj).val(data);
	                                    dialog.close();
	                                }
	                            }
	                            else {
	                                alert("添加图片出错");
	                                dialog.close();
	                            }
	                        },
	                        error: function (data, status, e) {
	                            alert(e);
	                            dialog.close();
	                        }
	                    }
                    )
        return false;
    });
    var b2 = new X.UI.Button('<input class="input-button" type="button" value="\u53d6\u6d88" />');
    dialog.addButton(b2, dialog.close);
    dialog.show();
}
/*创建分类无限极DorpDownList
Author          :   xbf321@gmail.com
containerId     :   容器ID
cidStr          :   格式：0(cid)|0(parentId)|0(index可以有多个条目)
data            :   格式{"pid":2,"count":3,"cats":[{"id":3,"pid":2,"name":"企业动态"},{"id":4,"pid":2,"name":"新品发布"},{"id":5,"pid":2,"name":"论坛展会"}]}
defaultObj      :   默认选中的值格式：{ id: 6, cat: { id: 8} }  
*/
function createCatSelectV2(containerId, cidStr, data, defaultObj) {
    var cidArr = cidStr.split('|');
    var cid = cidArr[0];            //栏目ID
    var pid = cidArr[1];            //父节点ID
    var index = cidArr[2];          //索引ID,为了不让ID冲突，所以加了一个区分ID的标示
    var parentId = cid;

    if (data.count == 0) { return; }

    //创建栏目SELECT
    var selId = "cid_" + index + "_" + cid;
    var sel = document.createElement("SELECT");
    sel.setAttribute("id", selId);
    sel.setAttribute("name", selId);
    sel.setAttribute("index", index);

    var op = document.createElement("OPTION");
    op.setAttribute("value", "0");
    op.innerHTML = "==请选择分类==";
    sel.appendChild(op);
    for (i in data.cats) {
        var id = data.cats[i].id;
        op = document.createElement("OPTION");
        op.setAttribute("value", id);
        op.innerHTML = data.cats[i].name;
        if (defaultObj && defaultObj.id == id) {
            parentId = defaultObj.id;
            op.setAttribute("selected", "selected");
        }
        sel.appendChild(op);
    }
    $("#" + containerId).append(sel);

    //子类别SELECT容器
    var cidSpan = document.createElement("span");
    cidSpan.id = "cid_" + index + "_" + cid + "_span";
    cidSpan.setAttribute("index", index);
    cidSpan.style.cssText = "margin-left:10px;";
    $("#" + containerId).append(cidSpan);

    sel.onchange = function () {
        catSelect_OnChangedV2(this, cidSpan.id, parentId, index, defaultObj);
    }
    if (defaultObj && defaultObj.cat) {
        //如果设置默认值
        catSelect_OnChangedV2(sel, cidSpan.id, parentId, index, defaultObj.cat);
    }
}
function catSelect_OnChangedV2(selectObj, containerId, parentId, index, defaultObj) {
    var cid = selectObj.value;
    $("#" + selectObj.id + "_span").html('');
    if (cid == 0 || cid == -1) { return; }
    var postValue = jQuery.param({ m: "getcatsbyparentid", parentId: cid });
    $.ajax({
        type: "POST",
        url: "/ArticleCategory/Ajax.cshtml",
        data: postValue,
        dataType: "json",
        beforeSend: function (x) {
            $("#catloading").show();
        },
        success: function (data) {
            $("#catloading").hide();
            var reponse = cid + "|" + parentId + "|" + index;
            createCatSelectV2(containerId, reponse, data, defaultObj);
        }
    });
}
function getCatsV2(index, parentId) {
    var str = '';
    var selId = 'cid_' + index + '_' + parentId;
    if (document.getElementById(selId)) {
        var catId = document.getElementById(selId).value;
        if (catId > 0) {
            str += catId + '-' + getCatsV2(index, catId);
        }
    }
    return str;
}
/*End*/