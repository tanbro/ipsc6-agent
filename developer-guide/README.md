# README

使用 [MkDocs][] 与 [Material for MkDocs][] 撰写的《IPSC6 座席客户端开发人员指南》

## 环境搭建

1. 安装 Python 3.6 或以上

1. 在子目录 `.venv` 建立该项目的 Python 虚拟环境，并激活:

    - Posix:

      ```bash
      python -m venv .venv
      source .venv/bin/activate
      ```

    - Windows:

      ```bat
      python -m venv .venv
      .venv\Scripts\Activate.bat
      ```

1. 安装 PyPI 上的依赖软件包

    在上述 Python 虚拟环境激活的前提下执行命令:

    ```bash
    pip install -r requirements.txt
    ```

   > **Tips:**
   >
   > 如果 PyPI 官方站点访问速度慢，可以考虑使用阿里云或者腾讯云镜像站。

## PlantUML 和 PDF 相关问题

这个文档工程需要的外部工具：

- [_必需_] [PlantUML][] (通过 [plantuml-markdown][]): 将 Markdown Code Block 中的 [PlantUML][] 代码转为图形

  默认使用 [PlantUML][] 的官方在线服务  <http://www.plantuml.com/plantuml> 生成图形，无需本地安装 [PlantUML][] 与 [GraphViz][]。

- [_可选_] [WeasyPrint][] (通过 [mkdocs-with-pdf][]): 将生成的 `HTML` 站点转为 `PDF` 文档

  如果难以在Windows / MacOS 环境下使用 [WeasyPrint][]，可以考虑利用较新的常见 `Linux` 发布版本的 `docker` 镜像构建这个工程。

[mkdocs]: https://www.mkdocs.org/ "MkDocs is a fast, simple and downright gorgeous static site generator that's geared towards building project documentation."
[material for mkdocs]: https://squidfunk.github.io/mkdocs-material/
[plantuml]: https://plantuml.com
[plantuml-markdown]: https://github.com/mikitex70/plantuml-markdown
[GraphViz]: https://graphviz.org/ "GraphViz is open source graph visualization software."
[mkdocs-with-pdf]: https://github.com/orzih/mkdocs-with-pdf
[WeasyPrint]: https://weasyprint.org/ "WeasyPrint is a smart solution helping web developers to create PDF documents."
