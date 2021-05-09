# README

该脚本用于生成 `pjproject` 的 `C#` wrapper

**注意！应在 `repo` 的根目录执行这些脚本，而不是脚本文件所在目录！**

生成的 `c#` 代码中，有些枚举值定义有误，类似

```csharp
public enum pjmedia_format_id {
    // ....
    PJMEDIA_FORMAT_INVALID = 0xFFFFFFFF
}
```

需要手动修正为:

```csharp
public enum pjmedia_format_id {
    // ....
    PJMEDIA_FORMAT_INVALID = -1
}
```

