const randomId = () => {
  const n = window.crypto.getRandomValues(new Uint32Array(1))[0];
  return n.toString(16);
};

var _ws = null; // typ

const startSock = (url = "ws://127.0.0.1:9696/jsonrpc") => {
  _ws = new WebSocket(url);
  console.log(`${_ws.url} opening ...`);

  _ws.onopen = (e) => {
    console.log(`${e.target.url} open.`);
  };

  _ws.onclose = (e) => {
    console.error(`${e.target.url} close: ${e.code}`);
    setTimeout(() => startSock(e.target.url), 5000);
  };

  _ws.onmessage = (e) => {
    console.log(`${e.target.url} message: ${e.data}`);
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

const throwAnException = (s) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "throwAnException",
    params: [s],
  };
  _ws.send(JSON.stringify(req));
};

const logIn = (workerNum, password, serverList=null) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "logIn",
    params: [workerNum, password, serverList],
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

const dequeue = (ctiIndex, channel) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "dequeue",
    params: [ctiIndex, channel],
  };
  _ws.send(JSON.stringify(req));
};

const dial = (calledTeleNum) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "dial",
    params: [calledTeleNum],
  };
  _ws.send(JSON.stringify(req));
};

const xfer = (groupId, workerNum="") => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "dial",
    params: [groupId, workerNum],
  };
  _ws.send(JSON.stringify(req));
};

const xferConsult = (groupId, workerNum="") => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "xferConsult",
    params: [groupId, workerNum],
  };
  _ws.send(JSON.stringify(req));
};

const xferExt = (calledTeleNum) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "xferExt",
    params: [calledTeleNum],
  };
  _ws.send(JSON.stringify(req));
};

const xferExtConsult = (calledTeleNum) => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "xferExtConsult",
    params: [calledTeleNum],
  };
  _ws.send(JSON.stringify(req));
};

const callIvr = (ivrId, invokeType=0, customString="") => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "callIvr",
    params: [ivrId, invokeType, customString],
  };
  _ws.send(JSON.stringify(req));
};

const getIvrMenu = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getIvrMenu",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const getStats = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "getStats",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const logOut = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "logOut",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const exitApp = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "exitApp",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const showApp = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "showApp",
    params: [],
  };
  _ws.send(JSON.stringify(req));
};

const hideApp = () => {
  const req = {
    jsonrpc: "2.0",
    id: randomId(),
    method: "hideApp",
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
