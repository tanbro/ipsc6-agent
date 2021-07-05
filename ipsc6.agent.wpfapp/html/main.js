const randomId = () => {
  const n = window.crypto.getRandomValues(new Uint32Array(1))[0];
  return n.toString(16);
};

var _ws = null; // typ

const startSock = (url = "ws://127.0.0.1:9696/jsonrpc") => {
  _ws = new WebSocket(url);
  console.log(`${_ws.url} opening.`);

  _ws.onopen = (e) => {
    console.log(`${e.target.url} onopen.`);
  };

  _ws.onclose = (e) => {
    console.error(`${e.target.url} onclose. ${e.code}`);
    setTimeout(() => startSock(e.target.url), 5000);
  };

  _ws.onmessage = (e) => {
    console.log(`${e.target.url} onmessage. ${e.data}`);
  };
};

const stopSock = (url) => {
  if (_ws !== null) {
    _ws.onclose = null;
    _ws.close();
  }
};

const echo = (s) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "echo",
    params: [s],
  };
  _ws.send(JSON.stringify(req));
};

const logIn = (workerNum, password) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "logIn",
    params: [workerNum, password],
  };
  _ws.send(JSON.stringify(req));
};

const signGroup = (id, isSignIn=true) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "signGroup",
    params: [id, isSignIn],
  };
  _ws.send(JSON.stringify(req));
};

const getGroups = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getGroups",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const setBusy = (value=10) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "setBusy",
    params: [value],
  };
  _ws.send(JSON.stringify(req));
};

const setIdle = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "setIdle",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const getModel = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getModel",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const getCtiServers = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getCtiServers",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const getStatus = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getStatus",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const getWorkerNum = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getWorkerNum",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};


const getSipAccounts = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getSipAccounts",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const getCalls = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getCalls",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const answer = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "answer",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const hangup = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "hangup",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const hold = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "hold",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const unHold = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "unHold",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const getQueueInfos = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getQueueInfos",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

document.getElementById("frmLogin").addEventListener("submit", ev=>{
  ev.preventDefault();
  const form = ev.submitter.form;
  const inputs = form.elements;
  const workerNum = inputs[0].value;
  const password = inputs[1].value;
  logIn(workerNum, password);
});

startSock();
