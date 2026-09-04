namespace GodotGadgets.FSCore.RadialMenuDomain

open System

/// 一个方向读数：摇杆轴合成 / 鼠标相对轮盘中心的偏移都归约为它。
/// 量级参与"回中判定"，方向参与"选扇区"。
[<Struct>]
type Stick = { X: float; Y: float }

/// 当前高亮。None = 摇杆回中（松开即取消）。
type Selection =
    | None
    | Sector of int

/// 轮盘可见状态 + 当前高亮（高亮跨帧携带，迟滞靠它判断）。
type WheelState =
    | Closed
    | Open of Selection

/// 核心只认这三种输入；Godot 层把原始 InputEvent 翻译成它们。
type DialInput =
    | ShoulderDown
    | Aim of Stick
    | ShoulderUp

/// 返回给表现层的事件；表现层只 switch、只执行，不做任何判断。
type WheelEvent =
    | Opened
    | HoverChanged of Selection
    | Picked of int
    | Cancelled

/// 一次 step 的结果：新状态 + 0..1 个待执行事件（表现层每帧调用也零成本）。
type StepResult = { State: WheelState; Events: WheelEvent list }

/// 扇区数 ≥ 2；激活半径 ∈ (0,1)；迟滞角 ∈ [0, 半扇区角)。
/// 字段 private——非法配置不可表示，只能经 Create/Default 获得合法值。
type RadialConfig =
    private
        { SlotCount: int
          ActivationRadius: float
          Hysteresis: float }

    static member Create(slotCount: int, activationRadius: float, hysteresis: float) =
        if slotCount < 2 then Error "至少需要 2 个扇区"
        elif activationRadius <= 0.0 || activationRadius >= 1.0 then Error "激活半径必须在 (0,1) 内"
        elif hysteresis < 0.0 || hysteresis >= Math.PI / float slotCount then
            Error "迟滞角必须在 [0, 半扇区角) 内"
        else
            Ok { SlotCount = slotCount; ActivationRadius = activationRadius; Hysteresis = hysteresis }

    /// 默认 8 扇区，迟滞取半扇区角的 1/3。
    static member Default =
        { SlotCount = 8; ActivationRadius = 0.5; Hysteresis = Math.PI / 8.0 / 3.0 }

[<RequireQualifiedAccess>]
module RadialMenu =
    /// 初始状态：轮盘未显示。
    let initial : WheelState = Closed

    /// 扇区数（C# 侧无法读 private 字段，经此函数取）。
    let slotCount (cfg: RadialConfig) = cfg.SlotCount

    /// 第 i 个扇区的中心角（弧度）。
    let centerAngle (cfg: RadialConfig) (sector: int) =
        2.0 * Math.PI * float sector / float cfg.SlotCount

    /// 扇区半宽（弧度）。
    let halfWidth (cfg: RadialConfig) = Math.PI / float cfg.SlotCount

    /// 归一化到 (-π, π] 的有符号角差（角环绕：圆上的最近距离）。
    let private wrapSigned rad =
        let twoPi = 2.0 * Math.PI
        let r = rad % twoPi
        if r > Math.PI then r - twoPi
        elif r < -Math.PI then r + twoPi
        else r

    let private magnitude (s: Stick) = sqrt (s.X * s.X + s.Y * s.Y)
    let private aimAngle (s: Stick) = atan2 s.Y s.X

    /// 距给定角最近的扇区。O(N) 暴力（N 小），避免闭式取整的环绕错误。
    let private nearestSector (cfg: RadialConfig) rad =
        [ 0 .. cfg.SlotCount - 1 ]
        |> List.minBy (fun i -> abs (wrapSigned (rad - centerAngle cfg i)))

    /// 带迟滞的高亮推进：
    /// - 无高亮 → 直接吸最近扇区；
    /// - 已高亮 → 越过"边界 + 迟滞"才步进相邻扇区（防边界抖动）；
    /// - 甩过一整格多（≥ 3·半宽）→ 视为大动作，直接吸最近扇区。
    let aim (cfg: RadialConfig) (current: Selection) (stick: Stick) : Selection =
        let rad = aimAngle stick
        match current with
        | None -> Sector(nearestSector cfg rad)
        | Sector cur ->
            let d = wrapSigned (rad - centerAngle cfg cur)
            let k = abs d
            let stepAt = halfWidth cfg + cfg.Hysteresis
            if k <= stepAt then current
            elif k >= 3.0 * halfWidth cfg then Sector(nearestSector cfg rad)
            elif d > 0.0 then Sector((cur + 1) % cfg.SlotCount)
            else Sector((cur + cfg.SlotCount - 1) % cfg.SlotCount)

    /// 单步：输入 + 状态 -> (新状态, 0..1 事件)。全函数、穷尽匹配。
    let step (cfg: RadialConfig) (input: DialInput) (state: WheelState) : StepResult =
        let next, events =
            match state, input with
            | Closed, ShoulderDown -> Open None, [ Opened ]
            | Closed, _ -> state, []
            | Open _, ShoulderDown -> state, []
            | Open sel, Aim stick when magnitude stick < cfg.ActivationRadius ->
                match sel with
                | None -> state, []
                | Sector _ -> Open None, [ HoverChanged None ]
            | Open sel, Aim stick ->
                let nextSel = aim cfg sel stick
                Open nextSel, (if nextSel = sel then [] else [ HoverChanged nextSel ])
            | Open(Sector i), ShoulderUp -> Closed, [ Picked i ]
            | Open None, ShoulderUp -> Closed, [ Cancelled ]

        { State = next; Events = events }

    // ---- C# 边界：只暴露显式参数函数；C# 侧不构造 DU，只读 StepResult ----
    let openDial (cfg: RadialConfig) (state: WheelState) = step cfg ShoulderDown state
    let aimDial (cfg: RadialConfig) (x: float) (y: float) (state: WheelState) =
        step cfg (Aim { X = x; Y = y }) state
    let closeDial (cfg: RadialConfig) (state: WheelState) = step cfg ShoulderUp state
