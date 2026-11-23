---
layout: layouts/blog.njk
title: C# 中的函数式错误处理
date: 2024-04-10
author: Christian Findlay
excerpt: 了解 RestClient.Net 如何使用 Result 类型和可辨识联合实现函数式错误处理。
lang: zh
permalink: /zh/blog/functional-error-handling/
tags:
  - zhposts
  - tutorial
---

# C# 中的函数式错误处理

传统的 C# 错误处理依赖异常。但异常有问题：

1. **不可见** - 你无法从方法签名知道它可能抛出什么
2. **性能开销** - 抛出异常是昂贵的
3. **容易遗漏** - 忘记处理异常意味着运行时崩溃

## Result 类型方法

RestClient.Net 使用 Result 类型来表示可能失败的操作：

```csharp
// 返回类型清楚地表明这可能失败
Result<Post, HttpError<ErrorResponse>> result = await httpClient.GetAsync(...);
```

## 穷尽性检查的力量

使用 Exhaustion 分析器，遗漏任何情况都会导致编译错误：

```csharp
// 这不会编译！
var output = result switch
{
    OkPost(var post) => "成功",
    // 遗漏了 ErrorPost 情况
};
```

编译器会告诉你：

```
error EXHAUSTION001: Switch 不完整
遗漏: ErrorPost
```

## 最佳实践

1. **总是使用类型别名** - 它们使模式匹配更加简洁
2. **不要吞掉错误** - 始终处理所有情况
3. **使用 Exhaustion** - 让编译器帮助你

## 总结

函数式错误处理使你的代码更安全、更可预测。RestClient.Net 让这在 C# 中变得简单！
