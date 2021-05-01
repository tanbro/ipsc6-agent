using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public enum Privilege
    {
        //系统呼出
        AUTO_CALLOUT = 3,
        //闭塞其它座席
        BLOCK = 13,
        //挂断
        BREAK_SESS = 20,
        //排队呼入
        CALLIN = 1,
        //咨询到座席
        CONSULT = 7,
        //咨询到外线
        CONSULT_EX = 8,
        //指定呼入
        DES_CALLIN = 2,
        //强拆其它座席 ForceHangup
        FORCEBREAK = 12,
        //强制其它座席空闲
        FORCEIDLE = 21,
        // 强插
        FORCEINSERT = 11,
        //强制其它座席暂停
        FORCEPAUSE = 22,
        // 强行签出
        FORCESIGNOFF = 16,
        //问候语管理
        GREETVOICE = 23,
        //拦截 Intercept
        INTERCEPT = 6,
        //强行注销
        KICKOUT = 15,
        //监听、停止监听
        LISTEN = 17,
        //手动电话呼出
        MAN_CALLOUT = 5,
        //手动咨询
        MAN_CONSULT = 30,
        //修改权限
        MODIFYPOWER = 24,
        //修改扩展权限
        MODIFYPOWER_EX = 25,
        //录音、停止录音 （本座席）
        RECORD = 18,
        //录音、停止录音 （其它座席）
        RECORD2 = 19,
        //手动系统呼出
        SYS_CALLOUT = 4,
        //转接到座席 Transfer
        TRANSFER = 9,
        //转接到外线 Transfer
        TRANSFER_EX = 10,
        //解闭其它座席
        UNBLOCK = 14,
        //座席权限
        UNUSED01 = 0,
        //保留
        UNUSED02 = 26,
        //保留
        UNUSED03 = 27,
        //保留
        UNUSED04 = 28,
        //保留
        UNUSED05 = 29,
        //保留
        UNUSED06 = 31,
        //保留
        UNUSED07 = 32,
        //保留

        UNUSED08 = 33,
        //保留

        UNUSED09 = 34,
        //保留

        UNUSED10 = 35,
        //保留

        UNUSED11 = 36,
        //保留

        UNUSED12 = 37,
        //保留

        UNUSED13 = 38,
        //保留

        UNUSED14 = 39,
        //保留

        UNUSED15 = 40,
    }
}
