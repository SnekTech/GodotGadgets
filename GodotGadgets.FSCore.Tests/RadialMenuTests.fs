module RadialMenuTests

open System
open Expecto
open GodotGadgets.FSCore.RadialMenuDomain

let private deg d = d * Math.PI / 180.0

/// 构造一个指定方向角与量级的摇杆读数（量级 1 = 满推）。
let private stickAt (angleDeg: float) (magnitude: float) : Stick =
    let r = deg angleDeg
    { X = magnitude * cos r; Y = magnitude * sin r }

/// 8 扇区 + 指定迟滞的合法配置（迟滞单位：度）。
let private cfg (hysteresisDeg: float) =
    match RadialConfig.Create(8, 0.5, deg hysteresisDeg) with
    | Ok c -> c
    | Error msg -> failwith msg

let private cfg0 = cfg 0.0
let private cfgH = cfg 15.0 // 半宽 22.5°，迟滞 15° → 切换阈值 37.5°

[<Tests>]
let radialMenuBehavior =
    testList "radialMenu" [
        // ---- 几何：无迟滞时"最近扇区" ----
        testCase "各扇区中心角对准自身" <| fun () ->
            for i in 0..7 do
                let sel = RadialMenu.aim cfg0 Selection.None (stickAt (float i * 45.0) 1.0)
                Expect.equal sel (Sector i) $"中心角 {i * 45}° 应对准扇区 {i}"

        testTheory "最近扇区（含 ±180° 环绕缝）" [
            (0.0, 0); (45.0, 1); (90.0, 2); (135.0, 3)
            (180.0, 4); (225.0, 5); (270.0, 6); (315.0, 7)
            (359.0, 0) // 359° 折叠成 -1°，应选 0 而非 7
            (-1.0, 0)
            (181.0, 4)
            (179.0, 4)
        ] <| fun (angleDeg, expected) ->
            let sel = RadialMenu.aim cfg0 Selection.None (stickAt angleDeg 1.0)
            Expect.equal sel (Sector expected) $"{angleDeg}° 应对准扇区 {expected}"

        // ---- 迟滞 ----
        testCase "迟滞：越过边界但在迟滞带内不换扇区" <| fun () ->
            // 扇区 0 中心 0°，边界 22.5°，迟滞带到 37.5°；30° 仍在带内
            let sel = RadialMenu.aim cfgH (Sector 0) (stickAt 30.0 1.0)
            Expect.equal sel (Sector 0) "30° 应在迟滞带内保持扇区 0"

        testCase "迟滞：明确越界才步进相邻扇区" <| fun () ->
            let sel = RadialMenu.aim cfgH (Sector 0) (stickAt 40.0 1.0)
            Expect.equal sel (Sector 1) "40° 越过迟滞带应步进到扇区 1"

        testCase "迟滞：反向越界步进到上一个扇区（环绕）" <| fun () ->
            let sel = RadialMenu.aim cfgH (Sector 0) (stickAt 300.0 1.0) // -60°
            Expect.equal sel (Sector 7) "-60° 应步进到扇区 7"

        testCase "大幅甩动直接吸附最近扇区" <| fun () ->
            let sel = RadialMenu.aim cfg0 (Sector 0) (stickAt 140.0 1.0)
            Expect.equal sel (Sector 3) "140° 距扇区 3(135°)最近，应直接跳过"

        // ---- 生命周期（经 C# 边界 wrapper 测，覆盖同一条路径）----
        testCase "按下肩键打开轮盘并发出 Opened" <| fun () ->
            let r = RadialMenu.openDial cfg0 RadialMenu.initial
            Expect.equal r.State (Open None) "打开后无高亮"
            Expect.equal r.Events [ Opened ] "应发出 Opened"

        testCase "关闭状态收到 Aim / 松开被忽略" <| fun () ->
            let rAim = RadialMenu.aimDial cfg0 1.0 0.0 RadialMenu.initial
            Expect.equal rAim.State RadialMenu.initial "关闭时 Aim 不应改变状态"
            Expect.equal rAim.Events [] "不应有事件"
            let rUp = RadialMenu.closeDial cfg0 RadialMenu.initial
            Expect.equal rUp.State RadialMenu.initial "关闭时松开不应改变状态"
            Expect.equal rUp.Events [] "不应有事件"

        testCase "低量级视为回中，且本已回中时无事件" <| fun () ->
            let opened = RadialMenu.openDial cfg0 RadialMenu.initial
            let r = RadialMenu.aimDial cfg0 0.3 0.0 opened.State
            Expect.equal r.State (Open None) "低量级应保持无高亮"
            Expect.equal r.Events [] "本已回中，不应发事件"

        testCase "有效瞄准发 HoverChanged，重复瞄准幂等" <| fun () ->
            let opened = RadialMenu.openDial cfg0 RadialMenu.initial
            let r1 = RadialMenu.aimDial cfg0 1.0 0.0 opened.State
            Expect.equal r1.State (Open(Sector 0)) "0° 应选中扇区 0"
            Expect.equal r1.Events [ HoverChanged(Sector 0) ] "应发 HoverChanged"
            let r2 = RadialMenu.aimDial cfg0 1.0 0.0 r1.State
            Expect.equal r2.State r1.State "状态不变"
            Expect.equal r2.Events [] "同扇区重复瞄准不应重复发事件"

        testCase "回到中心清空高亮并通知" <| fun () ->
            let opened = RadialMenu.openDial cfg0 RadialMenu.initial
            let aimed = RadialMenu.aimDial cfg0 1.0 0.0 opened.State
            let r = RadialMenu.aimDial cfg0 0.1 0.1 aimed.State
            Expect.equal r.State (Open None) "回中后无高亮"
            Expect.equal r.Events [ HoverChanged None ] "应通知清空高亮"

        testCase "松开确认 Picked / 回中松开取消 Cancelled" <| fun () ->
            let opened = RadialMenu.openDial cfg0 RadialMenu.initial
            let aimed = RadialMenu.aimDial cfg0 1.0 1.0 opened.State // 45° → 扇区 1
            Expect.equal aimed.State (Open(Sector 1)) "45° 应选中扇区 1"
            let picked = RadialMenu.closeDial cfg0 aimed.State
            Expect.equal picked.State Closed "确认后关闭"
            Expect.equal picked.Events [ Picked 1 ] "应发 Picked 1"
            let cancelled = RadialMenu.closeDial cfg0 opened.State // 无高亮松开
            Expect.equal cancelled.State Closed "取消后关闭"
            Expect.equal cancelled.Events [ Cancelled ] "应发 Cancelled"

        // ---- config 校验 ----
        testCase "config 拒绝非法值、接受合法值" <| fun () ->
            Expect.isError (RadialConfig.Create(1, 0.5, 0.0)) "扇区数 < 2 应拒绝"
            Expect.isError (RadialConfig.Create(8, 0.0, 0.0)) "激活半径 0 应拒绝"
            Expect.isError (RadialConfig.Create(8, 1.0, 0.0)) "激活半径 1 应拒绝"
            Expect.isError (RadialConfig.Create(8, 0.5, -0.1)) "负迟滞应拒绝"
            Expect.isError (RadialConfig.Create(8, 0.5, Math.PI / 8.0)) "迟滞 ≥ 半宽应拒绝"
            Expect.isOk (RadialConfig.Create(8, 0.5, Math.PI / 8.0 / 3.0)) "默认迟滞合法"
    ]
