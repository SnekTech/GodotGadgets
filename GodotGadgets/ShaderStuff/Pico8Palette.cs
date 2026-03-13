namespace GodotGadgets.ShaderStuff;

using Godot;

/// <summary>
/// 包含 PICO-8 幻想主机完整调色板的静态类。
/// 所有颜色值均以 Godot 的 Color 类型表示，可直接用于绘图和材质。
/// </summary>
[UsedImplicitly]
public static class Pico8Palette
{
    // 索引 0: 黑色
    public static readonly Color Black = new Color(0f / 255f, 0f / 255f, 0f / 255f);
    
    // 索引 1: 深蓝
    public static readonly Color DarkBlue = new Color(29f / 255f, 43f / 255f, 83f / 255f);
    
    // 索引 2: 深紫
    public static readonly Color DarkPurple = new Color(126f / 255f, 37f / 255f, 83f / 255f);
    
    // 索引 3: 深绿
    public static readonly Color DarkGreen = new Color(0f / 255f, 135f / 255f, 81f / 255f);
    
    // 索引 4: 棕
    public static readonly Color Brown = new Color(171f / 255f, 82f / 255f, 54f / 255f);
    
    // 索引 5: 深灰
    public static readonly Color DarkGray = new Color(95f / 255f, 87f / 255f, 79f / 255f);
    
    // 索引 6: 浅灰
    public static readonly Color LightGray = new Color(194f / 255f, 195f / 255f, 199f / 255f);
    
    // 索引 7: 白
    public static readonly Color White = new Color(255f / 255f, 241f / 255f, 232f / 255f);
    
    // 索引 8: 红
    public static readonly Color Red = new Color(255f / 255f, 0f / 255f, 77f / 255f);
    
    // 索引 9: 橙
    public static readonly Color Orange = new Color(255f / 255f, 163f / 255f, 0f / 255f);
    
    // 索引 10: 黄
    public static readonly Color Yellow = new Color(255f / 255f, 236f / 255f, 39f / 255f);
    
    // 索引 11: 绿
    public static readonly Color Green = new Color(0f / 255f, 228f / 255f, 54f / 255f);
    
    // 索引 12: 蓝
    public static readonly Color Blue = new Color(41f / 255f, 173f / 255f, 255f / 255f);
    
    // 索引 13: 紫
    public static readonly Color Purple = new Color(131f / 255f, 118f / 255f, 255f / 255f);
    
    // 索引 14: 粉
    public static readonly Color Pink = new Color(255f / 255f, 119f / 255f, 168f / 255f);
    
    // 索引 15: 浅桃
    public static readonly Color LightPeach = new Color(255f / 255f, 204f / 255f, 170f / 255f);
}