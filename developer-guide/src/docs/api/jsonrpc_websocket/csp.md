# 内容安全策略问题

当开发者从浏览器访问访问座席程序时，以下两个特性可能导致内容安全策略(CSP)问题:

1. 座席程序的嵌入式 WebServer 在本地 `localhost` 监听，它的域名是 `localhost` 或 `127.0.0.1`，与要控制它的系统域名不同。
1. 座席程序的嵌入式 WebServer 仅提供 `HTTP` 而 **不提供 `HTTPS`**。

为了能够从业务系统控制座席程序，我们需要在页面上加上 CSP 头。

我们需要配置的网络服务器返回  `Content-Security-Policy`  HTTP头部；除此之外, `<meta>`  元素也可以被用来配置该策略, 例如：

```html
<meta
    http-equiv="Content-Security-Policy"
    content="connect-src ws://localhost:9876"
>
```

对于座席程序，由于需要 `WebSocket` 连接，我们需要在 CSP 头中指定合适的 `connect-src`。

举例来说，假设我们配置座席程序在本地的 `9876` 端口提供 HTTP 服务，则 `CSP` 头可以是: `connect-src ws://localhost:9876`

这样，我们就可以使用 `WebSocket` 对象连接到 `ws://localhost:9876`:

```javascript
var ws = new WebSocket(ws://localhost:9876);
ws.onopen = event=>{
    console.log("opened!");
};
ws.onmessage = event=>{
    console.log(`message data: ${event.data}`);
};
ws.onclose = event=>{
    console.log("closed!");
};
```

另外一点要注意点是，**一旦使用 CSP ，原有的 inline script/style 就不再被允许**。

- 这样的脚本 (inline event handlers) 会被拦截：

    ```html
    <button id="btn" onclick="doSomething()">
    ```

    我们需要用使用 `addEventListener` 取代:

    ```javascript
    document.getElementById("btn").addEventListener('click', doSomething);
    ```

- 这样的脚本 (inline script) 也会被拦截

    ```html
    <script>
      var inline = 1;
    </script>
    ```

    除非:

    1. 使用 `unsafe-inline` CSP 策略:

        ```html
        Content-Security-Policy: script-src 'unsafe-inline';
        ```

    1. 使用 `nonce` 验证：

        在 CSP 头中设置 `nonce`:

        ```http
        Content-Security-Policy: script-src 'nonce-2726c7f26c'
        ```

        在 `<script>` 标签中设置相同的 `nonce`

        ```html
        <script nonce="2726c7f26c">
            var inline = 1;
        </script>
        ```

    1. 使用散列值验证

        在 CSP 头中设置 inline 脚本的散列值的算法和 Base64 字串。
        (CSP 支持 sha256, sha384 和 sha512，计算散列值的时候要包含首尾的空白。)

        例如，有 `<script>` 内容如下：

        ```html
        <script>var inline = 1;</script>
        ```

        可在 CSP 头中设置其 SHA256 Hash:

        ```http
        Content-Security-Policy: script-src 'sha256-B2yPHKaXnvFWtRChIbabYmUBFZdVfKKXHbWtWidDVF8='       
        ```

!!! tip
    目前， W3C 推荐的 CSP 版本是 Level 2，这也是主流浏览器采用的标准。
    可访问 <https://www.w3.org/TR/CSP2/> 获取更详细的信息
