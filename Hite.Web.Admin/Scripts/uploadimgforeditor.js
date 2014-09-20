(function () {
    tinymce.create('tinymce.plugins.xuimagePlugin', {
        bookMark: null,
        init: function (ed, url) {
            var This = this;
            ed.addCommand('mceXuimage', function () {
                This.bookMark = ed.selection.getBookmark(); //Fix IE BUG 保存焦点
                //bookmark = ed.selection.getBookmark(1);
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
	                                    if (data != "") {
	                                        insertImage(data);
	                                    }
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
                function insertImage(url) {
                    //ed.selection.moveToBookmark(this.bookMark);
                    ed.selection.moveToBookmark(This.bookMark);
                    var _url = url || $(ed.id + "_theImageUrl").value;
                    ed.execCommand('mceInsertContent', false, '<p style="text-align: center;"><img id="__mce_tmp" /></p>', { skip_undo: 1 });
                    ed.dom.setAttribs('__mce_tmp', { src: _url });
                    ed.dom.setAttrib('__mce_tmp', 'id', '');
                    ed.undoManager.add();
                }
            });
            ed.addButton('xuimage', {
                title: '\u4E0A\u4F20\u672C\u5730\u56FE\u7247',
                cmd: 'mceXuimage',
                image: '/images/editor/image.gif'
            });
            ed.onNodeChange.add(function (ed, cm, n) {
                //cm.setActive('xuimage', n.nodeName == 'IMG');
            });
        },
        createControl: function (n, cm) {
            return null;
        },
        getInfo: function () {
            return {
                longname: 'Xuimage plugin',
                author: 'Some author',
                authorurl: 'http://tinymce.moxiecode.com',
                infourl: 'http://wiki.moxiecode.com/index.php/TinyMCE:Plugins/example',
                version: "1.0"
            };
        }
    });
    tinymce.PluginManager.add('xuimage', tinymce.plugins.xuimagePlugin);
})();