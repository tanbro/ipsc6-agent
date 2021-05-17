using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// 有关程序集的常规信息通过下列属性集
// 控制。更改这些属性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("hesong.Lib.Log")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("hesong.Lib.Log")]
[assembly: AssemblyCopyright("版权所有 (C)  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 属性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("c85bd37c-9fe5-4cf2-9a27-c3e9cb6b5ade")]

// 允许部分受信任的代码调用具有强名称的程序集
[assembly: AllowPartiallyTrustedCallers()]
// 使用文件的名称初始化 AssemblyKeyFileAttribute 类的新实例，该文件包含为正在属性化的程序集生成强名称的密钥对。
[assembly: AssemblyKeyFile(@"hesong.Lib.Log.snk")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“修订号”和“内部版本号”的默认值，
// 方法是按如下所示使用“*”:
[assembly: AssemblyVersion("1.0.2.3")]
[assembly: AssemblyFileVersion("1.2021.0502.1909")]

// 更新日志
//
// 2021年05月02日
//     版 本 号：2.0.0.1
//     文件版本：1.2021.0502.1909
//     更新内容：切换至新的项目空间[hesong.Lib.Log]
//               切换framework平台版本至 .Net Framework 4.7.2
//               切换签名文件至新版本
//
// 2014年11月25日
//     版 本 号：1.0.2.3
//     文件版本：1.2014.1125.1535
//     更新内容：为构造方法增加2个重载，引入XmlElement对象当作配置参数
//
// 2012年09月12日
//     版 本 号：1.0.2.2
//     文件版本：1.2012.0912.1304
//     更新内容：为Log方法和COW方法分别增加一个参数logType为string的重载
//
// 2012年08月09日
//     版 本 号：1.0.2.1
//     文件版本：1.2012.0809.1212
//     更新内容：重新封装，为程序集生成强名称的密钥对，允许部分受信任的代码调用具有强名称的程序集
//
// 2012年04月23日
//     版 本 号：1.0.1.1
//     文件版本：1.2012.0423.2306
//     更新内容：重新封装，规范了工程名称和命名空间。[hesong.Lib.Log]
//
// 2012年02月06日
//     版 本 号：1.0.0.1
//     文件版本：1.2012.0206.1459
//     更新内容：第一个版本的日志操作封装类库。
//               实现了动态指定log4net配置文件及日志保存器功能。
//               实现了输出日志到控制台功能。
