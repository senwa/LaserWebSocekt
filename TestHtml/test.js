//#####LaserMark开始(当前代码块不要动)######
(function() {

    function Sock(url) {
        this.url = url;
        this.sock = null;
        this.connected = false;
        this.timeout = null;
        this.receive = null;

        this.reset();
        this.open();
    }

    Sock.prototype = {
        open: function() {
            var me = this;

            if (me.sock) {
                me.sock.onclose = null;
                me.sock.close();
            }

            var sock = me.sock = new WebSocket(me.url);

            sock.onopen = function(e) {
                me.reset();
                me.connected = true;
                me.onopen && me.onopen(e);
            };

            sock.onerror = function(e) {
                console.error(e);
                sock.close();
            }

            sock.onclose = function(e) {
                me.connected = false;
                me.retry && me.retry();

                me.onclose && me.onclose(e);
            }

            sock.onmessage = function(evt) {
                me.onmessage && me.onmessage(evt.data);
            }
        },

        send: function(obj) {
            this.sock.send(JSON.stringify(obj));
        },

        close: function() {
            this.retry = null;
            this.sock && this.sock.close();
        },

        reset: function() {
            this.retry = this._retryLoop;
            this.retryMs = 500;
        },

        _retryLoop: function() {
            var me = this;
            window.clearTimeout(me.timeout);
            me.timeout = window.setTimeout(function() {
                me.retryMs = Math.min(15000, me.retryMs += 500);
                me.open();
            }, me.retryMs);
        }
    };
    window.LaserMark = Sock;
}());
//#####LaserMark结束(上面代码块不要动)######

//#####业务处理代码开始#######
$(function() {

    //随便写一个可以在网页上看到状态
    function status(msg) {
        var d = $('<div />');
        d.text(msg);
        $('#status').append(d);
    }

    //创建一个连接,用于发送数据接收数据
    var laserMark = new LaserMark('ws://localhost:8881');

    //绑定事件:连接上之后事件
    laserMark.onopen = function() {
        $('#status').html(' ');
        status('已连接');
    };
    //绑定事件:连接关闭事件
    laserMark.onclose = function() {
        status('连接已关闭');
    };
    //绑定事件:网页端收到消息事件
    laserMark.onmessage = function(evt) {
        status(evt.data);
        console.log(evt.data);
    };

    //发送按钮
    $('#sendBtn').bind('click', function() {
        //获取要发送的批号
        var message = $('#message').val();
        //如果获取到的批号是逗号隔开的,会发送后按逗号隔开批量执行
        if (message) {
            var msgs = message.split(",");
            var msgList = [];
            for (var i = 0; i < msgs.length; i++) {
                //数据格式,发送一个数组,id可以随便写,PrintCode是需要打印的内容,注意大小写
                msgList.push({
                    ID: i,
                    PrintCode: msgs[i]
                });
            }

            var msg = {
                msgs: msgList
            };

            //发送数据
            laserMark.send(msg);
        }
    })
});
//#####业务处理代码结束#######