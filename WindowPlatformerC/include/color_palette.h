#pragma once

#include "number.h"

typedef struct ColorPalette {
    u32 background, wall, player, danger, goal, portal, breakable;
} ColorPalette;

#include "object.h"

ColorPalette cpal_generate(u32 c);
ColorPalette *cpal_alloc(u32 c);
u32 cpal_get_col(const ColorPalette *pal, ObjectType objType);