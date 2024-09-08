# Shape2D
## 一、项目介绍

1.2D图形相交检测库，已支持OBB、圆形、扇形、圆环之间的相互相交判定，以及2D点在以上几种形状中的判定。

2.可以用于技能命中判定、交互范围判定等，相比于Unity的Collider实现上述功能性能更好。

3.一些更复杂的形状可以考虑由以上形状拼装。

## 二、使用方法

1.源码拷贝到项目中。

2.创建合适的形状并保存。

```c#
var obb = new OBB(new Vector2(0, 0), new Vector2(1,1), new Vector2(0, 1));
var circle = new Circle(new Vector2(2, 0), 1);
```

3.判定相交。

```c#
// 两种皆可
var isIntersect1 = obb.IsIntersect(circle);
var isIntersect2 = circle.IsIntersect(obb);
```

## 三、源码介绍

1.基于SAT(Separating Axis Theorem)分离轴定律实现。

2.具体细节可查看各形状的IsIntersect方法，每一步都有注释。

## 四、后续计划

1.支持Unity编辑器和运行时的各形状显示，方便测试相交判定结果。

2.增加Broad Phase粗筛阶段功能，支持多种效果（网格、四叉树、BVH Tree、Sort and Sweep等等）。

