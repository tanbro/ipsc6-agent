# README

使用 [MkDocs][] 与 [Material for MkDocs][] 撰写的《IPSC6 座席客户端开发人员指南》

## 环境搭建

1. 安装 Python 3.6 或以上

1. [_可选_] 在子目录 `venv` 建立该项目的 Python 虚拟环境，并激活:

    - Posix:

        ```sh
        python -m venv venv
        source venv/bin/activate
        ```

    - Windows 命令提示符:

        ```bat
        py -m venv venv
        venv\Scripts\Activate.bat
        ```

1. 安装依赖软件

    在上述 Python 虚拟环境激活的前提下执行命令:

    ```sh
    pip install -r requirements.txt
    ```

## PlantUML 和 PDF 相关问题

这个文档工程：

- 使用 [PlantUML][] 绘图(通过 [plantuml-markdown][] 文档中转 Markdown Code Block 为图形)
- 使用 [WeasyPrint][] (通过 [mkdocs-with-pdf][]) 将生成的 `HTML` 站点转为 `PDF` 文档

但这些工具链比较复杂，包括 [PlantUML][], [GraphViz][], [WeasyPrint][] 等，它们在 Windows 环境下可能并不容易使用。

如果难以配置，建议在较新的常见 `Linux` 发布版本 `docker` 镜像中构建这个工程。

> 💡 **Tip**:
>
> 修改 `mkdocs.yml` 中的 [plantuml-markdown][] 插件设置，可直接使用公网上的 [PlantUML][] 在线绘图生成服务，如 <http://www.plantuml.com/plantuml>

[mkdocs]: https://www.mkdocs.org/ "MkDocs is a fast, simple and downright gorgeous static site generator that's geared towards building project documentation."
[material for mkdocs]: https://squidfunk.github.io/mkdocs-material/
[plantuml]: https://plantuml.com
[plantuml-markdown]: https://github.com/mikitex70/plantuml-markdown
[GraphViz]: https://graphviz.org/ "GraphViz is open source graph visualization software."
[mkdocs-with-pdf]: https://github.com/orzih/mkdocs-with-pdf
[WeasyPrint]: https://weasyprint.org/ "WeasyPrint is a smart solution helping web developers to create PDF documents."
