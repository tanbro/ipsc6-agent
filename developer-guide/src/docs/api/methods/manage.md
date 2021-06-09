# 管理

## 闭塞

被闭塞的座席不参与任何排队

- **Method**: `block`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`

## 取消闭塞

被闭塞的座席不参与任何排队

- **Method**: `unBlock`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`

## 强行注销

- **Method**: `kickOut`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`

## 强行签出

- **Method**: `signOut`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`
