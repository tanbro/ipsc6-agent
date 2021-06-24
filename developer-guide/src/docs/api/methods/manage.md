# 管控相关方法

## 监听

-   **Method**: `monitor`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 取消监听

-   **Method**: `unMonitor`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 振铃拦截

-   **Method**: `intercept`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 强行插话

-   **Method**: `interrupt`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 强行挂断

-   **Method**: `hangup`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 强行示忙

-   **Method**: `setBusy`

-   **Params**:

    | Argument    | Type      | Default | Description                     |
    | ----------- | --------- | ------- | ------------------------------- |
    | `workerNum` | `String`  | -       | 目标座席工号                    |
    | `workType`  | `Integer` | `10`    | 忙碌的种类: [WorkType][] 枚举值 |

-   **Result**: `null`

## 强行示闲

-   **Method**: `setIdle`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 闭塞

被闭塞的座席不参与任何排队

-   **Method**: `block`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 取消闭塞

被闭塞的座席不参与任何排队

-   **Method**: `unBlock`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 强行注销

-   **Method**: `kickOut`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

## 强行签出

-   **Method**: `signOut`

-   **Params**:

    | Argument    | Type     | Default | Description  |
    | ----------- | -------- | ------- | ------------ |
    | `workerNum` | `String` | -       | 目标座席工号 |

-   **Result**: `null`

[worktype]: ../types/enums.md#座席工作类型

--8<-- "includes/glossary.md"
