#pragma once

#include "number.h"

#define WHITE 0xffffffff
#define BLACK 0xff000000
#define RED 0xffff0000
#define GREEN 0xff00ff00
#define BLUE 0xff0000ff


u32 col_mix_linear(u32 c1, u32 c2);
u32 col_mix_additive(u32 c1, u32 c2);
u32 col_mix_spacial_linear(u32 c1, u32 c2);
u32 col_mix_spacial_additive(u32 c1, u32 c2);

void col_get_rgb(u32 c, u8 *r, u8 *g, u8 *b);
void col_get_argb(u32 c, u8 *a, u8 *r, u8 *g, u8 *b);

u32 col_combine_rgb(u8 r, u8 g, u8 b);
u32 col_combine_argb(u8 a, u8 r, u8 g, u8 b);

u32 col_multiply(u32 col, f32 v);

u32 col_gray(u8 value);

u32 col_invert(u32 c);