#pragma once

#include "number.h"

f32 clampf(f32 v, f32 min, f32 max);
i32 clampi(i32 v, i32 min, i32 max);

f32 signf(f32 f);
i32 signi(i32 i);

f32 absf(f32 f);

f32 maxf(f32 a, f32 b);
f32 minf(f32 a, f32 b);
i32 maxi(i32 a, i32 b);
i32 mini(i32 a, i32 b);