#include "color.h"
#include "math.h"
#include "utils.h"
#include "stdio.h"

u32 col_mix_linear(u32 c1, u32 c2) {
    u8 r1, g1, b1, r2, g2, b2;
    col_get_rgb(c1, &r1, &g1, &b1);
    col_get_rgb(c2, &r2, &g2, &b2);

    return col_combine_rgb(
        (r1 + r2) / 2, 
        (g1 + g2) / 2, 
        (b1 + b2) / 2
    );
}
u32 col_mix_additive(u32 c1, u32 c2) {
    u8 r1, g1, b1, r2, g2, b2;
    col_get_rgb(c1, &r1, &g1, &b1);
    col_get_rgb(c2, &r2, &g2, &b2);

    return col_combine_rgb(
        clampi((i32)r1 + (i32)r2, 0, 0xff), 
        clampi((i32)g1 + (i32)g2, 0, 0xff), 
        clampi((i32)b1 + (i32)b2, 0, 0xff)
    );
}
u32 col_mix_spacial_linear(u32 c1, u32 c2) {
    u8 r1, g1, b1, r2, g2, b2;
    col_get_rgb(c1, &r1, &g1, &b1);
    col_get_rgb(c2, &r2, &g2, &b2);

    return col_combine_rgb(
        (u8)sqrtf(((f32)r1*(f32)r1 + (f32)r2*(f32)r2) / 2.0f),
        (u8)sqrtf(((f32)g1*(f32)g1 + (f32)g2*(f32)g2) / 2.0f),
        (u8)sqrtf(((f32)b1*(f32)b1 + (f32)b2*(f32)b2) / 2.0f)
    );
}
u32 col_mix_spacial_additive(u32 c1, u32 c2) {
    u8 r1, g1, b1, r2, g2, b2;
    col_get_rgb(c1, &r1, &g1, &b1);
    col_get_rgb(c2, &r2, &g2, &b2);

    return col_combine_rgb(
        (u8)clampi(sqrtf((f32)r1*(f32)r1 + (f32)r2*(f32)r2), 0, 0xff),
        (u8)clampi(sqrtf((f32)g1*(f32)g1 + (f32)g2*(f32)g2), 0, 0xff),
        (u8)clampi(sqrtf((f32)b1*(f32)b1 + (f32)b2*(f32)b2), 0, 0xff)
    );
}

inline void col_get_rgb(u32 c, u8 *r, u8 *g, u8 *b) {
    *r = (u8)(c >> 16);
    *g = (u8)(c >> 8);
    *b = (u8)(c);
}
inline void col_get_argb(u32 c, u8 *a, u8 *r, u8 *g, u8 *b) {
    *a = (u8)(c >> 24);
    *r = (u8)(c >> 16);
    *g = (u8)(c >> 8);
    *b = (u8)(c);
}

inline u32 col_combine_rgb(u8 r, u8 g, u8 b) {
    return 0xff000000u | ((u32)r << 16) | ((u32)g << 8) | (u32)b;
}
inline u32 col_combine_argb(u8 a, u8 r, u8 g, u8 b) {
    return ((u32)a << 24) | ((u32)r << 16) | ((u32)g << 8) | (u32)b;
}

u32 col_multiply(u32 col, f32 v) {
    u8 r, g, b;
    col_get_rgb(col, &r, &g, &b);

    return col_combine_rgb(
        (u8)((f32)r * v),
        (u8)((f32)g * v),
        (u8)((f32)b * v)
    );

}

inline u32 col_gray(u8 value) {
    return col_combine_rgb(value, value, value);
}

inline u32 col_invert(u32 c) {
    return c ^ 0xffffff;
}