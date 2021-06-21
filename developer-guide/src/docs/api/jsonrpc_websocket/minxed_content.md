# 混合内容安全问题

## Cross Origin

与普通的 HTTP 请求不同， 浏览器通常并不强制要求 WebSocket 连接到同源。所以我们并不需要考虑跨域问题。

## Mixed content

座席程序**不提供**安全的 WebSocket 连接(无 `wss://`, 仅 `ws://`) ，其默认的 URL 是 `ws://localhost:9696/jsonrpc` 或者 `ws://127.0.0.1:9696/jsonrpc` ， 对应的 origin 分别是 `http://localhost:9696` 和 `http://127.0.0.1:9696` 。

从非安全 (**非** `HTTPS`) 的 Origin 的页面访问上述地址通常并不会被浏览器安全策略拦截。

但是，如果从安全 (`HTTPS`) 的 Origin 访问上述地址，不同浏览器会有不同的安全策略。
这样情况下，通行的方式是使用 [CSP][] 规则让浏览器允许指定的混合内容。

不过，幸运的是，现代浏览器一般认为回环地址 `127.0.0.1` 是可以信赖的，时间更近一些的主流浏览器有些也会允许 `localhost` 上的混合内容。

资料显示[^1]:

- Chrome 允许 `http://127.0.0.1/` 和 `http://localhost/` 上的混合内容
- Firefox 55 和更高的版本允许 `http://127.0.0.1/` 上的混合内容
- Firefox 84 和更高的版本允许 `http://localhost/` 和 `http://*.localhost/` 上的混合内容
- Safari 不允许任何混合内容。(座席程序仅在 Windows 上运行，此情况不予考虑)

这表明，当用于对接/控制座席客户端的 WebApp 使用的浏览器是：

- Chrome/Chromium 或 Firefox(版本至少 55)： 可从 `HTTPS` 页面直接访问座席客户端的 WebSocket，除非有 [CSP][] 规则阻止。
- 其它浏览器： 需要进一步验证

[^1]: 参考来源: <https://developer.mozilla.org/en-US/docs/Web/Security/Mixed_content#loading_locally_delivered_mixed-resources>

[CSP]: https://www.w3.org/TR/CSP2/ "Content Security Policy"
