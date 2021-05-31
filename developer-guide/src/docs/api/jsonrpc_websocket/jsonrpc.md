# JSONRPC

座席程序的接口采用 RPC 方式，大体上遵照 [JSON-RPC 2.0 Specification](https://www.jsonrpc.org/specification) 的定义[^1]。

在本文的后续部分，我们将使用类似面向过程语言的函数定义形式，说明 API 接口。

本节针对各种常见的调用方式作出说明，更多信息请参考 <https://www.jsonrpc.org/specification> 。

## 举例说明

### 修改坐席状态

### 接收振铃事件

### 接听来电

座席程序提供了方法 `OffHook`，这个方法常被称为“摘机”。在该座席的软电话振铃时，调用此方法可以进行呼叫应答。

这个方法没有参数，其请求部分的 JSON 的形态可以是以下例子中的任意一个:

- 省略 `params` 属性:

  ```json
  {"jsonrpc": "2.0", "id": "value-of-the-id", "method": "offHook"}
  ```

- `params` 属性是空的数组:

  ```json
  {"jsonrpc": "2.0", "id": "value-of-the-id", "method": "offHook", "params": []}
  ```

- `params` 属性是空的对象:

  ```json
  {"jsonrpc": "2.0", "id": "value-of-the-id", "method": "offHook", "params": {}}
  ```

当座席收到这个请求时，如果执行成功，就会把响应数据发送给调用方。
对于此例中的 `offHook`，这个方法没有返回值，返回的结果是 `null`。

返回数据形如：

```json
{"jsonrpc": "2.0", "id": "value-of-the-id", "result": null}
```

如果在浏览器上访问进行这样的调用，可我可以这样编写 Javascript 代码:

```js
// 新建到座席程序的 WebSocket 连接.
const socket = new WebSocket('ws://localhost:9876');

// 输出收到的 JSONRPC Response
socket.onmessage = event => {
    console.log(event.data);
};

// 连接成功后发送 JSONRPC Request
socket.onopen = event => {
    socket.send(JSON.stringify({
        id: 1,
        method: "offHook",
    }));
};
```

如果执行成功，我们可以在浏览器的控制台观察到输出

```json
{id: 1, result: null}
```

## 综合起来

用时序图图表示:

bla bla ...

[^1]: 座席程序的RPC接口基本支持常见的 [JSONRPC][] 使用方式，但是有所限制:

      - 不支持 [`Batch`](https://www.jsonrpc.org/specification#batch) 方式
      - 在 RPC 请求执行完成之前，不接受新的 RPC 请求

[JSONRPC]: https://www.jsonrpc.org/specification
