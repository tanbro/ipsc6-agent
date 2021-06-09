# 技能组

## 获取技能组列表

- **Method**: `getSkillGroups`

- **Params**: 无

- **Result**:

    座席所属的技能组的数组，数组元素是 [技能组对象][]。

    eg:

    ```json
    [
        {"id": "zx", "name": "咨询", "signed": true},
        {"id": "sh", "name": "售后", "signed": true},
        {"id": "ts", "name": "投诉", "signed": false}
    ]
    ```

## 签入

- **Method**: `signIn`

- **Params**:

    1. 如不提供参数，或者参数为空数组，则签入该座席所属的所有技能组
    1. 否则参数应是字符串数组，数组元素是技能组的ID。座席将签入这些技能组。

- **Result**: `null`

## 签出

- **Method**: `signOut`

- **Params**:

    1. 如不提供参数，或者参数为空数组，则签出该座席所属的所有技能组
    1. 否则参数应是字符串数组，数组元素是技能组的ID。座席将签出这些技能组。

- **Result**: `null`

[技能组对象]: ../../objects/skill_group.md