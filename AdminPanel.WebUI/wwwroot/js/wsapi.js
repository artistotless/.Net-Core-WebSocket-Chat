var WrapperWebSocket = function (url) {

	var conn = new WebSocket(url);
	var callbacks = {};

	this.bind = function (event_name, callback) {
		callbacks[event_name] = callbacks[event_name] || [];
		callbacks[event_name].push(callback);
		return this;// chainable
	};

	this.send = function (event_name, event_data) {
		var payload = JSON.stringify({ "type": event_name, "data": event_data });
		conn.send(payload);
		return this;
	};

	// dispatch to the right handlers
	conn.onmessage = function (evt) {
		console.log(evt.data);
		var json = JSON.parse(evt.data)
		dispatch(json['type'], json['data'])
	};

	conn.onclose = function () { dispatch('close', null) }
	conn.onopen = function () { dispatch('open', null) }

	var dispatch = function (event_name, message) {
		var chain = callbacks[event_name];
		if (typeof chain == 'undefined') return; // no callbacks for this event
		for (var i = 0; i < chain.length; i++) {
			chain[i](message)
		}
	}
};

var isConnected;
var server;

function connect(uri) {
	server = new WrapperWebSocket(uri);
	console.log("Connecting to " + uri)

	server.bind('close', function () {
		isConnected = false;
		console.log("Connection is closed")
	})
}


function sendMessage(message, playerId) {
	let data = {
		"message": message,
		"playerId": playerId
	};
	server.send("sendMessage", data);
}


function setMessageRead(messageId) {
	let data = {
		"messageId": messageId,
	};
	console.log(this.id);
	server.send("setMessageRead", data);
}


var prepend = function prepend(message) {
	$('#messages').prepend('<p>' + message + '</p>');
}

function addMessage(data, isAppend) {

	let msg = $('<p>', {
		class: 'message'
	});

	let readBtn = $('<button>', {
		class: 'readBtn',
		id: `${data.messageId}`,
		type: 'submit'
	});

	readLabel = 'Прочитано';
	unreadreadLabel = 'Отметить прочитанным';

	readBtn.html(data.read == null || data.read == false ? unreadreadLabel : readLabel)
	let messageText = getParsedData(data);
	if (readBtn.text() == unreadreadLabel) {
		readBtn.click(function () {
			setMessageRead(this.id);
			this.removeEventListener('click', arguments.callee, false);
			this.innerHTML = readLabel;
		});
	}

	msg.html(messageText);
	if (data.operator == null || data.operator.name == "") {
		msg.append(readBtn);
	}

	if (isAppend)
		msg.appendTo($('#messages'));
	else
		msg.prependTo($('#messages'));
}

function getParsedData(data) {
	return `${data.timeStamp} - ${data.operator == null ||
		data.operator.name == "" ? data.player.name : data.operator.name} : ${data.text}`;
}

function setHtmlPages(currentPage, totalPages) {

	$('#pager').empty();

	for (var i = 1; i <= totalPages; i++) {

		var btn = $('<button>', {
			class: currentPage != i ? 'pageBtn' : 'selected',
			type: 'submit',
			onclick: `getMessages(${i})`
		});

		btn.html(i.toString());
		btn.appendTo('#pager');
	}
}

function clear() {
	$('#messages').empty();
}
